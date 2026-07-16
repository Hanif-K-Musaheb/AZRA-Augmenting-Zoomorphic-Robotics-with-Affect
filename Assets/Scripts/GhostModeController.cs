using UnityEngine;
using UnityEngine.InputSystem;

public class GhostModeController : MonoBehaviour
{
	[Header("References")]
	[SerializeField] private Transform arQooboRoot; // AR robot root to move/scale
	[SerializeField] private Renderer[] bodyRenderers; // Mesh renderers to adjust transparency
	[SerializeField] private Transform followTarget; // Typically Camera.main transform; auto-filled if null

	[Header("Toggle")] 
	[SerializeField] private bool startInGhostMode = false;
	[SerializeField] private bool allowKeyboardToggle = true;
	[SerializeField] private Key toggleKeyFallback = Key.G; // Fallback key

	[Header("Ghost Settings")] 
	[SerializeField] private float ghostScaleFactor = 0.5f; // shrink to 50%
	[SerializeField] private float riseHeight = 0.3f; // meters upward on enter
	[SerializeField] private float transitionDuration = 1.5f; // seconds for enter/exit
	[SerializeField] private float thresholdForCatch = 0.5f; // meters from frisbee to consider "caught"

	[Header("Follow Settings")] 
	[SerializeField] private float preferredDistance = 0.5f; // meters from target
	[SerializeField] private float maxDriftSpeed = 1.0f; // m/s while following
	[SerializeField] private float turnSpeedDegPerSec = 240f; // yaw to face target
	[SerializeField] private float followSmoothing = 0.15f; // positional smoothing factor
	[SerializeField] private float heightOffset = 0.0f; // optional offset relative to target height
	[SerializeField] private Transform playerTransform; // drag your Main Camera here in the Inspector

	[Header("Fetch Settings")]
	[SerializeField] private Transform carryPoint; // empty child GameObject on the ghost, e.g. positioned near its "mouth"/front
	[SerializeField] private float MoveOnTimeThreshold = 10f;

	[Header("Jump Settings")]
	[SerializeField] private float jumpHeight = 0.4f; // Meters to jump upward
	[SerializeField] private float jumpDuration = 0.5f; // How long the jump takes
	private bool isJumping = false;

	private bool isGhost;
	private bool isTransitioning;
	private Vector3 originalPosition;
	private Quaternion originalRotation;
	private Vector3 originalScale;

	// Public property to check ghost state
	public bool IsGhost => isGhost;
	private enum FetchState { Idle, ChasingFrisbee, ReturningToPlayer }
	private FetchState fetchState = FetchState.Idle;
	private Transform carriedFrisbee = null;
	private bool fetchAllowed = false;
	private bool retrieveAllowed = false;
	private bool dropAllowed = false;
	private float TimeSinceLastCommandInSequence = 0f;
	private float RandomiseTimeRange; //used so that the retrieve command is not always exactly 3 seconds after the fetch command, but can be between 3 and 7 seconds


	void Awake()
	{
		if (arQooboRoot == null) arQooboRoot = transform;
	}

	void Start()
	{
		CacheOriginalPose();
		if (followTarget == null && Camera.main != null) followTarget = Camera.main.transform;
		if (startInGhostMode) EnterGhostModeImmediate();

		RandomiseTimeRange = Random.Range(0f, 4f);
	}

	void Update()
	{
		TimeSinceLastCommandInSequence += Time.deltaTime;

		// Debug.Log($"Time since last command in sequence: {TimeSinceLastCommandInSequence:F2} seconds");

		if (allowKeyboardToggle)
		{
			bool hashApprox = Keyboard.current != null && Keyboard.current.shiftKey.isPressed && Keyboard.current[Key.Digit3].wasPressedThisFrame;
			bool fallbackKey = Keyboard.current != null && Keyboard.current[toggleKeyFallback].wasPressedThisFrame;
			if (hashApprox || fallbackKey)
			{
				ToggleGhostMode();
			}
		}

		if (Keyboard.current != null)
		{
			// New Jump Command
			if (Keyboard.current[Key.Digit6].wasPressedThisFrame)
			{
				Debug.Log("Simulating command: JUMP");
				TriggerJump();
			}
		}


		// Step 1: If we're idle and a frisbee appears, kick off the fetch sequence.
		// This works whether Qoobo is currently a ghost or not - we enter ghost
		// mode ourselves rather than requiring it to already be active.
		if (fetchState == FetchState.Idle && fetchAllowed && frisbeeMarker.Current != null && frisbeeMarker.IsReleased && !isTransitioning)
		{
			fetchState = FetchState.ChasingFrisbee;
			fetchAllowed = false;//requires fetch command every time
			if (!isGhost)
			{
				StartCoroutine(EnterGhostMode());
			}
			TimeSinceLastCommandInSequence = 0f; // reset the timer for the fetch sequence
		}

		// Step 2: Only move/chase/return once we're actually a ghost and not
		// mid-transition (rising up, shrinking, etc).
		if (isGhost && !isTransitioning)
		{
			switch (fetchState)
			{
				case FetchState.ChasingFrisbee:
					if (frisbeeMarker.Current == null)
					{
						// Frisbee vanished (e.g. someone deleted it) - abandon fetch, go home
						fetchState = FetchState.Idle;
						followTarget = playerTransform;
					}
					else
					{
						followTarget = frisbeeMarker.Current;
						if (IsNearFrisbee(arQooboRoot.position, frisbeeMarker.Current.position, thresholdForCatch)&&
						(retrieveAllowed||TimeSinceLastCommandInSequence>(MoveOnTimeThreshold+RandomiseTimeRange)))
						//if retrieveAllowed is false, then we can pick up the frisbee after 3 seconds of chasing it
						{
							PickUpFrisbee(frisbeeMarker.Current);
							fetchState = FetchState.ReturningToPlayer;
							retrieveAllowed = false; // requires retrieve command every time//possible error here <--
							TimeSinceLastCommandInSequence = 0f; // reset the timer for the fetch sequence
						}
					}
					break;

				case FetchState.ReturningToPlayer:
					followTarget = playerTransform;
					if (IsNearFrisbee(arQooboRoot.position, playerTransform.position, thresholdForCatch)&&
					(dropAllowed||TimeSinceLastCommandInSequence>(MoveOnTimeThreshold+RandomiseTimeRange)))//reusing the IsNearFrisbee function to check if we are close enough to the player to drop the frisbee :/
					{
						DropFrisbeeAndReturnToNormal();
						dropAllowed = false;//<--possible error here, requires drop command every time
					}
					break;

				case FetchState.Idle:
					// No fetch happening - just follow the player as normal ghost behavior
					followTarget = playerTransform;
					break;
			}

			FollowTargetUpdate();
		}
	}

	private void PickUpFrisbee(Transform frisbee)
{
    Transform attachPoint = carryPoint != null ? carryPoint : arQooboRoot;

    // Stop physics from affecting the frisbee while it's being carried
    Rigidbody frisbeeRb = frisbee.GetComponent<Rigidbody>();
    if (frisbeeRb != null)
    {
        frisbeeRb.isKinematic = true; // disables gravity + physics forces
        frisbeeRb.velocity = Vector3.zero; // stop any leftover momentum from the throw also it is linearVelocity in newer versions of Unity
    }

	//this turns off the colliders so the player cant take the frisbee from Qoobo
	Collider[] colliders = frisbee.GetComponentsInChildren<Collider>();
    foreach(Collider col in colliders)
    {
        col.enabled = false;
    }

    frisbee.SetParent(attachPoint);
    frisbee.localPosition = Vector3.zero;
    frisbee.localRotation = Quaternion.identity;

	carriedFrisbee = frisbee;
}

	private void DropFrisbeeAndReturnToNormal()
	{
		if (carriedFrisbee != null)
		{
			Destroy(carriedFrisbee.gameObject);
			carriedFrisbee = null; 
		}

		fetchState = FetchState.Idle;
		StartCoroutine(ExitGhostMode());
	}




	private bool IsNearFrisbee(Vector3 quooboPos, Vector3 frisbeePos, float threshold)//fix this to be universal distance check, not just for frisbee. change frisbee threshold
	{
		Vector3 quooboPosFlat = new Vector3(quooboPos.x, 0f, quooboPos.z);
		Vector3 frisbeePosFlat = new Vector3(frisbeePos.x, 0f, frisbeePos.z);
		float horizontalDist = Vector3.Distance(quooboPosFlat, frisbeePosFlat);

		return horizontalDist <= threshold;
	}

	public void ToggleGhostMode()
	{
		if (isTransitioning) return;
		if (!isGhost) StartCoroutine(EnterGhostMode());
		else StartCoroutine(ExitGhostMode());
	}

	private void CacheOriginalPose()
	{
		originalPosition = arQooboRoot.position;
		originalRotation = arQooboRoot.rotation;
		originalScale = arQooboRoot.localScale;
	}

	private System.Collections.IEnumerator EnterGhostMode()
	{
		isTransitioning = true;
		CacheOriginalPose();
		Vector3 startPos = arQooboRoot.position;
		Quaternion startRot = arQooboRoot.rotation;
		Vector3 startScale = arQooboRoot.localScale;
		Vector3 endPos = startPos + Vector3.up * Mathf.Max(0f, riseHeight);
		Vector3 endScale = originalScale * Mathf.Clamp(ghostScaleFactor, 0.1f, 1.0f);

		// Hardcoded alpha values: 0 (transparent) to 0.5 (semi-transparent)
		float startAlpha = 0f;
		float endAlpha = 0.5f;

		float t = 0f;
		while (t < transitionDuration)
		{
			t += Time.deltaTime;
			float k = Mathf.SmoothStep(0f, 1f, t / transitionDuration);
			arQooboRoot.position = Vector3.Lerp(startPos, endPos, k);
			arQooboRoot.localScale = Vector3.Lerp(startScale, endScale, k);
			SetBodyAlpha(Mathf.Lerp(startAlpha, endAlpha, k));
			yield return null;
		}

		arQooboRoot.position = endPos;
		arQooboRoot.localScale = endScale;
		SetBodyAlpha(endAlpha);
		isGhost = true;
		isTransitioning = false;
	}

	private void EnterGhostModeImmediate()
	{
		CacheOriginalPose();
		arQooboRoot.position = originalPosition + Vector3.up * Mathf.Max(0f, riseHeight);
		arQooboRoot.localScale = originalScale * Mathf.Clamp(ghostScaleFactor, 0.1f, 1.0f);
		SetBodyAlpha(0.5f); // Hardcoded semi-transparent
		isGhost = true;
		isTransitioning = false;
	}

	private System.Collections.IEnumerator ExitGhostMode()
	{
		isTransitioning = true;
		Vector3 startPos = arQooboRoot.position;
		Vector3 startScale = arQooboRoot.localScale;

		Vector3 endPos = originalPosition;
		Vector3 endScale = originalScale;

		// Hardcoded alpha values: 0.5 (semi-transparent) to 0 (fully transparent)
		float startAlpha = 0.5f;
		float endAlpha = 0f;

		float t = 0f;
		while (t < transitionDuration)
		{
			t += Time.deltaTime;
			float k = Mathf.SmoothStep(0f, 1f, t / transitionDuration);
			arQooboRoot.position = Vector3.Lerp(startPos, endPos, k);
			arQooboRoot.localScale = Vector3.Lerp(startScale, endScale, k);
			SetBodyAlpha(Mathf.Lerp(startAlpha, endAlpha, k));
			yield return null;
		}

		arQooboRoot.position = endPos;
		arQooboRoot.localScale = endScale;
		SetBodyAlpha(endAlpha);
		isGhost = false;
		isTransitioning = false;
	}

	private void FollowTargetUpdate()
	{
		if (followTarget == null) return;
		Vector3 targetPos = followTarget.position + Vector3.up * heightOffset;
		Vector3 toTarget = targetPos - arQooboRoot.position;
		Vector3 toTargetHorizontal = new Vector3(toTarget.x, 0f, toTarget.z);
		float dist = toTargetHorizontal.magnitude;

		// Maintain preferred distance in the horizontal plane
		Vector3 desiredOffset = Vector3.zero;
		if (dist > Mathf.Epsilon)
		{
			float delta = dist - preferredDistance;
			desiredOffset = toTargetHorizontal.normalized * delta;
		}

		Vector3 desiredPos = arQooboRoot.position + desiredOffset;
		// Smooth drift
		Vector3 newPos = Vector3.Lerp(arQooboRoot.position, desiredPos, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.001f, followSmoothing)));
		// Clamp max speed
		Vector3 deltaMove = newPos - arQooboRoot.position;
		float maxStep = maxDriftSpeed * Time.deltaTime;
		if (deltaMove.magnitude > maxStep) newPos = arQooboRoot.position + deltaMove.normalized * maxStep;
		arQooboRoot.position = newPos;

		// Face the user
		Vector3 lookDir = (arQooboRoot.position - followTarget.position); // Flipped: robot looks toward user
		lookDir.y = 0f;
		if (lookDir.sqrMagnitude > 0.0001f)
		{
			Quaternion targetYaw = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
			arQooboRoot.rotation = Quaternion.RotateTowards(arQooboRoot.rotation, targetYaw, turnSpeedDegPerSec * Time.deltaTime);
		}
	}

	private void SetBodyAlpha(float alpha)
	{
		if (bodyRenderers == null) return;
		for (int i = 0; i < bodyRenderers.Length; i++)
		{
			var r = bodyRenderers[i];
			if (r == null || r.sharedMaterial == null) continue;
			
			// Simple approach: just set the color alpha
			Color c = r.material.color;
			c.a = alpha;
			r.material.color = c;
			Debug.Log($"Set renderer {i} alpha to: {alpha}");
		}
	}

	private float GetCurrentAlpha()
	{
		if (bodyRenderers == null) return 0f; // Default to transparent
		for (int i = 0; i < bodyRenderers.Length; i++)
		{
			var r = bodyRenderers[i];
			if (r != null && r.sharedMaterial != null) return r.material.color.a;
		}
		return 0f; // Default to transparent
	}

	public void AllowFetch()
	{
		fetchAllowed = true;
	}
	public void AllowRetrieve()
	{
		retrieveAllowed = true;
	}
	public void AllowDrop()
	{
		dropAllowed = true;
	}

	public void TriggerJump()
	{
		// Prevent overlapping jumps or jumping while transitioning states
		if (isJumping || isTransitioning) return;

		if (!isGhost)
		{
			// If not a ghost, enter ghost mode first, then jump
			StartCoroutine(EnterGhostModeAndJump());
		}
		else
		{
			// If already a ghost, just jump
			StartCoroutine(JumpRoutine());
		}
	}

	private System.Collections.IEnumerator EnterGhostModeAndJump()
	{
		// Wait for the standard ghost transition to finish
		yield return StartCoroutine(EnterGhostMode());
		
		// Then execute the jump
		yield return StartCoroutine(JumpRoutine());
	}

	private System.Collections.IEnumerator JumpRoutine()
	{
		isJumping = true;
		float elapsed = 0f;
		Vector3 startPos = arQooboRoot.position;
		Vector3 peakPos = startPos + Vector3.up * jumpHeight;

		// Upward Phase (ease out)
		while (elapsed < jumpDuration / 2f)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / (jumpDuration / 2f);
			float easeT = Mathf.Sin(t * Mathf.PI * 0.5f); // Smooth deceleration at the top
			
			arQooboRoot.position = new Vector3(arQooboRoot.position.x, Mathf.Lerp(startPos.y, peakPos.y, easeT), arQooboRoot.position.z);
			yield return null;
		}

		elapsed = 0f;
		
		// Downward Phase (ease in)
		while (elapsed < jumpDuration / 2f)
		{
			elapsed += Time.deltaTime;
			float t = elapsed / (jumpDuration / 2f);
			float easeT = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); // Smooth acceleration downward
			
			arQooboRoot.position = new Vector3(arQooboRoot.position.x, Mathf.Lerp(peakPos.y, startPos.y, easeT), arQooboRoot.position.z);
			yield return null;
		}

		// Snap to exact original Y height to prevent floating point drift
		arQooboRoot.position = new Vector3(arQooboRoot.position.x, startPos.y, arQooboRoot.position.z);
		isJumping = false;
	}

	
}



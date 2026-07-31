import pyaudio
import wave
import threading
import os

# --- Audio Settings ---
CHUNK = 1024
FORMAT = pyaudio.paInt16
CHANNELS = 1 # Mono recording
RATE = 44100

def record_audio(filename, stop_event):
    """Records audio in a background thread until stop_event is set."""
    p = pyaudio.PyAudio()
    stream = p.open(format=FORMAT, channels=CHANNELS, rate=RATE, input=True, frames_per_buffer=CHUNK)
    
    print(f"\n🔴 RECORDING STARTED: {filename}")
    print("Press [Enter] to STOP recording...")
    
    frames = []
    
    # Keep recording until the main thread says stop
    while not stop_event.is_set():
        data = stream.read(CHUNK)
        frames.append(data)
        
    # Stop and close the stream
    stream.stop_stream()
    stream.close()
    p.terminate()
    
    # Save to .wav
    wf = wave.open(filename, 'wb')
    wf.setnchannels(CHANNELS)
    wf.setsampwidth(p.get_sample_size(FORMAT))
    wf.setframerate(RATE)
    wf.writeframes(b''.join(frames))
    wf.close()
    print(f"✅ SAVED: {filename}\n")

def main():
    print("=== ZOOMORPHIC ROBOT STUDY RECORDER ===")
    participant_id = input("Enter Participant ID (e.g., P01): ").strip()
    
    # Define the order for this specific participant
    print("\nEnter the 5 features in the order they will be tested.")
    print("(e.g., fetch, train, custom, tv, emotion)")
    features_input = input("Feature order (comma separated): ").strip()
    
    # Clean up the input into a list
    features = [f.strip().replace(" ", "_") for f in features_input.split(",")]
    
    # Create a directory for this participant to keep things tidy
    os.makedirs(participant_id, exist_ok=True)
    
    print(f"\nSetup complete for {participant_id}. Ready to begin study.")
    print("-" * 40)
    
    for index, feature in enumerate(features):
        input(f"Feature {index + 1}/{len(features)}: [{feature}]. \nPress [Enter] when ready to START recording...")
        
        filename = os.path.join(participant_id, f"{participant_id}_{index + 1}_{feature}.wav")
        stop_event = threading.Event()
        
        # Start recording in the background so we can wait for the next Enter press
        record_thread = threading.Thread(target=record_audio, args=(filename, stop_event))
        record_thread.start()
        
        # Wait for the user to press Enter to stop
        input() 
        stop_event.set()
        record_thread.join()

    print(f"🎉 All {len(features)} features recorded for {participant_id}. Session Complete!")

if __name__ == "__main__":
    main()

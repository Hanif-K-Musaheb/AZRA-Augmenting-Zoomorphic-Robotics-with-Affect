# [CHANGE LOG](https://github.com/Hanif-K-Musaheb/AZRA-Augmenting-Zoomorphic-Robotics-with-Affect/blob/main/README.md)
### v1.2.3
- fixed bug where a user can take the frisbee from Qoobo's mouth
- Qoobo can now hear user commands to fetch, retrieve and drop frisbee
- I added a timer so that if a user has be trying to tell Qoobo to retrieve or drop the frisbee for 10s + random amount of time between 0-3 it will automatically do so a user doesn't get stuck
- "catch" is the most understood version of fetch by Vosk
- **BUGS:** where Qoobo carries frisbee, Allow user to take frisbee out of qoobos mouth, **search for better more understable words for retrieve and drop for vosk**
### v1.3.0
- this is adding the tv feature
- working virtual screen playing the blue planet clip for ~7.5 min
- working menu button
- **BUGS:** blue planet clip not commited to here as file was to large, menu button goes white between clicks
### v1.3.1
- fixed tv button greying issue
- deleted unecessary files
### v1.4.0
- added traing menu button
- changed donut logic so that any donut can be eaten and it the new script is attached to each prefab
- added donut box
- added a feature where when you press 6 qoobo will jump
### v1.4.1
- added spin press key 7 , roll press key 8 , flip press key 9
### v1.4.2
- minor fix inplacement to tv made a tv spawn point so it woudnt sometime spawn behind a menu
- added Llama-3.2-1B-instruct-Q4_K_M.gguf model [found here](https://huggingface.co/bartowski/Llama-3.2-1B-Instruct-GGUF/blob/main/Llama-3.2-1B-Instruct-Q4_K_M.gguf)
  - it is not committed to the repo because it is too big
- new script to test this in the editor as i havent hooked it up to vosk yet
- new script called Qoobo brain where the model goes
- new txt [System Prompt](https://github.com/Hanif-K-Musaheb/AZRA-Augmenting-Zoomorphic-Robotics-with-Affect/blob/main/Assets/Scripts/System%20Prompt.txt) this is the prompt the model sees before running very important, I am curently updating it with the newest system prompt i make to record that
### v1.4.3
- started on the connection to from Qoobo brain to GhostMode to preform trick (in Editor)
- currently works with the jump trick but not the spin trick I don't know why, also always has a JSON error with the LLM first


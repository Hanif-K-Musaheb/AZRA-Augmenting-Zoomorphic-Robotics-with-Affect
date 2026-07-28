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
### v1.4.4
- all the moves work now translated from the mock vosk script input by the actual SLM
- solved the 1 spin problem instead of 3, it was because i wasn't giving it enough time to go into ghostMode
- however there is still some major problems: i asked it to the following ```fakeSpeechInput = "hey qoobo can you do two rolls for me then a flip";``` and it understood rolls to be a move not 2x roll but 2x rolls, flip was done correctly
### v1.4.5
- "do a flip" works but is a little buggy but i reduced the scope of the SLM to only 4 tricks read the  [System Prompt](https://github.com/Hanif-K-Musaheb/AZRA-Augmenting-Zoomorphic-Robotics-with-Affect/blob/main/Assets/Scripts/System%20Prompt.txt)
- come to a bit of a dead end with how accurate i can get this with VOSK being a bit of a bottle neck, will try to further this if spare time on the project but for now i am going to try and wizzard of oz it
- make some fixes to bugs which caused the fetch feature to fail
### v1.5.0
- completed wizzard of ozing the trick learner feature where, the study assesor will do the tricks using the a, b, x, y buttons. It will do 1 in the moveset i narrowed it down to so that it requires little skill and timing from the assesor.
- also completed the emotion show feature has a working button does the emotions perhaps adding more in the future will improve it.
- **There is a potential bug that the training could be on while the emotion show is on. I need to add something to make sure only one feature is active at a time**
### v1.5.1
- **added menuController** this will fix bug where feature overlap now when you select a button on the menu it turns off other overlapping features i.e training and emotionShow
- **modified emotionShow, FrisbeeController, ObjectToggler, TrainController** all to accomodate the new MenuController
### v1.5.2
- **added EndOfFeatureQuestionaire** this creates a window infront of the user so they can assess the feature using the likert questions submitted to the ethics board
- **modified MenuController** now it works with all 5 features added (emotionShow, train, customisation, TV, frisbee), I redid so that there is now an internal current feature which is useful for record purposes and standardising the codebase a bit
- **modified WoZManager** so that I can just click the joy stick down and the questionaire will appear.

### v1.5.3
-**added FeatureSignController** adds a sign to all the new feature explaining what they are and how to use them
### v1.5.4
-**added hatInteractive** which allows you to pick up the hats and place them on Qoobo's head
-**added hatManager** essentially just spawns the hats in and manages which hat is on qoobos head and then parents that hat to them when they go to another feature.
-_completly working and found no bugs during pilot test no polishing needed_





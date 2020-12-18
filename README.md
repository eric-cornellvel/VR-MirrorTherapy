﻿# VR-MirrorTherapy
VR-MirrorTherapy was designed to accommodate patients using consumer virtual reality equipment for MirrorTherapy. As in conventional mirror visual feedback, movement from the patient’s uninjured hand was transformed over the midline to symmetrically animate their avatar hand on the injured side. Thus, patients could use their right or their left hand to control the movements of both avatar hands, just as in conventional mirror therapy, and see both hands move freely and easily.

## Requirements
- Unity 2019.4 LTS verison
- Animation Rigging package (for Upper Body IK)
- Oculus Intergration for Unity (for finger tracking on Quset and Quest 2)

## Supported Platforms
- Oculus Rift, Rift S
- Oculus Quest, Quset2
- HTC VIVE and other devices that reuires SteamVR (under-development)

## How to run?
- Step1: Customize the Mirror Therapy Settings in VR (usually done via research assistants).
- Step2: Guide the patient to their seats and wear the VR equipments. Then click "Calibration" button in the UI to fit the avatar's height to participant's height.
- Step3: When it's ready, click "start" to start the target hitting tasks.
- Step4 (Optional): Copy out the recorded data for analysis.


## Mirror Therapy Settings
All the settings are shown in a UI canvas. Use trigger button to click the button with VR laser pointer.
Press X button (Left Controller) or A button (Right Controller) to show/hide the UI canvas
Press Y button (Left Controller) or B button (Right Controller) to show/hide the Mirror

### Avatar Gender


### Avatar Skin Color


### Mirroring mode


## Target Hitting Task
In the target-hitting task, participants held the Touch controller in their unaffected hand or directly used their unaffected hand if the VR system supported finger tracking. For participants suffering from CRPS of the right upper limb, both avatar hands were controlled using the left controller. For participants suffering from CRPS of the left upper limb, this was reversed and both avatar hands were controlled using the right controller. Each condition was saved in a different “scene” in the Unity file. 

### Modifying Mirroring
3 different mirroring modes (i.e. No Mirror, Mirror Left and Mirror Right) can be easily modified by selecting in the UI

### Modifying Target Size, Shape and Position.
When the researcher pressed the spacebar key, a series of targets of three different sizes appeared in random order in the midline in front of the participant as shown in Figure 2 (right). Eighteen targets (six large, six medium, and six small translucent balls, examples seen in Figure 3) appeared in a random order in the midline in front of each participant.  When an avatar hand makes contact with the target, a chime sounded, and the target object disappeared. Both the sound and the size and appearance of the target object can be readily modified for future iterations of this experiment. A separate “Event” data file records the position of each target hit, with the position of the avatar hands at the time of contact. The data was saved in the same folder with other movement data as described in the next section.

## Movement Data
Most consumer VR systems contain trackers on the head mounted display and two hand controllers, each of which captures both position (X,Y,Z) and orientation (pitch, yaw, roll) data at each frame. The default setting in our current module is to record every third frame, for a rate of approximately 30 frames per second. However, the frame rate can be modified to record at a higher or lower rate.

### Retrieving Movement Data From Different Systems
We saved movement data for each tracker to a separate CSV file at the end of the experiment.When the VR system is connected with a PC/Laptop (e.g. Oculus Rift,  Rift S and Ouest with Oculus Link), the data was saved in the folder below 
```shell 
C:\Users\<userprofile>\AppData\LocalLow\CornellVel\MirrorTherapy_V2\files\recording
```
For the standalone Oculus Quest/Quest 2, the files were saved at the folder below 
```shell 
/storage/sdcard0/Android/data/com.CornellVel.MirrorTherapy_V2/files/recording/
```
We note that this software is not a commercial system or patient-ready application. Rather, it is designed to be used as a potential starting point for research for other clinicians interested in exploring potential therapeutic uses of immersive virtual reality for visual feedback on movement. 

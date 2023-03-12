# Unity Voice Chat


There are numerous alternatives for voice chat mechanisms in unity. This research was mainly focused on finding the best alternative that can work smoothly with the Netcode for Game Objects package.
The best alternative seems to be Vivox voice chat which is a Pre-Release Unity service that can be enabled through the Unity Dashboard. After enabling it, the necessary packages should be added to the project and imported.
General structure of Vivox voice chat is somewhat similar to the Relay server which is our  multiplayer mechanism selection.

<a href="url"><img src="https://github.com/aeboyaci/VisionEye/blob/main/RnD/VoiceChat/Vivox.jpg" height="270" width="630" ></a>


There is a dedicated server provided by Vivox alongside with access token, issuer and domain. These variables determine the server to be connected. Once the connection is established, users can join a channel by providing the channel name. Once the integration with the project is completed in Sprint-3, we will be able to test the voice chat thoroughly.
In case of a failure or a mismatch between packages, solutions using other voice chat alternatives will be implemented (such as Photon Voice alongside with PUN). It is also possible for voice chat to be a completely independent service. Further testing will finalize the selection.

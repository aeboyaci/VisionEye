

# Multiplayer Mechanics For Unity

The mechanism that we are planning to use is the Relay Server architecture. Here, the relay server is the main server that other players connect. 

We will be using Netcode for Game Objects package of Unity as it allows the users to send game objects instead of specific instructions which makes the synchronizing quite easy. By synchronizing, we mean the location and movements of each player.

![alt text](https://github.com/aeboyaci/VisionEye/blob/30d1567053cb70331eff4248c539704d9ae277ce/RnD/Multi-Player/Image%201.02.2023%20at%2021.32.jpg)


In the game, the players will be getting their team information from our API and each team will have an independent relay server. Once a team leader is chosen, a relay server will start running on his side. The other team members will be invited by the team leader and again, these invitations will get them through the API.

Once the players get their team information, they will be able to connect to the relay server for their team by a unique team code.

Relay server will only be responsible for the mechanics, the scoreboard, achievements and other game statistics will be sent and saved through the API.



**GET** `/oauth2/google/sign-in`

- If the user successfully sign in via Google, Google redirects the user to this endpoint
- Response: Redirect to Google to sign in the user

**Acceptance Criteria:**

- Implement following endpoint: 
[https://github.com/aeboyaci/VisionEye/blob/main/RnD/OAuth/Demo_Implementation/routers/auth.js#L6](https://github.com/aeboyaci/VisionEye/blob/main/RnD/OAuth/Demo_Implementation/routers/auth.js#L6)

**GET** `/oauth2/google/callback/failure`

- If the user fails to sign in via Google, the api redirects the user to this endpoint
- Response:
    
  ```json
  {
    "success": false,
    "error": "login failed!"
  }
  ```
    

**Acceptance Criteria:**

- Implement following endpoint:
[https://github.com/aeboyaci/VisionEye/blob/main/RnD/OAuth/Demo_Implementation/routers/auth.js#L12](https://github.com/aeboyaci/VisionEye/blob/main/RnD/OAuth/Demo_Implementation/routers/auth.js#L12)

**GET** `/oauth2/google/logout`

- Logout the user via ACCESS_TOKEN
- Headers:
    - Cookies → ACCESS_TOKEN

**Acceptance Criteria:**

- Use passport.js to logout user via Google

**POST** `/teams/:teamId/invitations`

- Headers:
    - Cookies → ACCESS_TOKEN
    - Content-Type → “application/json”
- Body:
    
  ```json
  {
    "playerId": "..."
  }
  ```
    
- Response:
    
  ```json
  {
    "success": true
  }
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is a member of the team with given team id
- Add new entry to “invitation” table. Entry values:
    - sender → deserialized player’s id
    - receiver → “playerId” in request body
    - status → “PENDING”
    - team_id → “teamId” in URL params

**GET** `/teams/:teamId/invitations`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
  ```json
  [
    {
      "playerId": "...",
      "displayName": "...",
      "avatarUrl": "...",
      "status": "PENDING / ACCEPTED / REJECTED",
    },
    {
      "playerId": "...",
      "displayName": "...",
      "avatarUrl": "...",
      "status": "PENDING / ACCEPTED / REJECTED",
    }
  ]
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is a member of the team with given team id
- Get all invitations with “team_id = teamId (URL params)” from “invitation” table

**GET** `/invitations`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
  ```json
  [
    {
      "sender": {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "..."
      },
      "team": {
        "name": "...",
        "players": [
          {
            "playerId": "...",
            "displayName": "...",
            "avatarUrl": "...",
            "isCaptain": true
          },
          {
            "playerId": "...",
            "displayName": "...",
            "avatarUrl": "...",
            "isCaptain": false
          }
        ]
      }
    }
  ]
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is a member of the team with given team id
- Get all invitations with “team_id = teamId (URL params) AND receiver = deserializedPlayer.id” from “invitation” table joining with the “team” and “team_has_players” tables

**POST** `/invitations`

- Headers:
    - Cookies → ACCESS_TOKEN
    - Content-Type → “application/json”
- Body:
    
  ```json
  {
    "id": "...",
    "status": "ACCEPTED / REJECTED"
  }
  ```
    
- Response:
    
  ```json
  {
    "success": true
  }
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is a member of the team with given team id
- Update “invitation” entry’s “status” field with “status” in request body
- Update all others “invitation” entries with pseudo condition (If a player accepts an invitation, he/she rejects all others):
    - “status = ‘PENDING’ AND receiver = deserializedPlayer.id AND id ! = requestBody.id”

**GET** `/teams`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
  ```json
  [
    {
      "name": "...",
      "totalScoreGained": 150,
      "minutesPlayed": 25
    },
    {
      "name": "...",
      "totalScoreGained": 60,
      "minutesPlayed": 10
    }
  ]
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is a member of the team with given team id
- Get values from database with following:
    - join “team” table with “team_has_players” table ON team.id/team_has_players.team_id AND team_has_players.player_id/deserializedPlayer.id → (1)
    - join (1) with “game” table ON (1).team.id/game.team_id → (2)
    - join (2) with “player_has_achievements” ON (2).game.id/player_has_achievements.game_id AND player_has_achievements.player_id/deserializedPlayer.id → (3)
    - join (3) with “achievement” ON (3).player_has_achievements.achievement_id/achievement.id → (4)
- Aggregate the values to sum achievement scores, and calculate game durations (game.end_date - game.start_date)

**GET** `/teams/create`

- Headers:
    - Cookies → ACCESS_TOKEN
    
- Response:
    
  ```json
  {
    "success": true,
    "teamId": "..."
  }
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN
- Add new entry to “team” table with empty team name
- Add new entry to “team_has_players” with following values:
    - team_id → “id” field of the entry added to “team” table
    - player_id → deserialized player’s id
    - is_captain → true
    - is_active → true

**POST** `/teams/update`

- Headers:
    - Cookies → ACCESS_TOKEN
    - Content-Type → “application/json”
- Body:
    
  ```json
  {
    "team": {
      "id": "...",
      "name": "..."
    },
    "relayServer": {
      "ipv4": "...",
      "port": 1234,
      "joinCode": "..."
    }
  }
  ```
    
- Response:
    
  ```json
  {
    "success": true
  }
  ```
    

**Acceptance Criteria:**

- Deserialize player from ACCESS_TOKEN, and check if the player is the captain member of the team with given team id
- Update the entry with team id in "team" table
- Add new entry to "relay_server" table. Entry:
    - team_id → “id” field of the entry added to “team” table
    - ipv4 → "relayServer.ipv4" field in request body
    - port → "relayServer.port" field in request body
    - join_code → "relayServer.joinCode" field in request body

**GET** `/teams/:teamId`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
  ```json
  {
    "name": "...",
    "games": [
      {
        "roomName": "...",
        "score": 50,
        "minutesPlayed": 2,
        "startDate": "date string"
      }
    ],
    "players": [
      {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "...",
        "isCaptain": true,
        "isOnline": true
      }
    ]
  }
  ```
    

**Acceptance Criteria:**

- join “team” table with “team_has_players” table ON team.id/team_has_players.team_id → (1)
- Check if the deserialized player is a member of the team from (1) [can use “deserializedPlayer.id IN (…)” in SQL query]
- join “player” table with (1) ON (1).team_has_players.player_id/player.id → (2)
- join “game” table with (1) ON (1).team.id/game.team_id → (3)
- join “room” table with (3) ON (3).room_id/room.id → (4)
- join “player_has_achievements” table with (3) ON (3).game.id/player_has_achievements.game_id AND player_has_achievements.player_id/deserializedPlayer.id → (5)
- join “achievement” table with (4) ON (4).achievement_id/achievement.id → (6)
- Aggregate the values to sum achievement scores, and calculate game durations (game.end_date - game.start_date)

**POST** `/games`

- Headers:
    - Cookies → ACCESS_TOKEN
    - Content-Type → “application/json”
- Body:
    
  ```json
  {
    "teamId": "...",
    "roomId": "..."
  }
  ```
    
- Response:
    
  ```json
  {
    "success": true
  }
  ```
    

**Acceptance Criteria:**

- Check if the team has 4 players
- Add new entry to “game” table

**GET** `/games/:gameId`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
    ```json
    {
      "roomName": "...",
      "minutesPlayed": 2,
      "score": 50,
      "achievements": [
        {
          "id": "...",
          "name": "...",
          "description": "...",
          "score": 10,
          "owner": "{{owner player's display name}}"
        }
      ]
    }
    ```


**Acceptance Criteria:**

- Get player’s score via joining “player_has_achievements”, “achievement”, “game” tables
- Get room name via joining "game", "room" tables
- Get all achievements with player information via joining "game", "player_has_achievements", "player", "achievement" tables
- Get player's achievements to calculate score via joining "game", "player_has_achievements", "achievement" tables considering deserialized player's player id

**POST** `/achievements`

- Headers:
    - Cookies → ACCESS_TOKEN
    - Content-Type → “application/json”
- Body:
    
  ```json
  {
    "achievementId": "...",
    "gameId": "..."
  }
  ```
    
- Response:
    
  ```json
  {
    "success": true
  }
  ```
    

**Acceptance Criteria:**

- Add new entry to “achievement” table. Entry:
    - player_id → deserialized player’s id
    - game_id → “gameId” in request body
    - achievement_id → “achievementId” in request body

**GET** `/scoreboard`

- Headers:
    - Cookies → ACCESS_TOKEN
- Response:
    
  ```json
  [
    {
      "playerId": "...",
      "displayName": "...",
      "avatarUrl": "...",
      "score": 123
    },
    {
      "playerId": "...",
      "displayName": "...",
      "avatarUrl": "...",
      "score": 122
    }
  ]
  ```
    

**Acceptance Criteria:**

- Get players’ scores via joining “player_has_achievements”, “achievement”, “player” tables, and order by their score

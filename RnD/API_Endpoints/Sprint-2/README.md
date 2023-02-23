**GET** `/`

- Response: A webpage that has the OTP-code input along with the Scary Verse logo

**Acceptance Criteria:**

- Build a simple webpage using Handlebars
- Return the webpage

**GET** `/status`

- Response: A webpage showing the authentication status along with the Scary Verse logo

**Acceptance Criteria:**

- Build a simple webpage using Handlebars
- Get the "otp_code" from the cookies, check if tokens set in the Cache
- Return the webpage

**GET** `/oauth2/google/passcode`

- Response:

  ```json
  {
    "success:": true,
    "data": "[ GENERATED_OTP_CODE ]"
  }
  ```

**Acceptance Criteria:**

- Generate a random 6-digit OTP code, and store it in the Cache where key is the OTP-code and value is the user's tokens

  (e.g. 123456: null)

  (e.g. 123456: { accessToken: "[ ACCESS_TOKEN ]", idToken: "[ ID_TOKEN ]" })

- Return the generated OTP code

**GET** `/oauth2/google/passcode/:code`

- Response: Google's Sign-In page

**Acceptance Criteria:**

- Set a cookie named "otp_code" with the value of the "code" parameter in URL params
- Generate Google OAuth2 authentication URL, and redirect user to the URL

**GET** `/oauth2/google/callback`

- Response: Webpage showing the authentication status

**Acceptance Criteria:**

- Get the "otp_code" from the cookies, and update the Cache value with deserialized user's tokens
- Redirect user to the "Authentication Status Page"

**GET** `/oauth2/google/passcode/:code/status`

- Response:

  ```json
  {
    "success:": true,
    "data": {
      "status": "WAITING / COMPLETED",
      "tokens": {
        "accessToken": "[ACCESS_TOKEN]",
        "idToken": "[ID_TOKEN]"
      }
    }
  }
  ```

**Acceptance Criteria:**

- Get tokens from the Cache with the "code" in URL params
- Generate Google OAuth2 authentication URL, and redirect user to the URL

**GET** `/oauth2/google/logout`

- Logout the user via ACCESS_TOKEN
- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success:": true,
    "data": "logged out"
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and update

**GET** `/players`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN

- Response:

  ```json
  {
    "success": true,
    "data": [
      {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "..."
      },
      {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "..."
      }
    ]
  }
  ```

**Acceptance Criteria:**

- Get online players from "player" table

**GET** `/players/me`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success:": true,
    "data": {
      "displayName": "...",
      "avatarUrl": "..."
    }
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, then return the player's information

**POST** `/teams/:teamId/invitations`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
- Body:

  ```json
  {
    "playerId": "..."
  }
  ```

- Response:

  ```json
  {
    "success": true,
    "data": "player successfully invited"
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is a member of the team with given team id
- Add new entry to "invitation" table. Entry values:
    - sender → deserialized player’s id
    - receiver → "playerId" in request body
    - status → "PENDING"
    - team_id → "teamId" in URL params

**GET** `/teams/:teamId/invitations`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data" : [
      {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "...",
        "status": "PENDING / ACCEPTED / REJECTED"
      },
      {
        "playerId": "...",
        "displayName": "...",
        "avatarUrl": "...",
        "status": "PENDING / ACCEPTED / REJECTED"
      }
    ]
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is a member of the team with given team id
- Get all invitations with "team_id = teamId (URL params)" from "invitation" table

**GET** `/invitations`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": [
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
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is a member of the team with given team id
- Get all invitations with "team_id = teamId (URL params) AND receiver = deserializedPlayer.id" from "invitation" table
  joining with the "team" and "team_has_players" tables

**POST** `/invitations`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
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
    "success": true,
    "data": "invitation set to [request.body.status]"
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is a member of the team with given team id
- Update "invitation" entry’s "status" field with "status" in request body
- Update all others "invitation" entries with pseudo condition (If a player accepts an invitation, he/she rejects all
  others):
    - "status = ‘PENDING’ AND receiver = deserializedPlayer.id AND id ! = requestBody.id"

**GET** `/teams`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": [
      {
        "id": "...",
        "name": "...",
        "totalScoreGained": 150,
        "minutesPlayed": 25
      },
      {
        "id": "...",
        "name": "...",
        "totalScoreGained": 60,
        "minutesPlayed": 10
      }
    ]
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is a member of the team with given team id
- Get values from database with following:
    - join "team" table with "team_has_players" table ON team.id/team_has_players.team_id AND
      team_has_players.player_id/deserializedPlayer.id → (1)
    - join (1) with "game" table ON (1).team.id/game.team_id → (2)
    - join (2) with "player_has_achievements" ON (2).game.id/player_has_achievements.game_id AND
      player_has_achievements.player_id/deserializedPlayer.id → (3)
    - join (3) with "achievement" ON (3).player_has_achievements.achievement_id/achievement.id → (4)
- Aggregate the values to sum achievement scores, and calculate game durations (game.end_date - game.start_date)

**GET** `/teams/create`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN

- Response:

  ```json
  {
    "success": true,
    "data": {
      "teamId": "..."
    }
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN
- Add new entry to "team" table with empty team name
- Add new entry to "team_has_players" with following values:
    - team_id → "id" field of the entry added to "team" table
    - player_id → deserialized player’s id
    - is_captain → true
    - is_active → true

**POST** `/teams/update`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
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
    "success": true,
    "data": "team updated successfully"
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is the captain member of the team with
  given team id
- Update the entry with team id in "team" table
- Add new entry to "relay_server" table. Entry:
    - team_id → "id" field of the entry added to "team" table
    - ipv4 → "relayServer.ipv4" field in request body
    - port → "relayServer.port" field in request body
    - join_code → "relayServer.joinCode" field in request body

**GET** `/teams/:teamId`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": {
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
  }
  ```

**Acceptance Criteria:**

- join "team" table with "team_has_players" table ON team.id/team_has_players.team_id → (1)
- Check if the deserialized player is a member of the team from (1)
- join "player" table with (1) ON (1).team_has_players.player_id/player.id → (2)
- join "game" table with (1) ON (1).team.id/game.team_id → (3)
- join "room" table with (3) ON (3).room_id/room.id → (4)
- join "player_has_achievements" table with (3) ON (3).game.id/player_has_achievements.game_id AND
  player_has_achievements.player_id/deserializedPlayer.id → (5)
- join "achievement" table with (4) ON (4).achievement_id/achievement.id → (6)
- Aggregate the values to sum achievement scores, and calculate game durations (game.end_date - game.start_date)

**GET** `/teams/:teamId/relay-server`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": {
      "ipv4": "...",
      "port": 1234,
      "joinCode": "..."
    }
  }
  ```

**Acceptance Criteria:**

- Check if the deserialized player is a member of the team
- Get relay server information with "teamId" in URL params from "relay_server" table

**POST** `/games`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
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
    "success": true,
    "data": "game created successfully"
  }
  ```

**Acceptance Criteria:**

- Check if the team has 4 players
- Add new entry to "game" table

**GET** `/games/:gameId`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": {
        "roomName": "...",
        "minutesPlayed": 2,
        "score": 50,
        "achievements": [
          {
            "id": "...",
            "name": "...",
            "description": "...",
            "score": 10,
            "owner": "[owner player's display name]"
          }
        ]
      }
  }
  ```

**Acceptance Criteria:**

- Get player’s score via joining "player_has_achievements", "achievement", "game" tables
- Get room name via joining "game", "room" tables
- Get all achievements with player information via joining "game", "player_has_achievements", "player", "achievement"
  tables
- Get player's achievements to calculate score via joining "game", "player_has_achievements", "achievement" tables
  considering deserialized player's player id

**POST** `/games/:gameId`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
- Body:

  ```json
  {
    "teamId": "..."
  }
  ```

- Response:

  ```json
  {
    "success": true,
    "data": "game created successfully"
  }
  ```

**Acceptance Criteria:**

- Deserialize player using ACCESS_TOKEN and ID_TOKEN, and check if the player is the captain of the team with given team
  id
- Set "ended_at" to "now()" in the "game" table with "gameId" in URL params

**POST** `/achievements`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
    - Content-Type → "application/json"
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
    "success": true,
    "data": "achievement added to the player"
  }
  ```

**Acceptance Criteria:**

- Add new entry to "achievement" table. Entry:
    - player_id → deserialized player’s id
    - game_id → "gameId" in request body
    - achievement_id → "achievementId" in request body

**GET** `/scoreboard`

- Headers:
    - access_token → ACCESS_TOKEN
    - id_token → ID_TOKEN
- Response:

  ```json
  {
    "success": true,
    "data": [
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
  }
  ```

**Acceptance Criteria:**

- Get players’ scores via joining "player_has_achievements", "achievement", "player" tables, and order by their score

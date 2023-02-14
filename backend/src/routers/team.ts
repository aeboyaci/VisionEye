import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";

type PlayerInformation = {
  isMember: boolean;
  isCaptain: boolean;
}

type TeamInformation = {
  team: {
    id: string;
    name: string;
  },
  relayServer: {
    ipv4: string;
    port: number;
    joinCode: string;
  }
};

type TeamDetail = {
  name: string;
  games: {
    roomName: string;
    score: number;
    minutesPlayed: number;
    startDate: string;
  }[];
  players: {
    playerId: string;
    displayName: string;
    avatarUrl: string;
    isCaptain: boolean;
    isOnline: boolean;
  }[];
}

export async function checkIfThePlayerIsAMember(teamId: string, playerId: string): Promise<PlayerInformation> {
  const defaultValue = { isMember: false, isCaptain: false };
  const result: { values: PlayerInformation; }[] = await database.$queryRaw`
      SELECT json_build_object(
                     'isMember', CASE WHEN COUNT(*) = 1 THEN true ELSE false END,
                     'isCaptain', is_captain
                 ) as "values"
      FROM team
               INNER JOIN team_has_players ON team.id = team_has_players.team_id
      WHERE team.id = ${teamId}
        AND team_has_players.player_id = ${playerId}
      GROUP BY team.id, team_has_players.player_id, is_captain;
  `;
  return new Promise<PlayerInformation>(resolve => resolve(result.length === 0 ? defaultValue : result[0].values));
}

const router = express.Router();

router.get("/", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const result = await database.$queryRaw`
        WITH game_information AS (SELECT sum(
                                                 extract(MINUTE FROM age(ended_at, started_at))
                                             )   as minutes_played,
                                         team_id,
                                         game.id as game_id
                                  FROM game
                                  GROUP BY team_id, game.id),
             achievement_information AS (SELECT sum(score) as total_score,
                                                team_id,
                                                player_id,
                                                game_id
                                         FROM player_has_achievements
                                                  INNER JOIN team ON team.id = player_has_achievements.team_id
                                                  INNER JOIN achievement ON achievement.id = player_has_achievements.achievement_id
                                         GROUP BY team_id, player_id, game_id)
        SELECT team."name"                     as "name",
               CASE
                   WHEN achievement_information.total_score IS NULL THEN 0
                   ELSE achievement_information.total_score
                   END                         as "totalScoreGained",
               game_information.minutes_played as "minutesPlayed"
        FROM team_has_players
                 INNER JOIN team ON team.id = team_has_players.team_id
                 INNER JOIN game_information ON game_information.team_id = team_has_players.team_id
                 LEFT JOIN achievement_information ON
                    achievement_information.team_id = team_has_players.team_id
                AND
                    achievement_information.player_id = team_has_players.player_id
                AND
                    achievement_information.game_id = game_information.game_id
        WHERE team_has_players.player_id = ${player.id};
    `;

    return resp.status(200).json({
      success: true,
      data: result,
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.get("/:teamId", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { teamId } = req.params;

  try {
    // Check if the player is a member of the team
    const { isMember } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    const result: TeamDetail[] = await database.$queryRaw`
      WITH
          game_information AS (
              SELECT game.id,
                     game.team_id,
                     started_at,
                     ended_at,
                     r."name" as room_name,
                     sum(score) as score
              FROM game
                  INNER JOIN player_has_achievements pha on game.id = pha.game_id
                  INNER JOIN achievement a on a.id = pha.achievement_id
                  INNER JOIN room r on game.room_id = r.id
              WHERE player_id = ${player.id}
              GROUP BY game.id, player_id, r."name"
          ),
          player_information AS (
              SELECT team_id,
                     player_id,
                     display_name,
                     avatar_url,
                     is_captain,
                     is_online
              FROM team_has_players
                  INNER JOIN player p on p.id = team_has_players.player_id
          )
      SELECT team."name",
             json_agg(
                 distinct jsonb_build_object(
                     'roomName', game_information.room_name,
                     'minutesPlayed', extract(MINUTE FROM AGE(game_information.ended_at, game_information.started_at)),
                     'score', game_information.score,
                     'startedAt', game_information.started_at
                 )
             ) as games,
             json_agg(
                 distinct jsonb_build_object(
                     'playerId', player_information.player_id,
                     'displayName', player_information.display_name,
                     'avatarUrl', player_information.avatar_url,
                     'isCaptain', player_information.is_captain,
                     'isOnline', player_information.is_online
                 )
             ) as players
      FROM team
          INNER JOIN game_information ON game_information.team_id = team.id
          INNER JOIN player_information ON player_information.team_id = team.id
      WHERE team.id = ${teamId}
      GROUP BY team.id, team."name"`;

    return resp.status(200).json({
      success: true,
      data: result.length > 0 ? result : [],
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.get("/:teamId/relay-server", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { teamId } = req.params;

  try {
    // Check if the player is a member of the team
    const { isMember } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    const relayServer = await database.relay_server.findFirst({
      where: {
        team_id: teamId,
      },
      select: {
        ipv4: true,
        port: true,
        join_code: true,
      },
    });
    if (relayServer === null) {
      return resp.status(400).json({
        success: false,
        error: "bad request",
      });
    }

    return resp.status(200).json({
      success: true,
      data: {
        "ipv4": relayServer.ipv4,
        "port": relayServer.port,
        "joinCode": relayServer.join_code
      },
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.get("/:teamId/invitations", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { teamId } = req.params;

  try {
    // Check if the player is a member of the team
    const { isMember } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    const result = await database.$queryRaw`
        SELECT p.id              as "playerId",
               p.display_name    as "displayName",
               p.avatar_url      as "avatarUrl",
               invitation.status as status
        FROM invitation
                 INNER JOIN player p on p.id = invitation.receiver_player_id
        WHERE team_id = ${teamId};
    `;

    return resp.status(200).json({
      success: true,
      data: result,
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.post("/:teamId/invitations", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { teamId } = req.params;

  try {
    const { playerId } = req.body;

    // Check if the player is a member of the team
    const { isMember } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    await database.invitation.create({
      data: {
        sender_player_id: player.id,
        receiver_player_id: playerId,
        team_id: teamId,
      },
    });

    return resp.status(200).json({
      success: true,
      data: "player successfully invited",
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.get("/create", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    await database.$transaction(async (tx) => {
      const team = await tx.team.create({
        data: {},
      });

      await tx.team_has_players.create({
        data: {
          team_id: team.id,
          player_id: player.id,
          is_captain: true
        },
      });

      return resp.status(200).json({
        success: true,
        data: {
          teamId: team.id,
        },
      });
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.post("/update", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const body: TeamInformation = req.body;
    const { isCaptain } = await checkIfThePlayerIsAMember(body.team.id, player.id);
    if (!isCaptain) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    await database.team.update({
      where: {
        id: body.team.id,
      },
      data: {
        name: body.team.name,
        relay_server: {
          create: {
            ipv4: body.relayServer.ipv4,
            port: body.relayServer.port,
            join_code: body.relayServer.joinCode,
          },
        },
      },
    });

    return resp.status(200).json({
      success: true,
      data: "team updated successfully",
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

export default router;

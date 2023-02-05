import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";

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
        SELECT team."name" as "name",
               CASE
                   WHEN achievement_information.total_score IS NULL THEN 0
                   ELSE achievement_information.total_score
               END as "totalScoreGained",
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

router.get("/:teamId/invitations", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { teamId } = req.params;

  try {
    // Check if the player is a member of the team
    const { isMember }: { isMember: boolean } = await database.$queryRaw`
        SELECT json_build_object(
                       'isMember', WHEN COUNT(*) = 1 THEN true ELSE false
      )
        FROM team
                 INNER JOIN team_has_players ON team.id = team_has_players.team_id
        WHERE team.id = ${teamId}
          AND team_has_players.player_id = ${player.id};
    `;
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    const result = await database.$queryRaw`
        SELECT p.id as "playerId",
               p.display_name as "displayName",
               p.avatar_url as "avatarUrl",
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

    // Check if the player is the captain of the team
    const { isCaptain }: { isCaptain: boolean } = await database.$queryRaw`
        SELECT json_build_object(
                       'isCaptain', WHEN COUNT(*) = 1 THEN true ELSE false
      )
        FROM team
                 INNER JOIN team_has_players ON team.id = team_has_players.team_id
        WHERE team.id = ${teamId}
          AND team_has_players.player_id = ${player.id}
          AND team_has_players.is_captain = true;
    `;
    if (!isCaptain) {
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

export default router;

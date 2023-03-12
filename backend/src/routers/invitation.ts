import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";

const router = express.Router();

router.get("/", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const result = await database.$queryRaw`
      WITH
          team_players AS (
              SELECT team.id as team_id,
                     player.id as player_id,
                     player.display_name,
                     player.avatar_url,
                     team_has_players.is_captain
              FROM invitation
                       INNER JOIN team ON invitation.team_id = team.id
                       INNER JOIN team_has_players ON team_has_players.team_id = team.id
                       INNER JOIN player ON player.id = team_has_players.player_id
              WHERE
                  invitation.receiver_player_id = ${player.id}
                AND
                  invitation.status = 'PENDING'
          ),
          sender_player AS (
              SELECT invitation.team_id,
                     invitation.id as invitation_id,
                     player.id as player_id,
                     player.display_name,
                     player.avatar_url
              FROM invitation
                       INNER JOIN player ON
                          invitation.sender_player_id = player.id
                      AND
                          invitation.receiver_player_id = ${player.id}
          )
      SELECT sender_player.invitation_id as id,
             team.id as "teamId",
             json_build_object(
                     'playerId', sender_player.player_id,
                     'displayName', sender_player.display_name,
                     'avatarUrl', sender_player.avatar_url
                 ) as sender,
             json_agg(
                     json_build_object(
                             'playerId', team_players.player_id,
                             'displayName', team_players.display_name,
                             'avatarUrl', team_players.avatar_url,
                             'isCaptain', team_players.is_captain
                         )
                 ) as players
      FROM team
               INNER JOIN team_players ON team_players.team_id = team.id
               INNER JOIN sender_player ON sender_player.team_id = team.id
      GROUP BY team.id, sender_player.invitation_id, sender_player.player_id, sender_player.display_name, sender_player.avatar_url;
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

router.post("/", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const { id, status } = req.body;

    // Validate request body
    if (status !== "ACCEPTED" && status !== "REJECTED") {
      return resp.status(400).json({
        success: false,
        error: "invalid request body",
      });
    }

    await database.$transaction(async (tx) => {
      // Check if the invitation with "id" is sent for the player
      const invitation = await tx.invitation.findFirst({
        where: {
          id: id,
          receiver_player_id: player.id,
        },
      });
      if (invitation === null) {
        return resp.status(401).json({
          success: false,
          error: "unauthorized",
        });
      }
  
      await tx.invitation.update({
        where: {
          id: id,
        },
        data: {
          status: status,
        },
      });

      // Update all others “invitation” entries
      await tx.invitation.updateMany({
        where: {
          AND: [
            {
              status: "PENDING",
            },
            {
              receiver_player_id: player.id,
            },
          ],
        },
        data: {
          status: "REJECTED",
        },
      });
    });
  
    return resp.status(200).json({
      success: true,
      data: `invitation set to ${status}`,
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

export default router;

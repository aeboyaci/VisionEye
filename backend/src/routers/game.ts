import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";
import { checkIfThePlayerIsAMember } from "./team";

type GameInformation = {
  teamId: string;
  roomId: string;
};

type GameDetailInformation = {
  roomName: string;
  minutesPlayed: number;
  score: number;
  achievements: {
    id: string;
    name: string;
    description: string;
    score: number;
    owner: string;
  }[];
};

const router = express.Router();

router.post("/", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const { teamId, roomId }: GameInformation = req.body;

    const { isCaptain } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isCaptain) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    await database.$transaction(async (tx) => {
      // Check team has at most 4 players
      const teams = await tx.team_has_players.groupBy({
        by: ["team_id"],
        _count: {
          player_id: true,
        },
        where: {
          team_id: teamId,
        },
        having: {
          player_id: {
            _count: {
              lte: 4,
            },
          },
        },
      });
      if (teams.length === 0) {
        return resp.status(400).json({
          success: false,
          error: "number of team members exceeds 4. Invalid request sent",
        });
      }

      await tx.game.create({
        data: {
          team_id: teamId,
          room_id: roomId,
          ended_at: null,
        },
      });

      return resp.status(200).json({
        success: true,
        data: "game created successfully",
      });
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.post("/:gameId", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;
  const { gameId } = req.params;

  try {
    const { teamId }: GameInformation = req.body;

    const { isCaptain } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isCaptain) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    await database.game.update({
      where: {
        id: gameId,
      },
      data: {
        ended_at: new Date(),
      },
    });

    return resp.status(200).json({
      success: true,
      data: "game updated successfully",
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

router.get("/:gameId", enforceAuthentication, async (req, resp, next) => {
  try {
    const player = req.user!;
    const { gameId } = req.params;

    const result: { values: GameDetailInformation }[] = await database.$queryRaw`
      WITH
        score_information as (
          SELECT DISTINCT ON (player_id) game_id, sum(score) as score FROM player_has_achievements
        INNER JOIN achievement ON player_has_achievements.achievement_id = achievement.id
        INNER JOIN game ON player_has_achievements.game_id = game.id
      WHERE player_id = ${player.id}
      GROUP BY player_id, game_id
        ),
        room_information as (
      SELECT "name", game.id as game_id FROM room
        INNER JOIN game ON room.id = game.room_id
        ),
        achievement_information as (
      SELECT game.id as game_id, achievement.id, "name", description, player.display_name as owner FROM game
        INNER JOIN player_has_achievements ON game.id = player_has_achievements.game_id
        INNER JOIN player ON player_has_achievements.player_id = player.id
        INNER JOIN achievement ON player_has_achievements.achievement_id = achievement.id
        )
      SELECT DISTINCT ON (game.id) json_build_object(
        'roomName', room_information."name",
        'minutesPlayed', extract(MINUTE FROM age(ended_at, started_at)),
        'score', score_information.score,
        'achievements', json_agg(achievement_information)
        ) as "values" FROM game
        INNER JOIN score_information ON game.id = score_information.game_id
        INNER JOIN room_information ON score_information.game_id = room_information.game_id
        INNER JOIN achievement_information ON score_information.game_id = achievement_information.game_id
      WHERE game.id = ${gameId}
      GROUP BY game.id, room_information."name", ended_at, started_at, score_information.score
    `;

    return resp.status(200).json({
      success: true,
      data: result.length === 0 ? null : result[0].values,
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

export default router;

import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";
import { checkIfThePlayerIsAMember } from "./team";

type AchievementInformation = {
  achievementId: string;
  gameId: string;
  teamId: string;
};

const router = express.Router();

router.post("/", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  try {
    const { achievementId, gameId, teamId }: AchievementInformation = req.body;

    // Check if the player is a member of the team
    const { isMember } = await checkIfThePlayerIsAMember(teamId, player.id);
    if (!isMember) {
      return resp.status(401).json({
        success: false,
        error: "You are not allowed to this operation",
      });
    }

    await database.$transaction(async (tx) => {
      const record = await tx.player_has_achievements.create({
        data: {
          achievement_id: achievementId,
          game_id: gameId,
          player_id: player.id,
          team_id: teamId,
        },
        include: {
          achievement: true,
          player: {
            include: {
              scoreboard: true,
            },
          },
        },
      });
      await tx.scoreboard.update({
        where: {
          player_id: player.id,
        },
        data: {
          score: record.achievement.score + record.player.scoreboard!.score,
        },
      });

      return resp.status(200).json({
        success: true,
        data: "achievement added to the player",
      });
    });
  } catch (ex) {
    return resp.status(500).json({
      success: false,
      error: ex,
    });
  }
});

export default router;

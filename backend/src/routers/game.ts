import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";
import { checkIfThePlayerIsAMember } from "./team";

type GameInformation = {
  teamId: string;
  roomId: string;
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

export default router;

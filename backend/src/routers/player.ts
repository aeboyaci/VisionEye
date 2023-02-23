import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";

const router = express.Router();

router.get("/", enforceAuthentication, async (req, resp, next) => {
  try {
    const players = await database.player.findMany({
      where: {
        is_online: true,
      },
    });
    const result = players.map((p) => {
      return {
        playerId: p.id,
        displayName: p.display_name,
        avatarUrl: p.avatar_url,
      };
    });

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

router.get("/me", enforceAuthentication, async (req, resp, next) => {
  const player = req.user!;

  return resp.status(200).json({
    success: true,
    data: {
      displayName: player.display_name,
      avatarUrl: player.avatar_url,
    },
  });
});

export default router;

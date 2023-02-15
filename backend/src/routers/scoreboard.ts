import express from "express";
import { database } from "../prisma/database";
import { enforceAuthentication } from "./authentication";

const router = express.Router();

router.get("/", enforceAuthentication, async (req, resp, next) => {
  try {
    const result = await database.$queryRaw`
        SELECT scoreboard.player_id as "playerId",
               player.display_name as "displayName",
               player.avatar_url as "avatarUrl",
               score
        FROM scoreboard
                 INNER JOIN player ON player.id = scoreboard.player_id
        ORDER BY score DESC;
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

export default router;

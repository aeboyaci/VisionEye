import express, { Request, Response, NextFunction } from "express";
import passport from "passport";
import { database } from "../prisma/database";

export function enforceAuthentication(req: Request, resp: Response, next: NextFunction) {
  const isAuthenticated = req.isAuthenticated();

  if (!isAuthenticated) {
    return resp.status(401).json({
      success: false,
      error: "You are not logged in yet",
    });
  }

  next();
}

const router = express.Router();

router.get("/google/sign-in", passport.authenticate("google"));

router.get("/google/callback", passport.authenticate("google", {
  failureRedirect: "/api/oauth2/google/callback/failure",
}), async (req, resp, next) => {
  return resp.status(200).json({
    success: true,
    data: "login successful",
  });
});

router.get("/google/callback/failure", (req, resp, next) => {
  return resp.status(401).json({
    success: false,
    error: "login failed!"
  });
});

router.get("/google/logout", enforceAuthentication, async (req, resp, next) => {
  req.session.destroy(() => {
    database.player.update({
      where: {
        id: req.user?.id,
      },
      data: {
        is_online: false,
      },
    });

    return resp.redirect("/oauth2/google/check");
  });
});

router.get("/google/check", (req, resp, next) => {
  const isAuthenticated = req.isAuthenticated();

  if (isAuthenticated) {
    const player = req.user;

    return resp.status(200).json({
      success: true,
      data: player,
    });
  }

  return resp.status(200).json({
    success: true,
    data: "You are not logged in yet",
  });
});

export default router;

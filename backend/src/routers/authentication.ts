import express, { Request, Response, NextFunction } from "express";
import passport from "passport";
import { database } from "../prisma/database";

export function authorize(req: Request, resp: Response, next: NextFunction) {
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

router.get("/google/logout", authorize, async (req, resp, next) => {
  req.logout((err) => {
    if (Boolean(err)) {
      return resp.status(200).json({
        success: false,
        error: "logging out failed",
      });
    }

    database.playerConnectionInformation.update({
      where: {
        playerId: req.user?.id,
      },
      data: {
        status: "OFFLINE",
      },
    });

    return resp.status(200).json({
      success: true,
      data: "successfully logged out",
    });
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

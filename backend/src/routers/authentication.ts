import express, { NextFunction, Request, Response } from "express";
import { google } from "googleapis";
import { database } from "../prisma/database";
import { BASE_URL, GOOGLE_CLIENT_ID, GOOGLE_CLIENT_SECRET } from "../utils/env";
import axios from "axios";
import crypto from "crypto";

const GOOGLE_CALLBACK_URL = `${BASE_URL}/oauth2/google/callback`;
const scopes = [
  'https://www.googleapis.com/auth/userinfo.profile',
  'https://www.googleapis.com/auth/userinfo.email',
];
type GoogleUser = {
  id: string;
  email: string;
  verified_email: boolean;
  name: string;
  given_name: string;
  family_name: string;
  picture: string;
  locale: string;
};
type UserTokens = {
  accessToken: string;
  idToken: string;
} | null;

const cache: Map<string, UserTokens> = new Map<string, UserTokens>();

function generateOTP(): string {
  const dictionary = "0123456789";
  let otp = "";
  for (let i = 0; i < 6; i++) {
    let idx = crypto.randomInt(0, dictionary.length);
    otp += dictionary.at(idx);
  }

  return otp;
}

function getGoogleAuthURL() {
  return oauth2Client.generateAuthUrl({
    access_type: 'offline',
    prompt: 'consent',
    scope: scopes,
  });
}

async function getGoogleUser(idToken: string, accessToken: string): Promise<GoogleUser> {
  // Fetch the user's profile with the access token and bearer
  return await axios
    .get(
      `https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token=${accessToken}`,
      {
        headers: {
          Authorization: `Bearer ${idToken}`,
        },
      },
    )
    .then(res => res.data)
    .catch(error => {
      throw new Error(error.message);
    });
}

export async function enforceAuthentication(req: Request, resp: Response, next: NextFunction) {
  try {
    const idToken = (req.headers["id_token"] || "") as string;
    const accessToken = (req.headers["access_token"] || "") as string;

    const googleUser = await getGoogleUser(idToken, accessToken);
    const player = await database.player.findFirst({
      where: {
        email: googleUser.email,
      },
    });
    if (player === null) {
      return resp.status(401).json({
        success: false,
        error: "You are not logged in yet",
      });
    }
    req.user = player;

    next();
  } catch (ex) {
    return resp.status(401).json({
      success: false,
      error: "You are not logged in yet",
    });
  }
}

const router = express.Router();
const oauth2Client = new google.auth.OAuth2(
  GOOGLE_CLIENT_ID,
  GOOGLE_CLIENT_SECRET,
  GOOGLE_CALLBACK_URL,
);

router.get("/google/passcode", (req, resp, next) => {
  const otp = generateOTP();
  cache.set(otp, null);
  return resp.status(200).json({
    success: true,
    data: otp,
  });
});

router.get("/google/passcode/:code", (req, resp, next) => {
  const otp = req.params.code;
  if (!cache.has(otp)) {
    // TODO redirect to Authentication status webpage (Task-17.1)
    return resp.status(400).json({
      success: false,
      data: "invalid OTP code",
    });
  }
  const googleAuthURL = getGoogleAuthURL();
  req.session.otp = otp;
  return resp.redirect(googleAuthURL);
});

router.get("/google/callback", async (req, resp, next) => {
  try {
    const code = (req.query.code) as string;
    const { tokens } = await oauth2Client.getToken(code);

    const googleUser = await getGoogleUser(tokens.id_token as string, tokens.access_token as string);
    const player = await database.player.findFirst({
      where: {
        email: googleUser.email,
      },
    });
    if (player === null) {
      const newUser = {
        display_name: googleUser.name,
        email: googleUser.email,
        avatar_url: googleUser.picture,
      };
      await database.player.create({
        data: {
          is_online: true,
          ...newUser,
          scoreboard: {
            create: {
              score: 0,
            },
          },
        },
      });
    }
    else {
      await database.player.update({
        where: {
          id: player.id,
        },
        data: {
          is_online: true,
        },
      });
    }

    const otp: string = req.session.otp ?? "";
    if (!cache.has(otp)) {
      return resp.redirect("/oauth2/google/callback/failure");
    }
    cache.set(otp, {
      accessToken: tokens.access_token ?? "",
      idToken: tokens.id_token ?? "",
    });

    // TODO redirect to Authentication status webpage (Task-17.1)
    return resp.status(200).json({
      success: true,
      data: {
        accessToken: tokens.access_token,
        idToken: tokens.id_token,
      },
    });
  } catch (ex) {
    return resp.redirect("/oauth2/google/callback/failure");
  }
});

router.get("/google/passcode/:code/status", (req, resp, next) => {
  const otp = req.params.code;
  if (!cache.has(otp)) {
    return resp.status(400).json({
      success: false,
      error: "invalid otp-code",
    });
  }

  const tokens = cache.get(otp);
  return resp.status(200).json({
    success: true,
    tokens: tokens,
  });
});

router.get("/google/callback/failure", (req, resp, next) => {
  // TODO redirect to Authentication status webpage (Task-17.1)
  return resp.status(401).json({
    success: false,
    error: "login failed!"
  });
});

router.get("/google/logout", enforceAuthentication, async (req, resp, next) => {
  const otp = req.session.otp ?? "";
  cache.delete(otp);

  req.session.destroy(async (err) => {
    if (Boolean(err)) {
      return resp.status(500).json({
        success: false,
        error: err.error,
      });
    }
    const player = req.user!;

    await database.player.update({
      where: {
        id: player.id,
      },
      data: {
        is_online: false,
      },
    });
    return resp.redirect("/");
  });
});

export default router;

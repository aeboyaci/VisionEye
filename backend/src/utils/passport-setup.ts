import passport from "passport";
import passportGoogle from "passport-google-oauth20";
import { BASE_URL, GOOGLE_CLIENT_ID, GOOGLE_CLIENT_SECRET } from "./env";
import { database } from "../prisma/database";

function setup() {
  const GoogleStrategy = passportGoogle.Strategy;

  passport.serializeUser((user, done) => {
    done(null, user.id);
  });

  passport.deserializeUser(async (userId: string, done) => {
    let dbUser = await database.player.findFirst({
      where: {
        id: userId,
      },
    });

    done(null, dbUser);
  });

  passport.use(
    new GoogleStrategy({
      clientID: GOOGLE_CLIENT_ID,
      clientSecret: GOOGLE_CLIENT_SECRET,
      callbackURL: `${BASE_URL}/oauth2/google/callback`,
      scope: ["email", "profile"],
      passReqToCallback: true,
    }, async (request, accessToken, refreshToken, profile, done) => {
      const playerEmail = (profile.emails?.[0].value || "") as string;
      const playerImage = (profile.photos?.[0].value || "") as string;

      let dbUser = await database.player.findFirst({
        where: {
          email: playerEmail,
        },
      });

      if (dbUser !== null) {
        await database.playerConnectionInformation.update({
          where: {
            playerId: dbUser.id,
          },
          data: {
            status: "ONLINE",
            ipAddress: request.ip,
          },
        });

        done(null, dbUser);
        return;
      }

      let newUser = {
        displayName: profile.displayName,
        email: playerEmail,
        avatarUrl: playerImage,
      };
      await database.$transaction(async (tx) => {
        const createdUser = await tx.player.create({
          data: { ...newUser }
        });

        await tx.playerConnectionInformation.create({
          data: {
            ipAddress: request.ip,
            player: {
              connect: { id: createdUser.id },
            },
          },
          include: {
            player: true,
          },
        });

        done(null, createdUser);
      });
    })
  );
}

export default setup;

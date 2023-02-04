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
        await database.player.update({
          where: {
            id: dbUser.id,
          },
          data: {
            is_online: true,
          },
        });

        done(null, dbUser);
        return;
      }

      let newUser = {
        display_name: profile.displayName,
        email: playerEmail,
        avatar_url: playerImage,
      };
      const createdUser = await database.player.create({
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
      done(null, createdUser);
    })
  );
}

export default setup;

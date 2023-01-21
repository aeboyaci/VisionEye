const passport = require("passport");
const GoogleStrategy = require("passport-google-oauth20");

passport.serializeUser((user , done) => {
  done(null , user);
});

passport.deserializeUser((user, done) => {
  done(null, user);
});

passport.use(
  new GoogleStrategy({
    clientID: process.env.GOOGLE_CLIENT_ID,
    clientSecret: process.env.GOOGLE_CLIENT_SECRET,
    callbackURL: "http://localhost:8080/api/oauth2/google/callback",
    scope: ["email", "profile"],
  }, (accessToken, refreshToken, profile, done) => {
    // Check if the user exists in the database, if not create a new user entry
    
    done(null, profile);
  })
);

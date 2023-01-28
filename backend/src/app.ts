import { BASE_URL, COOKIE_SECRET, PORT } from "./utils/env";
import express from "express";
import passport from "passport";
import session from "express-session";
import setupPassport from "./utils/passport-setup";

import authenticationRouter from "./routers/authentication";

const app = express();
app.set("trust proxy", true)
setupPassport();

app.use(session({
  secret: COOKIE_SECRET,
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }
}));
app.use(passport.initialize());
app.use(passport.session());

app.use("/oauth2", authenticationRouter);

app.listen(PORT, () => {
  console.log(`[INFO] Server started!\n${BASE_URL}/`);
});

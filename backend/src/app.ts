import { BASE_URL, COOKIE_SECRET, PORT } from "./utils/env";
import express from "express";
import passport from "passport";
import session from "express-session";
import setupPassport from "./utils/passport-setup";

import authenticationRouter from "./routers/authentication";
import invitationRouter from "./routers/invitation";
import scoreboardRouter from "./routers/scoreboard";

(BigInt.prototype as any).toJSON = function () {
  return parseInt(this.toString());
};

const app = express();
app.set("trust proxy", true)

app.use(session({
  secret: COOKIE_SECRET,
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }
}));
app.use(passport.initialize());
app.use(passport.session());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
setupPassport();

app.use("/oauth2", authenticationRouter);
app.use("/invitations", invitationRouter);
app.use("/scoreboard", scoreboardRouter);

app.listen(PORT, () => {
  console.log(`[INFO] Server started!\n${BASE_URL}/`);
});

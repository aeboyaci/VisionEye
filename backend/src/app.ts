import { BASE_URL, COOKIE_SECRET, PORT } from "./utils/env";
import express from "express";
import session from "express-session";

import authenticationRouter from "./routers/authentication";
import invitationRouter from "./routers/invitation";
import scoreboardRouter from "./routers/scoreboard";
import teamRouter from "./routers/team";
import achievementRouter from "./routers/achievement";
import gameRouter from "./routers/game";

(BigInt.prototype as any).toJSON = function () {
  return parseInt(this.toString());
};

const app = express();
app.set("trust proxy", true);
app.set("view engine", "hbs");

app.use(express.static(__dirname + "/public"));

app.use(session({
  secret: COOKIE_SECRET,
  resave: false,
  saveUninitialized: true,
  cookie: { secure: false }
}));
app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.use("/oauth2", authenticationRouter);
app.use("/invitations", invitationRouter);
app.use("/scoreboard", scoreboardRouter);
app.use("/teams", teamRouter);
app.use("/achievements", achievementRouter);
app.use("/games", gameRouter);

app.listen(PORT, () => {
  console.log(`[INFO] Server started!\n${BASE_URL}/`);
});

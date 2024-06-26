import { BASE_URL, COOKIE_SECRET, PORT } from "./utils/env";
import express from "express";
import session from "express-session";

import authenticationRouter from "./routers/authentication";
import invitationRouter from "./routers/invitation";
import scoreboardRouter from "./routers/scoreboard";
import teamRouter from "./routers/team";
import achievementRouter from "./routers/achievement";
import gameRouter from "./routers/game";
import playerRouter from "./routers/player";
import viewsRouter from "./routers/views";

(BigInt.prototype as any).toJSON = function () {
  return parseInt(this.toString());
};

const app = express();
app.set("trust proxy", true);
app.set("view engine", "hbs");

if (process.env.NODE_ENVIRONMENT !== "PRODUCTION") {
  app.use(express.static(__dirname + "/public"));
}
else {
  app.use(express.static(__dirname + "/../public"));
}

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
app.use("/players", playerRouter);
app.use("/", viewsRouter);

app.listen(PORT, () => {
  console.log(`[INFO] Server started!\n${BASE_URL}/`);
});

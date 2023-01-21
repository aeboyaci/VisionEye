// imports
const express = require("express");
const authRouter = require("./routers/auth");
const passport = require("passport");
const session = require("express-session");
require('dotenv').config();
require("./utils/setup-passport");

// static variables
const PORT = 8080;

const app = express();
app.set("view engine", "hbs");

app.use(session({
   secret: process.env.COOKIE_SECRET,
   resave: false,
   saveUninitialized: true,
   cookie: { secure: false }
}));
app.use(passport.initialize());
app.use(passport.session());

// register routers
app.use("/api/oauth2", authRouter);

// home page
app.get("/", (req, resp, next) => {
  return resp.render("index", {
    isLoggedIn: req.isAuthenticated(),
    user: req.user,
  });
});

app.listen(PORT, () => {
  console.log(`[INFO] Server started!\nAvailable on: http://localhost:${PORT}/`);
});

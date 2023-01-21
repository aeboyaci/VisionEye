const router = require("express").Router();
const passport = require("passport");

router.get("/google/sign-in", passport.authenticate("google"));

router.get("/google/callback", passport.authenticate("google", {
  failureRedirect: "/api/oauth2/google/callback/failure",
}), (req, resp, next) => {
  return resp.redirect("/");
});

router.get("/google/callback/failure", (req, resp, next) => {
  return resp.status(401).json({
    success: false,
    error: "unauthorized! login failed",
  });
});

router.get("/google/sign-out", (req, resp, next) => {
  return resp.json({
    success: true,
  });
});

module.exports = router;

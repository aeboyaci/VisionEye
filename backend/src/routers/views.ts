import express from "express";

const router = express.Router();

router.get("/", (req, resp, next) => {
  return resp.render("index");
});

router.get("/status", (req, resp, next) => {
  const status = req.session.authenticationStatus;
  if (!Boolean(status)) {
    return resp.redirect("/");
  }

  return resp.render("status", {
    success: status?.success,
    message: status?.message,
  });
});

export default router;

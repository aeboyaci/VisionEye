declare global {
  module 'express-session' {
    interface SessionData {
      otp: string;
    }
  }
}

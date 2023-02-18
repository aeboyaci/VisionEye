declare global {
  module 'express-session' {
    interface SessionData {
      otp: string;
      authenticationStatus: {
        success: boolean;
        message: string;
      };
    }
  }
}

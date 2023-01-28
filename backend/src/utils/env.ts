import dotenv from "dotenv";
import fs from "fs";

// checking if .env file is available
if (fs.existsSync(".env")) {
  dotenv.config({ path: ".env" });
} else {
  console.error(".env file not found.");
  process.exit(1);
}

export const PORT = 8080;
export const BASE_URL = (process.env.BASE_URL || `http://localhost:${PORT}`) as string;
export const COOKIE_SECRET = (process.env.COOKIE_SECRET || "COOKIE_SECRET") as string;
export const GOOGLE_CLIENT_ID = (process.env.GOOGLE_CLIENT_ID || "") as string;
export const GOOGLE_CLIENT_SECRET = (process.env.GOOGLE_CLIENT_SECRET || "") as string;

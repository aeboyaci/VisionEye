import { Player } from "../../models/player";

declare global {
  namespace Express {
    interface Request {
      user: Player
    }
  }
}

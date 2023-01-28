import { Player } from "../../models/player";

declare global {
  namespace Express {
    interface User extends Player {}
  }
}

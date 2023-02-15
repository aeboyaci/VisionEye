-- DropIndex
DROP INDEX "game_team_id_key";

-- AlterTable
ALTER TABLE "config" ADD COLUMN     "details" JSONB;

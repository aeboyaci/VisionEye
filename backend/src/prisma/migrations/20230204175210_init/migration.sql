/*
  Warnings:

  - The primary key for the `player_has_achievements` table will be changed. If it partially fails, the table could be left without primary key constraint.
  - Added the required column `achievement_id` to the `player_has_achievements` table without a default value. This is not possible if the table is not empty.

*/
-- AlterTable
ALTER TABLE "player_has_achievements" DROP CONSTRAINT "player_has_achievements_pkey",
ADD COLUMN     "achievement_id" TEXT NOT NULL,
ADD CONSTRAINT "player_has_achievements_pkey" PRIMARY KEY ("player_id", "game_id", "team_id", "achievement_id");

-- AddForeignKey
ALTER TABLE "player_has_achievements" ADD CONSTRAINT "player_has_achievements_achievement_id_fkey" FOREIGN KEY ("achievement_id") REFERENCES "achievement"("id") ON DELETE CASCADE ON UPDATE CASCADE;

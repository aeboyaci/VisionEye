/*
  Warnings:

  - You are about to drop the `relay_server` table. If the table is not empty, all the data it contains will be lost.

*/
-- DropForeignKey
ALTER TABLE "relay_server" DROP CONSTRAINT "relay_server_team_id_fkey";

-- DropTable
DROP TABLE "relay_server";

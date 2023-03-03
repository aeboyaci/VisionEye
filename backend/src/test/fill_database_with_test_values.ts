import { database } from "../prisma/database";
import { achievement, game, player, room, team } from "@prisma/client";

let players: player[];
let mainTeam: team | null;
let mainRoom: room | null;
let mainGame: game | null;
let achievements: achievement[];

async function fillDatabaseWithTestingValues(): Promise<void> {
  await addPlayer();
  await addTeam();
  await addTeamHasPlayers();
  await addInvitation();
  await addRoom();
  await addGame();
  await addAchievement();
  await addPlayerHasAchievements();
  await addScoreboard();
}

async function addPlayer(): Promise<void> {
  console.log("[DEBUG] player");

  await database.player.createMany({
    data: [
      {
        email: "scaryverse.visioneye@gmail.com",
        display_name: "Vision Eye",
        avatar_url: "https://lh3.googleusercontent.com/ogw/AAEL6sjpBYBoi-le0Gv_br_Ia0R3Z2VX-HYCHaXvJj4h=s64-c-mo",
        is_online: false,
      },
      {
        email: "testuser+01@visioneye.local",
        display_name: "Test Player - 1",
        avatar_url: "https://lh3.googleusercontent.com/a/AEdFTp4JU0RRYp_brGeMZgEIG3rNFknstRhNvoKIZ_-O=s96-c",
        is_online: false,
      },
      {
        email: "testuser+02@visioneye.local",
        display_name: "Test Player - 2",
        avatar_url: "https://lh3.googleusercontent.com/a/AEdFTp4JU0RRYp_brGeMZgEIG3rNFknstRhNvoKIZ_-O=s96-c",
        is_online: false,
      },
      {
        email: "testuser+03@visioneye.local",
        display_name: "Test Player - 3",
        avatar_url: "https://lh3.googleusercontent.com/a/AEdFTp4JU0RRYp_brGeMZgEIG3rNFknstRhNvoKIZ_-O=s96-c",
        is_online: false,
      }
    ],
  });

  players = await database.player.findMany({
    orderBy: {
      email: "asc",
    },
  });

  console.log("[INFO] player");
  return new Promise((resolve) => resolve());
}

async function addTeam(): Promise<void> {
  console.log("[DEBUG] team");

  mainTeam = await database.team.create({
    data: {
      name: "Test Team"
    },
  });

  console.log("[INFO] team");
  return new Promise((resolve) => resolve());
}

async function addTeamHasPlayers(): Promise<void> {
  console.log("[DEBUG] team_has_players");

  const team = await database.team.findFirst({
    where: {
      name: "Test Team",
    },
  });
  const teamPlayers = [
    {
      player_id: players[0].id,
      team_id: team!.id,
      is_captain: true,
    }
  ];
  for (let i = 1; i < players.length; i++) {
    teamPlayers.push({
      player_id: players[i].id,
      team_id: team!.id,
      is_captain: false,
    });
  }
  await database.team_has_players.createMany({
    data: teamPlayers,
  });

  console.log("[INFO] team_has_players");
  return new Promise((resolve) => resolve());
}

async function addInvitation(): Promise<void> {
  console.log("[DEBUG] invitation");

  const invitations = [];
  for (let i = 1; i < players.length; i++) {
    invitations.push({
      team_id: mainTeam!.id,
      sender_player_id: players[i - 1].id,
      receiver_player_id: players[i].id,
    });
  }

  await database.invitation.createMany({
    data: invitations,
  });

  console.log("[INFO] invitation");
  return new Promise((resolve) => resolve());
}

async function addRoom(): Promise<void> {
  console.log("[DEBUG] room");

  mainRoom = await database.room.create({
    data: {
      name: "Test Room",
      story: "Background story",
      image_url: "EMPTY",
    },
  });

  console.log("[INFO] room");
  return new Promise((resolve) => resolve());
}

async function addGame(): Promise<void> {
  console.log("[DEBUG] game");

  const endedAt = new Date();
  endedAt.setMinutes(endedAt.getMinutes() + 5);

  mainGame = await database.game.create({
    data: {
      team_id: mainTeam!.id,
      ended_at: endedAt,
      room_id: mainRoom!.id,
    },
  });

  console.log("[INFO] game");
  return new Promise((resolve) => resolve());
}

async function addAchievement(): Promise<void> {
  console.log("[DEBUG] achievement");

  await database.achievement.createMany({
    data: [
      {
        name: "Test Achievement - 1",
        description: "Test",
        image_url: "EMPTY",
        score: 50,
      },
      {
        name: "Test Achievement - 2",
        description: "Test",
        image_url: "EMPTY",
        score: 35,
      },
      {
        name: "Test Achievement - 3",
        description: "Test",
        image_url: "EMPTY",
        score: 25,
      },
    ],
  });
  achievements = await database.achievement.findMany();

  console.log("[INFO] achievement");
  return new Promise((resolve) => resolve());
}

async function addPlayerHasAchievements(): Promise<void> {
  console.log("[DEBUG] player_has_achievements");

  await database.player_has_achievements.createMany({
    data: [
      {
        player_id: players[0].id,
        team_id: mainTeam!.id,
        game_id: mainGame!.id,
        achievement_id: achievements[0].id,
      },
      {
        player_id: players[0].id,
        team_id: mainTeam!.id,
        game_id: mainGame!.id,
        achievement_id: achievements[1].id,
      },
      {
        player_id: players[0].id,
        team_id: mainTeam!.id,
        game_id: mainGame!.id,
        achievement_id: achievements[2].id,
      },
    ],
  });

  console.log("[INFO] player_has_achievements");
  return new Promise((resolve) => resolve());
}

async function addScoreboard(): Promise<void> {
  console.log("[DEBUG] player_has_achievements");

  for (let i = 0; i < players.length; i++) {
    await database.scoreboard.create({
      data: {
        player_id: players[i].id,
        score: 0,
      },
    });
  }

  console.log("[INFO] player_has_achievements");
  return new Promise((resolve) => resolve());
}


fillDatabaseWithTestingValues();

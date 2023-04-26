using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementBody
{
    [JsonProperty("achievementId")]
    public string achievementId;

    [JsonProperty("gameId")]
    public string gameId;

    [JsonProperty("teamId")]
    public string teamId;
}
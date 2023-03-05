using Newtonsoft.Json;
using System.Collections.Generic;

class Team
{

    [JsonProperty("id")]
    public string id;

    [JsonProperty("name")]
    public string name;
    [JsonProperty("players")]
    public List<Player> players;

    /*[JsonProperty("games")]
    public List<Game> games;
    */


}
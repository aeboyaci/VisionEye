using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class Question
{
    public string category { get; set; }
    public string id { get; set; }
    public string correctAnswer { get; set; }
    public string[] incorrectAnswers { get; set; }
    public string question { get; set; }
    public string[] tags { get; set; }
    public string type { get; set; }
    public string difficulty { get; set; }
    public string[] regions { get; set; }
    public bool isNiche { get; set; }
}

public class PublicAPIClient
{
    public static async Task<Question[]> FetchQuiz()
    {
        using var client = new HttpClient();
        var response = await client.GetAsync("https://the-trivia-api.com/api/questions");
        var json = await response.Content.ReadAsStringAsync();
        var questions = JsonConvert.DeserializeObject<Question[]>(json);
        return questions;
    }
}

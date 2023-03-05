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

public class Program
{
    public static async Task Main()
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("x-api-key", "YOUR_API_KEY"); // Replace YOUR_API_KEY with your actual API key

        var response = await client.GetAsync("https://the-trivia-api.com/api/questions");
        var json = await response.Content.ReadAsStringAsync();
        var questions = JsonConvert.DeserializeObject<Question[]>(json);

        foreach (var question in questions)
        {
            Console.WriteLine($"Category: {question.category}");
            Console.WriteLine($"Question: {question.question}");
            Console.WriteLine($"Correct answer: {question.correctAnswer}");
            Console.WriteLine($"Incorrect answers: {string.Join(", ", question.incorrectAnswers)}");
            Console.WriteLine();
        }
    }
}

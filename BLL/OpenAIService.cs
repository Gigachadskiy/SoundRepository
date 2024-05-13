using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "sk-proj-MG6vhNJEVmdsxhkPspsrT3BlbkFJrLDRtRo8R4o5dKRX2f64";

        public OpenAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetResponseFromAI(string prompt)
        {
            var data = new
            {
                model = "text-davinci-003", 
                prompt = prompt,
                max_tokens = 100
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/engines/davinci/completions", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

            return result?.Choices[0].Text ?? string.Empty;
        }
    }

    public class OpenAIResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public string Text { get; set; }
    }
}

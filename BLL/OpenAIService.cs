using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "sk-proj-L7NflgdwxFNXBcZsgs9XT3BlbkFJLgZmc7tg0ZxAAV8qtjPB";

        public OpenAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<string> GetResponseFromAI(string prompt)
        {
            var data = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                max_tokens = 100
            };

            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                // Логирование полного содержимого ответа для отладки
                Console.WriteLine("Response from OpenAI API: " + responseString);

                var result = JsonSerializer.Deserialize<OpenAIResponse>(responseString);

                if (result?.Choices != null && result.Choices.Length > 0 && result.Choices[0].Message != null)
                {
                    return result.Choices[0].Message.Content;
                }

                // Логирование ошибки
                Console.Error.WriteLine("Invalid response structure from OpenAI API: " + responseString);
            }
            catch (Exception ex)
            {
                // Логирование исключения
                Console.Error.WriteLine("Exception occurred while calling OpenAI API: " + ex.Message);
            }

            return string.Empty;
        }
    }

    public class OpenAIResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}


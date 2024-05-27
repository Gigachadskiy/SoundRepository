using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Models;
using System.Collections.Generic;

namespace BLL
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly MusicFinderService _musicFinderService;

        public OpenAIService(HttpClient httpClient, string apiKey, MusicFinderService musicFinderService)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _musicFinderService = musicFinderService;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<(string question, string answer)> GetResponseFromAI(string prompt)
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

                // Использование JsonDocument для парсинга ответа
                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;

                    var choices = root.GetProperty("choices");
                    if (choices.GetArrayLength() > 0)
                    {
                        var message = choices[0].GetProperty("message");
                        var contentProperty = message.GetProperty("content");

                        return (prompt, contentProperty.GetString().Trim());
                    }
                }

                // Логирование ошибки
                Console.Error.WriteLine("Invalid response structure from OpenAI API: " + responseString);
            }
            catch (Exception ex)
            {
                // Логирование исключения
                Console.Error.WriteLine("Exception occurred while calling OpenAI API: " + ex.Message);
            }

            return (prompt, string.Empty);
        }

        // Метод для поиска музыки в базе данных
        public List<Music> FindMusicInDatabase(MusicFinderDTO finder)
        {
            return _musicFinderService.FindMusic(finder);
        }

        // Метод для поиска музыки с помощью OpenAI
        public async Task<string> FindMusicUsingOpenAI(string prompt)
        {
            var (_, answer) = await GetResponseFromAI(prompt);
            return answer;
        }
    }
}

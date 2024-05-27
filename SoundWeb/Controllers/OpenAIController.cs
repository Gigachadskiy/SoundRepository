using BLL.DTO;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.Models;

namespace SoundWeb.Controllers
{
    public class OpenAIController : Controller
    {
        private readonly OpenAIService _openAIService;

        public OpenAIController(OpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public IActionResult Find()
        {
            OpenAIDTO finder = new OpenAIDTO();
            return View(finder);
        }

        public IActionResult NotFound()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find([Bind("Question")] OpenAIDTO finder)
        {
            // Временно отключить проверку валидации
            // if (ModelState.IsValid)
            {
                var (question, answer) = await _openAIService.GetResponseFromAI(finder.Question);
                finder.Question = question;
                finder.Answer = answer;

                if (!string.IsNullOrEmpty(finder.Answer))
                {
                    TempData["OpenAiAnswer"] = JsonConvert.SerializeObject(finder);
                    return RedirectToAction("ShowFoundOpenAIAnswer");
                }
                else
                {
                    return RedirectToAction("NotFound");
                }
            }

            return View(finder);
        }

        public IActionResult ShowFoundOpenAIAnswer()
        {
            OpenAIDTO finder = new OpenAIDTO();
            if (TempData["OpenAiAnswer"] != null)
            {
                finder = JsonConvert.DeserializeObject<OpenAIDTO>(TempData["OpenAiAnswer"].ToString());
            }
            return View(finder);
        }

        // Новый метод для поиска музыки в базе данных
        public IActionResult FindMusicInDatabase()
        {
            MusicFinderDTO finder = new MusicFinderDTO();
            return View(finder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FindMusicInDatabase([Bind("Name,Author,Tag,Genre")] MusicFinderDTO finder)
        {
            List<Music> results = _openAIService.FindMusicInDatabase(finder);
            ViewBag.Results = results;
            return View(finder);
        }

        // Новый метод для поиска музыки с использованием OpenAI
        public IActionResult FindMusicUsingOpenAI()
        {
            OpenAIDTO finder = new OpenAIDTO();
            return View(finder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FindMusicUsingOpenAI([Bind("Question")] OpenAIDTO finder)
        {
            finder.Answer = await _openAIService.FindMusicUsingOpenAI(finder.Question);
            ViewBag.Answer = finder.Answer;
            return View(finder);
        }
    }
}

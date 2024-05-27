using BLL.DTO;
using BLL;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
            finder.Answer = await _openAIService.GetResponseFromAI(finder.Question);

            TempData["OpenAiAnswer"] = JsonConvert.SerializeObject(finder);
            return RedirectToAction("ShowFoundOpenAIAnswer");
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
    }
}

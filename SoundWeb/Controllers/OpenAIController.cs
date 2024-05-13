using BLL.DTO;
using BLL;
using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace SoundWeb.Controllers
{
    public class OpenAIController : Controller
    {
        OpenAIService _openAIService;

        public OpenAIController(OpenAIService openAIService)
        
        {
            _openAIService = openAIService;
        }

        public async Task<IActionResult> Find()
        {
            OpenAIDTO finder = new OpenAIDTO();


            return View(finder);
        }


        public async Task<IActionResult> NotFound()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find([Bind("Question")] OpenAIDTO finder)
        {
            finder.Answer = await _openAIService.GetResponseFromAI(finder.Question);
            
            TempData["OpenAiAnswer"] = JsonConvert.SerializeObject(finder.Answer);
               return RedirectToAction("ShowFoundOpenAIAnswer");

            //return RedirectToAction("NotFound");
        }


        public async Task<IActionResult> ShowFoundOpenAIAnswer()
        {

            OpenAIDTO finder = new OpenAIDTO();
            if (TempData["OpenAiAnswer"] != null)
            {
                finder.Answer = JsonConvert.DeserializeObject<String>(TempData["OpenAiAnswer"].ToString());
            }

            return View(finder);
        }
    }
}

using DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BLL;
using DAL.Models;
using BLL.DTO;
using Newtonsoft.Json;

namespace SoundWeb.Controllers
{
    public class MusicFinderController : Controller
    {
        MusicFinderService _musicFinderService;

        public MusicFinderController(MusicFinderService musicFinderService)
        {
            _musicFinderService = musicFinderService;
        }

        public async Task<IActionResult> Find()
        {
            MusicFinderDTO finder = new MusicFinderDTO();


            return View(finder);
        }


        public async Task<IActionResult> NotFound()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Find([Bind("Name,Author,Tag,Genre")] MusicFinderDTO finder)
        {
            List<Music> result = _musicFinderService.FindMusic(finder);
            if (result.Count > 0)
            {
                TempData["MusicResults"] = JsonConvert.SerializeObject(result);
                return RedirectToAction("ShowFoundMusicList");
            }



            return RedirectToAction("NotFound");
        }


        public async Task<IActionResult> ShowFoundMusicList()
        {
            List<Music> musicList = new List<Music>();

            if (TempData["MusicResults"] != null)
            {
                musicList = JsonConvert.DeserializeObject<List<Music>>(TempData["MusicResults"].ToString());
            }

            return View(musicList);
        }
    }
}

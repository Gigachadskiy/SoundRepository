using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using BLL;
using Microsoft.SqlServer.Server;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SoundWeb.Controllers
{
    public class MusicsController : Controller
    {
        private readonly SoundContext _context;
        MediaService _mediaService;

        public MusicsController(SoundContext context, MediaService mediaService)
        {
            _context = context;
            _mediaService = mediaService;   
        }

        // GET: Musics
        public async Task<IActionResult> Index()
        {
            return View(await _context.Musics.ToListAsync());
        }

        // GET: Musics/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetInt32("MusicId", (int)id);
            var media = await _context.Media.FirstOrDefaultAsync(m => m.MusicId == id);
            ViewBag.Media = media;

            return View(music);
        }

        // GET: Musics/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Musics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Music music)
        {
            if (ModelState.IsValid)
            {
                _context.Add(music);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(music);
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile uploadedFile)
        {

            int musicId = (int)HttpContext.Session.GetInt32("MusicId");

            if (uploadedFile != null)
            {
                await _mediaService.AddMusicFileAsync(musicId, uploadedFile, "mp3");  
                return View("Success");
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            int musicId = (int)HttpContext.Session.GetInt32("MusicId");

            if (imageFile == null || imageFile.Length == 0)
            {
                return View("Error");
            }
            await _mediaService.UploadPictureAsync(musicId, imageFile);
            return View("Success");

        }

        public async Task<IActionResult> GetMusicFile(int id)
        {
            var musicFile = await _context.Media.Where(m=>m.MusicId == id).FirstOrDefaultAsync();
            if (musicFile == null)
            {
                return NotFound();
            }
            return File(musicFile.Data, "audio/mpeg");
        }


        // GET: Musics/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics.FindAsync(id);
            if (music == null)
            {
                return NotFound();
            }
            return View(music);
        }

        // POST: Musics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Music music)
        {
            if (id != music.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(music);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusicExists(music.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(music);
        }

        // GET: Musics/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var music = await _context.Musics
                .FirstOrDefaultAsync(m => m.Id == id);
            if (music == null)
            {
                return NotFound();
            }

            return View(music);
        }

        // POST: Musics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var music = await _context.Musics.FindAsync(id);
            if (music != null)
            {
                _context.Musics.Remove(music);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusicExists(int id)
        {
            return _context.Musics.Any(e => e.Id == id);
        }
    }
}

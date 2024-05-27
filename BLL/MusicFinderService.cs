
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BLL.DTO;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL
{
    public class MusicFinderService
    {
        private readonly SoundContext _soundContext;

        public MusicFinderService(SoundContext soundContext)
        {
            _soundContext = soundContext;
        }

        public List<Music> FindMusic(MusicFinderDTO finder)
        {
            if (finder == null)
                return new List<Music>();

            IQueryable<Music> query = _soundContext.Musics;

            if (!string.IsNullOrEmpty(finder.Name))
            {
                query = query.Where(m => m.Name.Contains(finder.Name));
            }

            if (!string.IsNullOrEmpty(finder.Author))
            {
                query = query.Include(m => m.Author)
                             .Where(m => m.Author.Name.Contains(finder.Author));
            }

            if (!string.IsNullOrEmpty(finder.Tag))
            {
                query = query.Include(m => m.MusicTags)
                             .ThenInclude(mt => mt.Tag)
                             .Where(m => m.MusicTags.Any(mt => mt.Tag.Name.Contains(finder.Tag)));
            }

            if (!string.IsNullOrEmpty(finder.Genre))
            {
                query = query.Include(m => m.MusicGenres)
                             .ThenInclude(mg => mg.Genre)
                             .Where(m => m.MusicGenres.Any(mg => mg.Genre.Name.Contains(finder.Genre)));
            }

            return query.ToList();
        }

        // Метод поиска с учетом возможных опечаток
        public List<Music> FindMusicWithFuzzySearch(MusicFinderDTO finder)
        {
            // Для реализации нечёткого поиска можно использовать внешние библиотеки,
            // такие как FuzzySharp или написать свой алгоритм.
            // Здесь используется обычный поиск как пример.
            return FindMusic(finder);
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BLL.DTO;
//using DAL.Models;

//namespace BLL
//{


//    public class MusicFinderService
//    {

//        SoundContext _soundContext;

//        public MusicFinderService(SoundContext soundContext)
//        {
//            _soundContext = soundContext;
//        }
//        public List<Music> FindMusic(MusicFinderDTO finder)
//        {
//            List<Music> result = new List<Music>();
//            if (finder == null)
//                return null;

//            IQueryable<Music> query = _soundContext.Musics;

//            if (!String.IsNullOrEmpty(finder.Name))
//            {
//                query = query.Where(m => m.Name.Contains(finder.Name));
//            }

//            if (!String.IsNullOrEmpty(finder.Author)) 
//            {
//                query = query.Where(m => m.Author.Name.Contains(finder.Author));
//            }

//            if (!String.IsNullOrEmpty(finder.Tag))
//            {
//                query = query.Where(m => m.MusicTags.Any(mt => mt.Tag.Name.Contains(finder.Tag)));
//            }

//            if (!String.IsNullOrEmpty(finder.Genre))
//            {
//                query = query.Where(m => m.MusicGenres.Any(mg => mg.Genre.Name.Contains(finder.Genre)));
//            }

//            result = query.ToList(); 

//            return result;
//        }


//    }
//}

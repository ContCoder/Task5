using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Task5Web.Models;

namespace Task5Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index(int? seed = null, string language = "en", double likes = 4, double review = 3)
        {
            var currentSeed = seed ?? new Random().Next(10000000, 99999999);

            var _books = Generate(1, currentSeed, language, likes, review);

            var viewModel = new BooksViewModel
            {
                Books = _books,
                HasMore = _books.Count >= 20,
                CurrentPage = 1,
                Seed = currentSeed,
                Language = language,
                Likes = likes,
                Review = review
            };

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult LoadMore(int page, int seed, string language, double likes, double review)
        {

            var allBooks = Generate(page, seed, language, likes, review);
            var pageSize = page == 1 ? 20 : 10;
            var skip = page == 1 ? 0 : 20 + ((page - 2) * 10);

            var books = allBooks.Skip(skip).Take(pageSize).ToList();
            var totalLoaded = skip + books.Count;

            return Json(new
            {
                books = allBooks,
                hasMore = true,
                currentPage = page
            });
        }
               
        private List<BookModel> Generate(int page, int seed, string language, double likes, double review)
        {
            return new BookGenerator().GeneratePage(seed, page, language, likes, review);
        }

    }
}

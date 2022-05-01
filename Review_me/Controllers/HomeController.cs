using Microsoft.AspNetCore.Mvc;
using Review_me.DataAccess;
using Newtonsoft.Json;
using Review_me.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Review_me.Controllers
{
    public class HomeController : Controller
    {
        public ApplicationDbContext dbContext;
        static string BASE_URL = "https://api.nytimes.com/svc/books/v3";
        static string API_KEY = "?api-key=f4TzHekDWiG8GYnbLknpadEJGJZJxdud"; //Add your API key here inside ""

        HttpClient httpClient;

        /// <summary>
        ///  Constructor to initialize the connection to the data source
        /// </summary>
        public HomeController(ApplicationDbContext context)
        {
            dbContext = context;
        }

        public List<Category> GetCategories()
        {
            string NAMES_API_PATH = BASE_URL + "/lists/names.json" + API_KEY;
            CategoryList categoryList = null;
            // It can take a few requests to get back a prompt response, if the API has not received
            //  calls in the recent past and the server has put the service on hibernation
            try
            {
                string namesData = HttpClitentCall(NAMES_API_PATH);

                if (!string.IsNullOrEmpty(namesData))
                {
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    categoryList = JsonConvert.DeserializeObject<CategoryList>(namesData);
                }
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return new List<Category>(categoryList.results);
        }

        public List<Book> GetCollection(string Category)
        {
            string BOOKS_API_PATH = BASE_URL + "/lists/current/" + Category + ".json" + API_KEY;
            WeeklyHitsObject weeklyHitsObject = null;
            // It can take a few requests to get back a prompt response, if the API has not received
            //  calls in the recent past and the server has put the service on hibernation
            try
            {
                string bookOverviewData = HttpClitentCall(BOOKS_API_PATH);

                if (!string.IsNullOrEmpty(bookOverviewData))
                {
                    Book book = new Book();
                    // JsonConvert is part of the NewtonSoft.Json Nuget package
                    weeklyHitsObject = JsonConvert.DeserializeObject<WeeklyHitsObject>(bookOverviewData);
                }
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }

            return new List<Book>(weeklyHitsObject.results.books);
        }

        public IActionResult Index()
        {
            List<Category> names = GetCategories();
            ViewBag.Categories = names;
            return View();
        }

        [HttpPost]
        public IActionResult Browse(string categories)
        {
            if (string.IsNullOrEmpty(categories))
            {
                List<Category> names = GetCategories();
                categories = names[new Random().Next(names.Count)].list_name;
            }
            List<Book> books = GetCollection(categories);
            books.ForEach(b =>
            {
                Book book = dbContext.Books.Where(x => x.title.Equals(b.title) && x.primary_isbn10.Equals(x.primary_isbn10)).FirstOrDefault();
                if (book == null)
                {
                    b.buyLinks = new List<BuyLink>(b.buy_links);
                    b.buy_links = null;
                    b.isbns = new List<Isbn>(b.isbns_array);
                    b.isbns_array = null;
                    dbContext.Books.Add(b);
                    dbContext.SaveChanges();
                }
                else
                {
                    b.bookId = book.bookId;
                }
            });
            int x = dbContext.Books.First().bookId;
            ViewBag.Books = books;
            return View();
        }

        [HttpGet]
        public IActionResult Browse()
        {
            return Browse(null);
        }

        public IActionResult About()
        {
            float booksCount = dbContext.Books.Count()/10;
            int booksReviewed = dbContext.ReviewLinks.GroupBy(r => r.bookId).Count();
            ViewBag.booksCount = booksCount;
            ViewBag.booksReviewed = booksReviewed;
            return View();
        }

        [HttpGet]
        public IActionResult UltimateBook(int bookId)
        {
            Book book = dbContext.Books.Include(b => b.buyLinks).Include(b => b.reviewLinks).Include(b => b.isbns).Where(b => b.bookId == bookId).FirstOrDefault();
            ViewBag.book = book;
            return View();
        }

        [HttpGet]
        public IActionResult AddReview(int bookId)
        {
            Book book = dbContext.Books.Include(b => b.buyLinks).Include(b => b.reviewLinks).Include(b => b.isbns).Where(b => b.bookId == bookId).FirstOrDefault();
            ViewBag.book = book;
            return View();
        }

        [HttpPost]
        public IActionResult AddReview([Bind("reviewLinkId, bookId,name,url")] ReviewLink review)
        {
            if (ModelState.IsValid)
            {
                dbContext.ReviewLinks.Add(review);
                dbContext.SaveChanges();
                return RedirectToAction("UltimateBook", "Home", new { bookId = review.bookId });
            }
            else
            {
                throw new Exception("Illegal object passed");
            }
        }

        [HttpGet]
        public IActionResult UpdateReview(int bookId, int reviewLinkId)
        {
            Book book = dbContext.Books.Include(b => b.buyLinks).Where(b => b.bookId == bookId).FirstOrDefault();
            ReviewLink review = dbContext.ReviewLinks.Include(r => r.book).Where(r => r.reviewLinkId == reviewLinkId).FirstOrDefault();
            ViewBag.book = book;
            ViewBag.review = review;
            return View();
        }

        [HttpPost]
        public IActionResult UpdateReview([Bind("reviewLinkId, bookId,name,url")] ReviewLink review)
        {
            if (ModelState.IsValid && review.reviewLinkId != 0)
            {
                dbContext.ReviewLinks.Update(review);
                dbContext.SaveChanges();
                return RedirectToAction("UltimateBook", "Home", new { bookId = review.bookId });
            }
            else
            {
                return AddReview(review);
            }
        }

        public IActionResult DeleteReview(int reviewLinkId)
        {
            ReviewLink reviewLink = dbContext.ReviewLinks.Where(r => r.reviewLinkId == reviewLinkId).FirstOrDefault();
            if(reviewLink == null)
            {
                throw new Exception("Illegal parameter passed");
            }
            dbContext.ReviewLinks.Remove(reviewLink);
            dbContext.SaveChanges();
            return RedirectToAction("UltimateBook", "Home", new { bookId = reviewLink.bookId });
        }

        public string HttpClitentCall(string API_PATH)
        {
            string responseString = "";

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = new Uri(API_PATH);
            // It can take a few requests to get back a prompt response, if the API has not received
            //  calls in the recent past and the server has put the service on hibernation
            try
            {
                HttpResponseMessage response = httpClient.GetAsync(API_PATH).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception e)
            {
                // This is a useful place to insert a breakpoint and observe the error message
                Console.WriteLine(e.Message);
            }
            return responseString;
        }
    }
}
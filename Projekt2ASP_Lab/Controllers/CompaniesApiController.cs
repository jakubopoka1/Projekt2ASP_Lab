using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Projekt2ASP_Lab.Models;
using Projekt2ASP_Lab.Models.Movies;
using Projekt2ASP_Lab.ViewModels;

namespace Projekt2ASP_Lab.Controllers;

[ApiController]
[Route("/api/companies")]
public class CompaniesApiController : Controller
{
    private readonly MoviesDbContext _context;

    public CompaniesApiController(MoviesDbContext context)
    {
        _context = context;
    }

    [HttpGet("list")]
    public IActionResult GetCompaniesWithDetails(int page = 1, int pageSize = 20)
    {
        var totalCompanies = _context.ProductionCompanies.Count();

        var companies = _context.ProductionCompanies
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ProductionCompanyViewModel
            {
                CompanyId = c.CompanyId,
                Name = c.CompanyName,
                MovieCount = _context.MovieCompanies.Count(mc => mc.CompanyId == c.CompanyId),
                TotalBudget = _context.MovieCompanies
                    .Where(mc => mc.CompanyId == c.CompanyId)
                    .Sum(mc => mc.Movie!.Budget ?? 0)
            })
            .AsNoTracking()
            .ToList();

        var viewModel = new PagedResult<ProductionCompanyViewModel>
        {
            Items = companies,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCompanies
        };

        return View("~/Views/Companies/production_companies_list.cshtml", viewModel);
    }




    [HttpGet("{companyId}/movies")]
    public IActionResult GetMoviesByCompanyView(int companyId, int page = 1, int pageSize = 20)
    {
        var totalMovies = _context.MovieCompanies
            .Count(mc => mc.CompanyId == companyId);

        var movies = _context.MovieCompanies
            .Where(mc => mc.CompanyId == companyId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(mc => new MovieViewModel
            {
                MovieId = mc.MovieId ?? 0,
                Title = mc.Movie!.Title,
                Popularity = mc.Movie.Popularity ?? 0,
                Revenue = mc.Movie.Revenue ?? 0,
                Runtime = mc.Movie.Runtime ?? 0,
                VotesAvg = Math.Round(mc.Movie.VoteAverage ?? 0, 1),
                VotesCount = mc.Movie.VoteCount ?? 0
            })
            .AsNoTracking()
            .ToList();

        ViewBag.CompanyName = _context.ProductionCompanies
            .FirstOrDefault(c => c.CompanyId == companyId)?.CompanyName;

        var viewModel = new PagedResult<MovieViewModel>
        {
            Items = movies,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalMovies
        };

        return View("~/Views/Companies/movies_by_company.cshtml", viewModel);
    }





    [HttpGet("movies/{movieId}/keywords")]
    public IActionResult GetKeywordsByMovie(int movieId)
    {
        var keywords = _context.MovieKeywords
            .Where(mk => mk.MovieId == movieId)
            .Select(mk => new KeywordViewModel
            {
                KeywordId = mk.Keyword!.KeywordId,
                KeywordName = mk.Keyword.KeywordName
            })
            .AsNoTracking()
            .ToList();

        return Ok(keywords);
    }

    [HttpPost("movies/{movieId}/keywords")]
    public IActionResult AddKeywordToMovie(int movieId, [FromForm] string keywordName)
    {
        try
        {
            Console.WriteLine($"Step 1: Received request with movieId={movieId}, keywordName={keywordName}");

            if (string.IsNullOrWhiteSpace(keywordName))
            {
                return BadRequest("Keyword name cannot be empty.");
            }

            var existingKeyword = _context.Keywords
                .FirstOrDefault(k => k.KeywordName!.ToLower() == keywordName.ToLower());

            int keywordId;
            if (existingKeyword != null)
            {
                keywordId = existingKeyword.KeywordId;
            }
            else
            {
                var maxId = _context.Keywords.Any()
                    ? _context.Keywords.Max(k => k.KeywordId)
                    : 0;
                keywordId = maxId + 1;

                var newKeyword = new Keyword
                {
                    KeywordId = keywordId,
                    KeywordName = keywordName
                };
                _context.Keywords.Add(newKeyword);
                _context.SaveChanges();
            }

            var existingRelation = _context.MovieKeywords
                .FirstOrDefault(mk => mk.MovieId == movieId && mk.KeywordId == keywordId);

            if (existingRelation == null)
            {
                var movieKeyword = new MovieKeyword
                {
                    MovieId = movieId,
                    KeywordId = keywordId
                };
                _context.MovieKeywords.Add(movieKeyword);
                _context.SaveChanges();
            }

            var keywords = _context.MovieKeywords
                .Where(mk => mk.MovieId == movieId)
                .Select(mk => mk.Keyword)
                .ToList();

            return PartialView("~/Views/Shared/_KeywordsList.cshtml", keywords);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "An internal error occurred.");
        }
    }



    [HttpGet("movies/{movieId}/manage-keywords")]
    public IActionResult ManageKeywords(int movieId)
    {
        var movie = _context.Movies.FirstOrDefault(m => m.MovieId == movieId);
        if (movie == null)
        {
            return NotFound();
        }

        var keywords = _context.MovieKeywords
            .Where(mk => mk.MovieId == movieId)
            .Select(mk => mk.Keyword)
            .ToList();

        var viewModel = new ManageKeywordsViewModel
        {
            MovieId = movieId,
            MovieTitle = movie.Title,
            Keywords = keywords
        };

        return View("~/Views/Companies/manage_keywords.cshtml", viewModel);
    }

}

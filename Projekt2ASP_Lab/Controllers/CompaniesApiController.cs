using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public IActionResult GetCompaniesWithDetails()
    {
        var companies = _context.ProductionCompanies
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

        return View("~/Views/Companies/production_companies_list.cshtml", companies);

    }



    [HttpGet("{companyId}/movies")]
    public IActionResult GetMoviesByCompanyView(int companyId)
    {
        var movies = _context.MovieCompanies
            .Where(mc => mc.CompanyId == companyId)
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

        return View("~/Views/Companies/movies_by_company.cshtml", movies);
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
    public IActionResult AddKeywordToMovie(int movieId, [FromBody] string keywordName)
    {
        var existingKeyword = _context.Keywords
            .FirstOrDefault(k => k.KeywordName!.ToLower() == keywordName.ToLower());

        if (existingKeyword == null)
        {
            existingKeyword = new Keyword { KeywordName = keywordName };
            _context.Keywords.Add(existingKeyword);
            _context.SaveChanges();
        }

        var existingRelation = _context.MovieKeywords
            .FirstOrDefault(mk => mk.MovieId == movieId && mk.KeywordId == existingKeyword.KeywordId);

        if (existingRelation == null)
        {
            var movieKeyword = new MovieKeyword
            {
                MovieId = movieId,
                KeywordId = existingKeyword.KeywordId
            };
            _context.MovieKeywords.Add(movieKeyword);
            _context.SaveChanges();
        }

        return Ok(new { Message = "Keyword added successfully." });
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
            .Select(mk => new KeywordViewModel
            {
                KeywordId = mk.Keyword!.KeywordId,
                KeywordName = mk.Keyword.KeywordName
            })
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

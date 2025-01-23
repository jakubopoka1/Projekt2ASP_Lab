using System.Collections.Generic;
using Projekt2ASP_Lab.Models.Movies;

namespace Projekt2ASP_Lab.ViewModels;

public class ManageKeywordsViewModel
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public List<Keyword> Keywords { get; set; } = new();
}

public class KeywordViewModel
{
    public int KeywordId { get; set; }
    public string KeywordName { get; set; } = string.Empty;
}
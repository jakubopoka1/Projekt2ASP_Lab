using System.Collections.Generic;

namespace Projekt2ASP_Lab.ViewModels;

public class ManageKeywordsViewModel
{
    public int MovieId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public List<KeywordViewModel> Keywords { get; set; } = new();
}

public class KeywordViewModel
{
    public int KeywordId { get; set; }
    public string KeywordName { get; set; } = string.Empty;
}
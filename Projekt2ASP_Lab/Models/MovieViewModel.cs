namespace Projekt2ASP_Lab.ViewModels;

public class MovieViewModel
{
    public int MovieId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double Popularity { get; set; }
    public long Revenue { get; set; }
    public int Runtime { get; set; }
    public double VotesAvg { get; set; }
    public int VotesCount { get; set; }
}
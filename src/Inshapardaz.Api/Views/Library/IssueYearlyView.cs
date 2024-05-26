namespace Inshapardaz.Api.Views.Library;

public class IssueYearlyView : ViewWithLinks
{
    public IEnumerable<IssueYearView> Data { get; set; }
}

public class IssueYearView : ViewWithLinks
{
    public int Year { get; set; }

    public int Count { get; set; }
}

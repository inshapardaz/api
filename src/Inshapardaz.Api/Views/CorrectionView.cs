namespace Inshapardaz.Api.Views;

public class CorrectionView : ViewWithLinks
{
    public long Id { get; set; }
    public string Language { get; set; }
    public string Profile { get; set; }
    public string IncorrectText { get; set; }
    public string CorrectText { get; set; }
    public bool CompleteWord { get; set; }

    // Read-only: changed through the usage endpoint, ignored on add/update.
    public long Usage { get; set; }
}

// The full list editors download; Id lets them report usage back.
public class CorrectionSimpleView
{
    public long Id { get; set; }
    public string IncorrectText { get; set; }
    public string CorrectText { get; set; }
    public bool CompleteWord { get; set; }
    public long Usage { get; set; }
}

// One entry of a correction list import/export file.
public class CorrectionTransferView
{
    public string IncorrectText { get; set; }
    public string CorrectText { get; set; }
    public bool CompleteWord { get; set; }
}

public class CorrectionUsageView
{
    public long Id { get; set; }
    public int Count { get; set; }
}

public class CorrectionImportResultView
{
    public int Added { get; set; }
    public int Updated { get; set; }
    public int Skipped { get; set; }
}

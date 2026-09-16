namespace NewsHub.Web.Areas.Admin.ViewModels.Components;

public class TableColumn
{
    public string Header { get; set; } = "";
    public string PropertyName { get; set; } = ""; // used for reflection or a Func
}

public class TableAction
{
    public string Label { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Url { get; set; } = "javascript:void(0);";
    public string CssClass { get; set; } = "dropdown-item";
}

public class TableRowViewModel
{
    public Dictionary<string, object?> Cells { get; set; } = new();
    public string? StatusLabel { get; set; }
    public string? StatusBadgeClass { get; set; } = "bg-label-primary";
    public List<TableAction> Actions { get; set; } = new();
}

public class TableViewModel
{
    public string Title { get; set; } = "";
    public List<TableColumn> Columns { get; set; } = new();
    public List<TableRowViewModel> Rows { get; set; } = new();
    public bool Hoverable { get; set; } = true;
}
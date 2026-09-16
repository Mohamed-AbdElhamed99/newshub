namespace NewsHub.Web.Areas.Admin.ViewModels.Components;

public enum FieldType { Text, Email, Phone, TextArea, Password, Number }

public class FormField
{
    public string Name { get; set; } = "";
    public string Label { get; set; } = "";
    public string Placeholder { get; set; } = "";
    public FieldType Type { get; set; } = FieldType.Text;
    public string? Icon { get; set; } // e.g. "bx bx-user"
    public object? Value { get; set; }
    public bool Required { get; set; }
}

public class FormViewModel
{
    public string Title { get; set; } = "";
    public string ActionUrl { get; set; } = "";
    public string Method { get; set; } = "post";
    public List<FormField> Fields { get; set; } = new();
    public string SubmitLabel { get; set; } = "Send";
}
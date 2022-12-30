using System.Dynamic;
using System.Net.Http.Headers;
using JiraTesterProData.Extensions;

namespace JiraTesterProData.JiraMapper;

public class JiraHtmlFieldDto
{
        public string FieldId { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        public string ElementType { get; set; }
        public bool IsRequired { get; set; }
        public bool IsCustomControl { get; set; }// => FieldId.StartsWith("custom");

        public bool RandomValueUsed { get; set; }
        
        public IList<string> AvailableValues { get; set; }
        public IList<string> SelectedValue { get; set; }

        public IList<string> InValidValuelist { get; set; }
        public string IsMandatory => IsRequired ? "Yes" : "No";

        public FieldGroupOrFieldLabel FieldGroupType { get; set; }

        public JiraHtmlFieldDto()
        {
            AvailableValues = new List<string>();
            SelectedValue=new List<string>();
            InValidValuelist = new List<string>();
        }
}

public static class ElementType
{
    public const string TextBox = "text";
    public const string TextArea = "textarea";
    public const string SingleDropdown = "select-one";
    public const string MultiDropdown = "select";
    public const string Checkbox = "checkbox";
    public const string DatePicker = "datepicker-input";
    public const string Combobox = "combobox";
    public const string FileUpload = "file";

    public static bool IsMultiValue(string elementtype)
    {
        return elementtype.EqualsWithIgnoreCase(Checkbox) || elementtype.EqualsWithIgnoreCase(MultiDropdown);
    }
}

public enum FieldGroupOrFieldLabel
{
    FieldGroup, Group, UnHandled
}

public static class CssSelectorClass
{
    public const string RequiredSelector = ".aui-icon.icon-required";
    public const string LabelSelector = "label";
    public const string LegendSelector = "legend > span";

    public const string FieldGroupSelector = ".field-group";
    public const string GroupSelector = ".group";
    public const string CheckboxSelector = "data-jbhv-checkbox-radio-fieldset-name";
    public const string CheckboxElementSelector = "data-jbhv-checkbox-radio-fieldname";
}

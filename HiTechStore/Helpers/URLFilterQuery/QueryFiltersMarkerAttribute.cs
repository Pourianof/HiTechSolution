namespace HiTechStore.Helpers.URLFilterQuery;

[AttributeUsage(AttributeTargets.Property)]
public class MiscQueryFiltersMarkerAttribute : Attribute
{ }

/// <summary>
/// To partitionarize the input quey string based on their prefixes which we called it <b>"namespace"</b>
/// <details>
/// /// For expample we define the "ct" prefix for storing all query string keys with this prefix in different Dictionary
/// <code>
/// [NamespacedQueryFiltersMarkerAttribute("ct")]
/// public Dictionary<string, QueriFilterItem> CategoryProperties {get; set;}
/// </code>
/// So any query strings like "ct.material" or "ct.capacity" will be there.
/// </details>
/// </summary>
public class NamespacedQueryFiltersMarkerAttribute : Attribute
{
    public string Namespace { get; }
    public NamespacedQueryFiltersMarkerAttribute(string ns)
    {
        Namespace = ns;
    }
}

public class BindingQueryAttribute : Attribute
{ }
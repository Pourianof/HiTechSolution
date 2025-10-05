namespace HiTechStore.Helpers.URLFilterQuery;

[AttributeUsage(AttributeTargets.Property)]
public class MiscQueryFiltersMarkerAttribute : Attribute
{ }

public class NamespacedQueryFiltersMarkerAttribute : Attribute
{
    public string Namespace { get; }
    public NamespacedQueryFiltersMarkerAttribute(string ns)
    {
        Namespace = ns;
    }
}
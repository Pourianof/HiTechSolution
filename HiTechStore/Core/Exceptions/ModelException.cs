namespace HiTechStore.Core.Exceptions;

public class ModelException : ApplicationException
{
    public string FieldName { get; private set; }

    public ModelException(string title, string description, string fieldName)
        : base(title, description)
    {
        FieldName = fieldName;
    }
}

namespace HiTechStore.Presentation.Controllers.ActionFilters.Exceptions
{
    public class NotExistedDbSetOfProvidedEntityTypeException : Exception
    {
        public NotExistedDbSetOfProvidedEntityTypeException(Type entityType)
            : base($"No DbSet found for entity type {entityType.Name}.")
        {
        }
    }
}
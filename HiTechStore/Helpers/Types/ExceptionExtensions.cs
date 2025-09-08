namespace HiTechStore.Helpers.Types
{
    public static class ExceptionExtensions
    {
        public static T? GetBaseExceptionOfType<T>(this Exception ex) where T : Exception
        {
            var root = ex;
            while (root.InnerException != null)
            {
                root = root.InnerException;

                if (root is T)
                {
                    return (T)root;
                }
            }
            return null;
        }
    }
}
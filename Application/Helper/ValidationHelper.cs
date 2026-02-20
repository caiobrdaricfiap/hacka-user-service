namespace Application.Helper
{
    public static class ValidationHelper
    {
        public static string ValidaEmpties<T>(T entity, string errorMessage)
        {
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType != typeof(string))
                    continue;

                if (string.IsNullOrEmpty(property.GetValue(entity) as string))
                {
                    errorMessage += property.Name + " não pode ser nulo. ";
                }
            }

            return errorMessage;
        }
    }
}

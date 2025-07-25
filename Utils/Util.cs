using System.Collections;
using System.Data.SqlTypes;

namespace apiBozzi.Utils;

public static class Util
{
    public static bool IsEmpty(this object source)
    {
        var result = (source == null ||
                      source.Equals(0) ||
                      source.ToString().Equals("0") ||
                      (source is string && source.ToString().Trim().Equals(string.Empty)) ||
                      source.Equals(string.Empty) ||
                      (source is decimal && ((decimal)source).Equals(0)) ||
                      SqlDateTime.MinValue.Equals(source) ||
                      DateTime.MinValue.Equals(source)) ||
                     (source is ICollection && ((ICollection)source).Count == 0) ||
                     (source is IQueryable && !((IQueryable<object>)source).Any()) ||
                     TimeSpan.Zero.Equals(source);

        if (!result)
        {
            var collectionType =
                source.GetType()
                    .GetInterfaces()
                    .SingleOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

            result = collectionType != null &&
                     ((int)collectionType.GetProperty("Count").GetValue(source, null)) == 0;
        }

        return result;
    }

    public static bool HasValue(this object source)
    {
        return !IsEmpty(source);
    }
}
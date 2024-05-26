using System.ComponentModel;
using System.Reflection;

namespace Inshapardaz.Api.Extensions;

public static class EnumExtensions
{
    public static string GetEnumDescription<T>(string value)
    {
        Type type = typeof(T);
        var name = Enum.GetNames(type)
                       .Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                       .Select(d => d).FirstOrDefault();

        if (name == null)
        {
            return string.Empty;
        }
        var field = type.GetField(name);
        var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return customAttribute.Any() ? ((DescriptionAttribute)customAttribute.First()).Description : name;
    }

    public static string GetEnumDescription<T>(T value)
    {
        Type type = value.GetType();

        MemberInfo[] memInfo = type.GetMember(value.ToString());

        if (memInfo.Length > 0)
        {
            var attrs = memInfo.First().GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Any())
            {
                return ((DescriptionAttribute)attrs.First()).Description;
            }
        }

        return value.ToString();
    }

    public static string GetFlagDescription<T>(T value)
    {
        var values = value.ToString().Split(',');
        var retval = string.Empty;
        foreach (var item in values)
        {
            var gtype = (T)Enum.Parse(typeof(T), item);
            retval += EnumExtensions.GetEnumDescription<T>(gtype.ToString()) + ",";
        }

        return retval.Trim(',', ' ');
    }

    public static string ToDescription<T>(this T value)
    {
        return GetEnumDescription<T>(value);
    }

    public static T ToEnum<T>(this string value, T defaultValue) where T : struct
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }

        T result;
        return Enum.TryParse<T>(value, true, out result) ? result : defaultValue;
    }
}

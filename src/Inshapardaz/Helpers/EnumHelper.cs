using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Inshapardaz.Helpers
{
    public static class EnumHelper
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
            var vals = value.ToString().Split(',');
            var retval = string.Empty;
            foreach (var item in vals)
            {
                var gtype = (T)Enum.Parse(typeof(T), item);
                retval += EnumHelper.GetEnumDescription<T>(gtype.ToString()) + ",";
            }

            retval.Trim(',', ' ');
            return retval;
        }

        public static string ToDescription<T>(this T value)
        {
            return GetEnumDescription<T>(value);
        }
    }
}

using Inshapardaz.Functions.Extensions;

namespace Inshapardaz.Functions.Converters
{
    public static class EnumRenderer
    {
        public static string Render<T>(this string source)
        {
            return EnumExtensions.GetEnumDescription<T>(source);
        }

        public static string Render<T>(this T source)
        {
            return EnumExtensions.GetEnumDescription<T>(source);
        }

        public static string RenderFlags<T>(this T source)
        {
            return EnumExtensions.GetFlagDescription<T>(source);
        }
    }
}

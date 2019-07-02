using Inshapardaz.Functions.Extensions;

namespace Inshapardaz.Functions.Adapters
{
    public class EnumRenderer : IRenderEnum
    {
        public string Render<T>(string source)
        {
            return EnumExtensions.GetEnumDescription<T>(source);
        }

        public string Render<T>(T source)
        {
            return EnumExtensions.GetEnumDescription<T>(source);
        }

        public string RenderFlags<T>(T source)
        {
            return EnumExtensions.GetFlagDescription<T>(source);
        }
    }
}
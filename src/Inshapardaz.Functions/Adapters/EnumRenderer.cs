using Inshapardaz.Functions.Extentions;

namespace Inshapardaz.Functions.Adapters
{
    public class EnumRenderer : IRenderEnum
    {
        public string Render<T>(string source)
        {
            return EnumExtentions.GetEnumDescription<T>(source);
        }

        public string Render<T>(T source)
        {
            return EnumExtentions.GetEnumDescription<T>(source);
        }

        public string RenderFlags<T>(T source)
        {
            return EnumExtentions.GetFlagDescription<T>(source);
        }
    }
}
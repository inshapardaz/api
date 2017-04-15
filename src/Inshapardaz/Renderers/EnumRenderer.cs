using Inshapardaz.Helpers;

namespace Inshapardaz.Renderers
{
    public class EnumRenderer : IRenderEnum
    {
        public string Render<T>(string source)
        {
            return EnumHelper.GetEnumDescription<T>(source);
        }

        public string Render<T>(T source)
        {
            return EnumHelper.GetEnumDescription<T>(source);
        }

        public string RenderFlags<T>(T source)
        {
            return EnumHelper.GetFlagDescription<T>(source);
        }
    }
}
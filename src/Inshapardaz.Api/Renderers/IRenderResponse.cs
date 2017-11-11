using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderLink
    {
        LinkView Render(string methodName, string rel);

        LinkView Render(string methodName, string rel, object data);

        LinkView Render(string methodName, string rel, string type, object data);
    }

    public interface IRenderEnum
    {
        string Render<T>(string source);

        string Render<T>(T source);

        string RenderFlags<T>(T source);
    }
}
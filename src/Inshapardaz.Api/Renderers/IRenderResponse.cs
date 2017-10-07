using System.Collections.Generic;
using Inshapardaz.Api.View;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderLink
    {
        LinkView Render(string methodName, string rel);

        LinkView Render(string methodName, string rel, object data);
    }

    public interface IRenderEnum
    {
        string Render<T>(string source);

        string Render<T>(T source);

        string RenderFlags<T>(T source);
    }

    public interface IRenderResponse<out T>
    {
        T Render();
    }

    public interface IRenderResponseFromObject<in TSource, out TDestination>
    {
        TDestination Render(TSource source);
    }

    public interface IRenderCollectionResponseFromObject<in TSource, out TDestination>
    {
        IEnumerable<TDestination> Render(IEnumerable<TSource> source);
    }
}
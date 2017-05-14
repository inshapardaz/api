using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class PageRendererArgs<T>
    {
        public string RouteName { get; set; }

        public Page<T> Page { get; set; }
        public string Query { get; internal set; }
    }
}
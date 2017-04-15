using Inshapardaz.Domain.Model;

namespace Inshapardaz.Renderers
{
    public class PageRendererArgs<T>
    {
        public string RouteName { get; set; }

        public Page<T> Page { get; set; }
    }
}
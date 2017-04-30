namespace Inshapardaz.Api.Renderers
{
    public abstract class RendrerBase
    {
        protected readonly IRenderLink LinkRenderer;

        protected RendrerBase(IRenderLink linkRenderer)
        {
            LinkRenderer = linkRenderer;
        }
    }
}
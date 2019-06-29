namespace Inshapardaz.Functions.Adapters
{
    public interface IRenderEnum
    {
        string Render<T>(string source);

        string Render<T>(T source);

        string RenderFlags<T>(T source);
    }
}
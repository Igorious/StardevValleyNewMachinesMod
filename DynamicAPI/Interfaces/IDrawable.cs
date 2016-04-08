namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IDrawable
    {
        int TextureIndex { get; }
        int? ResourceIndex { get; }
        int ResourceLength { get; }
    }
}

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IInformation
    {
        int ID { get; }
        string ToString();
        int ResourceLength { get; }
    }

    public interface ICropInformation : IInformation {}

    public interface IItemInformation : IInformation {}

    public interface ITreeInformation : IInformation {}
}
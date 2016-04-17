namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IInformation
    {
        int ID { get; }

        /// <summary>
        /// Get serialized string.
        /// </summary>
        string ToString();

        /// <summary>
        /// Length of reserved ID range.
        /// </summary>
        int ResourceLength { get; }
    }

    public interface ICropInformation : IInformation {}

    public interface IItemInformation : IInformation {}

    public interface ITreeInformation : IInformation {}
}
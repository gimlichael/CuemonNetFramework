namespace Cuemon.Data.Entity
{
    /// <summary>
    /// An interface describing the contract for retreiving an indentifier as a string.
    /// </summary>
    public interface IIdentifier
    {
        /// <summary>
        /// Gets the identifier of this object as a string.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> object.</returns>
        string GetIdentifier();
    }
}
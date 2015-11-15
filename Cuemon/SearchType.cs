namespace Cuemon
{
    /// <summary>
    /// Specifies the conditions of a search operation.
    /// </summary>
    public enum SearchType
    {
        /// <summary>
        /// Indicates that a search should determine whether the search string is an exact match of a field.
        /// </summary>
        Equals,
        /// <summary>
        /// Indicates that a search should determine whether the search string matches a string anywhere in a field.
        /// </summary>
        Contains,
        /// <summary>
        /// Indicates that a search should determine whether the search string matches a string at the beginning of a field.
        /// </summary>
        StartsWith,
        /// <summary>
        /// Indicates that a search should determine whether the search string matches a string at the end of a field.
        /// </summary>
        EndsWith
    }
}
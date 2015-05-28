namespace Cuemon
{
    /// <summary>
    /// Specifies a generic way of handling the supported actions for filtering statements.
    /// </summary>
    public enum Filter
    {
        /// <summary>
        /// Specifies that no filter should be applied.
        /// </summary>
        None = 0,
        /// <summary>
        /// If the condition is met, the input value(s) is excluded from the results.
        /// </summary>
        Exclude = 1,
        /// <summary>
        /// If the condition is met, only the input value(s) is included in the results.
        /// </summary>
        Include = 2
    }
}
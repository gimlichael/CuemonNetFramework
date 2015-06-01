using System;

namespace Cuemon.Web
{
    /// <summary>
    /// An interface designed to ease the implmentation of SEO-related initiatives.
    /// </summary>
    public interface ISearchEngineOptimizer
    {
        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> value from when a page was last modified, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <returns>A <see cref="DateTime"/> value from when a page was last modified, expressed as the Coordinated Universal Time (UTC).</returns>
        DateTime LastModified { get; set; }

        /// <summary>
        /// Gets or sets a hint on how frequently the content of the page is likely to change.
        /// </summary>
        /// <value>A hint on how frequently the content of the page is likely to change.</value>
        ChangeFrequency ChangeFrequency { get; set; }

        /// <summary>
        /// Gets or sets the relative search engine priority of a page, where 0.0 is the lowest priority and 1.0 is the highest.
        /// </summary>
        /// <value>The relative search engine priority of a page, where 0.0 is the lowest priority and 1.0 is the highest.</value>
        double CrawlerPriority { get; set; }
    }

    /// <summary>
    /// Specifies wow frequently a dynamic page is likely to change.
    /// </summary>
    public enum ChangeFrequency
    {
        /// <summary>
        /// Used to describe pages that change each time they are accessed.
        /// </summary>
        Always,
        /// <summary>
        /// Used to descripe pages that change on a hourly basis.
        /// </summary>
        Hourly,
        /// <summary>
        /// Used to descripe pages that change on a daily basis.
        /// </summary>
        Daily,
        /// <summary>
        /// Used to descripe pages that change on a weekly basis.
        /// </summary>
        Weekly,
        /// <summary>
        /// Used to descripe pages that change on a monthly basis.
        /// </summary>
        Monthly,
        /// <summary>
        /// Used to descripe pages that change on a yearly basis.
        /// </summary>
        Yearly,
        /// <summary>
        /// Used to describe pages that has been archived.
        /// </summary>
        Never
    }
}
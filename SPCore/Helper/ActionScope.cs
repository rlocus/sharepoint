
namespace SPCore.Helper
{
    public enum ActionScope
    {
        /// <summary>
        /// The scope of the Web site.
        /// </summary>
        Web = 0,
        /// <summary>
        /// The scope of the Root Web site on the site collection.
        /// </summary>
        RootWeb = 1,
        /// <summary>
        /// The scope of all Web sites immediately beneath the Web site, excluding children of those Web sites.
        /// </summary>
        SubWebs = 2,
        /// <summary>
        /// The scope of all Web sites immediately beneath the Web site, including children of those Web sites.
        /// </summary>
        SubWebsRecursion = 3,
        /// <summary>
        /// The scope of of the Web site and all Web sites immediately beneath the Web site, excluding children of those Web sites.
        /// </summary>
        WebAndSubWebs = 4,
        /// <summary>
        /// The scope of of the Web site and all Web sites immediately beneath the Web site, including children of those Web sites.
        /// </summary>
        WebAndSubWebsRecursion = 5,
        /// <summary>
        /// The scope of all Web sites that are contained within the site collection.
        /// </summary>
        AllWebs = 6
    }
}

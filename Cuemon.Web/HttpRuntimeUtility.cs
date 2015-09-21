using System;
using System.Reflection;
using System.Web;
using Cuemon.Reflection;

namespace Cuemon.Web
{
    /// <summary>
    /// This utility class is designed to make <see cref="HttpRuntime"/> operations easier to work with.
    /// </summary>
    public static class HttpRuntimeUtility
    {
        private static bool _supportsIisIntegratedPipelineMode = InitSupportsIisIntegratedPipelineMode();

        private static bool InitSupportsIisIntegratedPipelineMode()
        {
            try
            {
                return HttpRuntime.UsingIntegratedPipeline;
            }
            catch (MissingMethodException)
            {
                Type httpRuntimeType = typeof(HttpRuntime);
                FieldInfo useIntegratedPipeline = ReflectionUtility.GetField(httpRuntimeType, "_useIntegratedPipeline", ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic);
                if (useIntegratedPipeline != null &&
                    useIntegratedPipeline.FieldType == typeof(bool))
                {
                    return (bool)useIntegratedPipeline.GetValue(null);
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the executing platform of this ASP.NET implementation supports IIS integrated pipeline mode.
        /// </summary>
        /// <value><c>true</c> if the executing platform of this ASP.NET implementation supports IIS integrated pipeline mode; otherwise, <c>false</c>.</value>
        public static bool SupportsIisIntegratedPipelineMode
        {
            get { return _supportsIisIntegratedPipelineMode; }
            internal set { _supportsIisIntegratedPipelineMode = value; }

        }
    }
}
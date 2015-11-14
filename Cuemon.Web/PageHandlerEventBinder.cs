using System;
using System.Diagnostics;
using System.Globalization;
using System.Web.UI;
using Cuemon.Diagnostics;

namespace Cuemon.Web
{
    /// <summary>
    /// Provides access to <see cref="Page"/> events through generic delegates <see cref="Act{T}"/> while offering accurate time measurement of each event. This class cannot be inherited.
    /// </summary>
    public sealed class PageHandlerEventBinder : IHandlerEventBinder<Page>, ITimeMeasuring
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageHandlerEventBinder"/> class.
        /// </summary>
        /// <param name="handler">The <see cref="Page"/> of this instance.</param>
        public PageHandlerEventBinder(Page handler)
        {
            Handler = handler;
            Timer = Stopwatch.StartNew();
            ManageHandlerEvents();
        }

        /// <summary>
        /// Gets the <see cref="Page"/> that this instance represents.
        /// </summary>
        /// <value>The <see cref="Page"/> that this instance represents.</value>
        public Page Handler { get; private set; }

        /// <summary>
        /// Provides a way to initialize and manage the various <see cref="Page"/> specific events.
        /// </summary>
        public void ManageHandlerEvents()
        {
            Handler.PreInit += OnPreInitWrapper;
            Handler.Init += OnInitWrapper;
            Handler.InitComplete += OnInitCompleteWrapper;
            Handler.PreLoad += OnPreLoadWrapper;
            Handler.Load += OnLoadWrapper;
            Handler.LoadComplete += OnLoadCompleteWrapper;
            Handler.PreRender += OnPreRenderWrapper;
            Handler.PreRenderComplete += OnPreRenderCompleteWrapper;
            Handler.SaveStateComplete += OnSaveStateCompleteWrapper;
            Handler.Unload += OnUnloadWrapper;
        }

        /// <summary>
        /// Provides access to the Init event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked when the <see cref="Page"/> is initialized, which is the first step in its lifecycle.</remarks>
        public Act<Page> OnInit { get; set; }

        /// <summary>
        /// Provides access to the PreInit event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked at the beginning of the <see cref="Page"/> initialization.</remarks>
        public Act<Page> OnPreInit { get; set; }

        /// <summary>
        /// Provides access to the InitComplete event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked when the <see cref="Page"/> initialization is complete.</remarks>
        public Act<Page> OnInitComplete { get; set; }

        /// <summary>
        /// Provides access to the PreLoad event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked just before the <see cref="Page"/>/> load event.</remarks>
        public Act<Page> OnPreLoad { get; set; }

        /// <summary>
        /// Provides access to the Load event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked when the <see cref="Page"/>/> loads.</remarks>
        public Act<Page> OnLoad { get; set; }

        /// <summary>
        /// Provides access to the LoadComplete event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked at the end of the load stage of the <see cref="Page"/> life cycle.</remarks>
        public Act<Page> OnLoadComplete { get; set; }

        /// <summary>
        /// Provides access to the PreRender event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked after the <see cref="Page"/> is loaded but prior to rendering.</remarks>
        public Act<Page> OnPreRender { get; set; }

        /// <summary>
        /// Provides access to the PreRenderComplete event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked before the <see cref="Page"/> content is rendered.</remarks>
        public Act<Page> OnPreRenderComplete { get; set; }

        /// <summary>
        /// Provides access to the SaveStateComplete event of the currently executing <see cref="Page"/>.
        /// </summary>
        /// <remarks>This delegate is invoked after the <see cref="Page"/> has completed saving all view state and control state information.</remarks>
        public Act<Page> OnSaveStateComplete { get; set; }

        /// <summary>
        /// Provides access to the Unload event of the currently executing <see cref="Page" />.
        /// </summary>
        /// <remarks>This delegate is invoked when the <see cref="Page" /> is unloaded from memory.</remarks>
        public Act<Page> OnUnload { get; set; }

        /// <summary>
        /// Gets the timer that is used to accurately measure elapsed time.
        /// </summary>
        /// <value>The timer that is used to accurately measure elapsed time.</value>
        public Stopwatch Timer { get; private set; }

        /// <summary>
        /// Gets or sets the callback delegate for the measured elapsed time.
        /// </summary>
        /// <value>The callback delegate for the measured elapsed time.</value>
        public Act<string, TimeSpan> TimeMeasuringCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether time measuring is enabled.
        /// </summary>
        /// <value><c>true</c> if time measuring is enabled; otherwise, <c>false</c>.</value>
        public bool EnableTimeMeasuring { get; set; }

        private void OnPreInitWrapper(object sender, EventArgs e)
        {
            Handler.PreInit -= OnPreInitWrapper;
            DelegateUtility.InvokeIfNotNull(OnPreInit, sender as Page);
            PageLifecycleTracer("OnPreInit");
        }

        private void OnInitWrapper(object sender, EventArgs e)
        {
            Handler.Init -= OnInitWrapper;
            DelegateUtility.InvokeIfNotNull(OnInit, sender as Page);
            PageLifecycleTracer("OnInit");
        }

        private void OnInitCompleteWrapper(object sender, EventArgs e)
        {
            Handler.InitComplete -= OnInitCompleteWrapper;
            DelegateUtility.InvokeIfNotNull(OnInitComplete, sender as Page);
            PageLifecycleTracer("OnInitComplete");
        }

        private void OnPreLoadWrapper(object sender, EventArgs e)
        {
            Handler.PreLoad -= OnPreLoadWrapper;
            DelegateUtility.InvokeIfNotNull(OnPreLoad, sender as Page);
            PageLifecycleTracer("OnPreLoad");
        }

        private void OnLoadWrapper(object sender, EventArgs e)
        {
            Handler.Load -= OnLoadWrapper;
            DelegateUtility.InvokeIfNotNull(OnLoad, sender as Page);
            PageLifecycleTracer("OnLoad");
        }

        private void OnLoadCompleteWrapper(object sender, EventArgs e)
        {
            Handler.LoadComplete -= OnLoadCompleteWrapper;
            DelegateUtility.InvokeIfNotNull(OnLoadComplete, sender as Page);
            PageLifecycleTracer("OnLoadComplete");
        }

        private void OnPreRenderWrapper(object sender, EventArgs e)
        {
            Handler.PreRender -= OnPreRenderWrapper;
            DelegateUtility.InvokeIfNotNull(OnPreRender, sender as Page);
            PageLifecycleTracer("OnPreRender");
        }

        private void OnPreRenderCompleteWrapper(object sender, EventArgs e)
        {
            Handler.PreRenderComplete -= OnPreRenderCompleteWrapper;
            DelegateUtility.InvokeIfNotNull(OnPreRenderComplete, sender as Page);
            PageLifecycleTracer("OnPreRenderComplete");
        }

        private void OnSaveStateCompleteWrapper(object sender, EventArgs e)
        {
            Handler.SaveStateComplete -= OnSaveStateCompleteWrapper;
            DelegateUtility.InvokeIfNotNull(OnSaveStateComplete, sender as Page);
            PageLifecycleTracer("OnSaveStateComplete");
        }

        private void OnUnloadWrapper(object sender, EventArgs e)
        {
            Handler.Unload -= OnUnloadWrapper;
            DelegateUtility.InvokeIfNotNull(OnUnload, sender as Page);
            PageLifecycleTracer("OnUnload");
        }

        private void PageLifecycleTracer(string lifecycleEvent)
        {
            if (!EnableTimeMeasuring) { return; }
            Condition.Initialize(TupleUtility.CreateTwo(string.Format(CultureInfo.InvariantCulture, "Handler:{0}", lifecycleEvent), Timer.Elapsed), TimeMeasuringCallback, Condition.IsNull)
                .Invoke(tuple => Infrastructure.TraceWriteLifecycleEvent(tuple.Arg1, tuple.Arg2), tuple => TimeMeasuringCallback(tuple.Arg1, tuple.Arg2));
        }
    }
}
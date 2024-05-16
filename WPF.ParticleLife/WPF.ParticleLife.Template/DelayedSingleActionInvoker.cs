using System;
using System.Windows.Threading;

namespace WPF.ParticleLife.Template
{
    /// <summary>Enforces that an operation will only happen one time within a given time span.</summary>
    internal class DelayedSingleActionInvoker : DispatcherObject
    {
        #region Protected Properties

        protected Action ActionToInvoke { get; set; }

        protected DispatcherOperation LastOperation { get; set; }

        protected DispatcherPriority Priority { get; set; }

        protected DispatcherTimer Timer { get; set; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.
        /// TimeSpan is half a second.</summary>
        /// <param name="action">The action to execute.</param>
        public DelayedSingleActionInvoker(Action action)
        {
            ActionToInvoke = action;
            Priority = DispatcherPriority.Background;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Tick += Tick;
        }

        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.</summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="waitSpan">The amount of time to wait
        /// for the action to be relistened for before firing.</param>
        public DelayedSingleActionInvoker(Action action, TimeSpan waitSpan)
        {
            ActionToInvoke = action;
            Priority = DispatcherPriority.Background;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = waitSpan;
            Timer.Tick += Tick;
        }

        /// <summary>Initializes a new instance of DelayedSingleActionInvoker.</summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="waitSpan">The amount of time to wait
        /// for the action to be relistened for before firing.</param>
        /// <param name="priority">The priority.</param>
        public DelayedSingleActionInvoker(Action action, TimeSpan waitSpan, DispatcherPriority priority)
        {
            ActionToInvoke = action;
            Priority = priority;

            Timer = new DispatcherTimer(Priority, Dispatcher);
            Timer.Interval = waitSpan;
            Timer.Tick += Tick;
        }

        #endregion

        #region Methods

        private void Tick(object sender, EventArgs e)
        {
            LastOperation = Dispatcher.BeginInvoke((Action)delegate
            {
                Timer.Stop();

                ActionToInvoke.Invoke();

                LastOperation = null;
            }, Priority);
        }

        /// <summary>Begins invoking the action.</summary>
        public void BeginInvoke()
        {
            if (Timer.IsEnabled) Timer.Stop();

            if (LastOperation != null)
                LastOperation.Abort();

            Timer.Start();
        }

        #endregion
    }
}

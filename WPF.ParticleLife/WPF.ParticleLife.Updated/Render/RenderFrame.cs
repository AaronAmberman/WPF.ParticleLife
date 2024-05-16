using System;
using System.Windows.Media;

namespace WPF.ParticleLife.Updated.Render
{
    internal class RenderFrame
    {
        // https://stackoverflow.com/questions/5812384/why-is-frame-rate-in-wpf-irregular-and-not-limited-to-monitor-refresh
        // https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/

        #region Fields

        private static TimeSpan previous = TimeSpan.Zero;

        #endregion

        #region Events

        private static event EventHandler<RenderingEventArgs> onFrame;

        public static event EventHandler<RenderingEventArgs> OnFrame
        {
            add
            {
                if (onFrame == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering;

                onFrame += value;
            }
            remove
            {
                onFrame -= value;

                if (onFrame == null)
                    CompositionTarget.Rendering -= CompositionTarget_Rendering;
            }
        }

        #endregion

        #region Methods

        private static void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderingEventArgs rea = (RenderingEventArgs)e;

            if (rea.RenderingTime == previous)
                return;

            previous = rea.RenderingTime;

            onFrame.Invoke(null, rea);
        }

        #endregion
    }
}

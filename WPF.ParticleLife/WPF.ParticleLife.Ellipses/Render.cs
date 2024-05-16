using System;
using System.Windows.Media;

namespace WPF.ParticleLife.Ellipses
{
    public class Render
    {
        // https://stackoverflow.com/questions/5812384/why-is-frame-rate-in-wpf-irregular-and-not-limited-to-monitor-refresh
        // https://evanl.wordpress.com/2009/12/06/efficient-optimal-per-frame-eventing-in-wpf/

        #region Fields

        private static TimeSpan previous = TimeSpan.Zero;

        #endregion

        #region Events

        private static event EventHandler<RenderingEventArgs> renderFrame;
        public static event EventHandler<RenderingEventArgs> RenderFrame
        {
            add
            {
                if (renderFrame == null)
                    CompositionTarget.Rendering += CompositionTarget_Rendering;

                renderFrame += value;
            }
            remove
            {
                renderFrame -= value;

                if (renderFrame == null)
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

            renderFrame.Invoke(null, rea);
        }

        #endregion
    }
}

using System;
using System.Windows.Media;

namespace WPF.ParticleLife.Template.Render
{
    internal class RenderFrameDeltaTime
    {
        #region Fields

        private static double ellapsedPrevious;

        #endregion

        #region Properties

        #endregion

        #region Events

        /// <summary>Occurs when a new frame needs to be rendered. The argument is the amount time in milliseconds since the last frame was rendered.</summary>
        private static event EventHandler<double> onFrame;

        public static event EventHandler<double> OnFrame
        {
            add
            {
                if (onFrame == null)
                    RenderFrame.OnFrame += Render_RenderFrame;

                onFrame += value;
            }
            remove
            {
                onFrame -= value;

                if (onFrame == null)
                    RenderFrame.OnFrame -= Render_RenderFrame;
            }
        }

        #endregion

        #region Methods

        private static void Render_RenderFrame(object sender, RenderingEventArgs e)
        {
            double dif = e.RenderingTime.TotalMilliseconds - ellapsedPrevious;

            ellapsedPrevious = e.RenderingTime.TotalMilliseconds;

            onFrame?.Invoke(null, dif);
        }

        #endregion
    }
}

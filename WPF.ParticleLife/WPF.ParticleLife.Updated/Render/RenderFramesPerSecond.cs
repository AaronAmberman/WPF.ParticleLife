using System;
using System.Diagnostics;
using System.Threading;

namespace WPF.ParticleLife.Updated.Render
{
    internal class RenderFramesPerSecond
    {
        #region Fields

        private double ellapsed;
        private int frameCounter;
        private Stopwatch stopwatch;

        #endregion

        #region Properties

        /// <summary>Gets the frames per second.</summary>
        public int FramesPerSecond { get; private set; }

        #endregion

        #region Events

        /// <summary>Occurs when a new frame needs to be rendered. The argument is the amount time in milliseconds since the last frame was rendered.</summary>
        public event EventHandler<double> OnFrame;

        #endregion

        #region Methods

        private void Render_OnFrame(object sender, double e)
        {
            if (stopwatch == null)
                stopwatch = Stopwatch.StartNew();

            ellapsed += stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();

            Interlocked.Increment(ref frameCounter);

            if (ellapsed > 1000)
            {
                FramesPerSecond = frameCounter;
                frameCounter = 0;

                ellapsed -= 1000;
            }

            OnFrame?.Invoke(this, e);
        }

        public void Start()
        {
            RenderFrameDeltaTime.OnFrame += Render_OnFrame;
        }

        public void Stop()
        {
            RenderFrameDeltaTime.OnFrame -= Render_OnFrame;

            stopwatch?.Stop();
            stopwatch = null;

            ellapsed = 0;
            frameCounter = 0;
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Media.Imaging;
using WPF.ParticleLife.Template.Model;
using WPF.ParticleLife.Template.ViewModel;

namespace WPF.ParticleLife.Template.Rendering
{
    internal class UniverseRenderer : IDisposable
    {
        #region Fields

        private Bitmap bitmap;
        private Graphics graphics;

        #endregion

        #region Properties

        public Dictionary<string, Dictionary<string, double>> Forces { get; set; }

        public List<Particle> Particles { get; private set; } = new List<Particle>();

        public UniverseViewModel Universe { get; set; }

        #endregion

        #region Methods

        public void Dispose()
        {
            bitmap?.Dispose();
            graphics?.Dispose();

            bitmap = null;
            graphics = null;

            Particles.Clear();
            Particles = null;
        }

        public void DrawAtom(Atom atom)
        {
            float diameter = (float)Universe.Radius * 2;
            Pen border = Universe.BorderPen;

            foreach (Particle particle in atom.Particles) 
                DrawParticle((float)particle.X, (float)particle.Y, diameter, atom.ColorBrush, border);
        }

        public void DrawParticle(float x, float y, float diameter, SolidBrush color, Pen border) 
        {
            if (x < 0) x = 0;
            if (x + diameter > Universe.Width) x = (float)Universe.Width - diameter;

            if (y < 0) y = 0;
            if (y + diameter > Universe.Height) y = (float)Universe.Height - diameter;

            graphics.FillEllipse(color, x, y, diameter, diameter);
            graphics.DrawEllipse(border, x, y, diameter, diameter);
        }

        public void Initialize(int height, int width)
        {
            if (bitmap != null)
            {
                bitmap.Dispose();
                graphics.Dispose();
            }

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);

            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.Low;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
        }

        public void RemoveParticlesByName(string name)
        {
            int count = Particles.Count - 1;

            for (int i = count; i >= 0; i--)
            {
                if (Particles[i].Name == name)
                    Particles.RemoveAt(i);
            }
        }

        public BitmapSource Render()
        {
            if (bitmap == null) return null;

            graphics.Clear(Universe.BackgroundColorDrawing);

            float diameter = (float)Universe.Radius * 2;
            Pen border = Universe.BorderPen;

            foreach (AtomViewModel atom in Universe.Atoms)
                foreach (Particle particle in atom.Particles)
                    DrawParticle((float)particle.X, (float)particle.Y, diameter, atom.ColorBrush, border);

            BitmapSource bs = bitmap.ToBitmapSource();
            bs.Freeze();

            return bs;
        }

        public void UpdateParticlePositions(double deltaMilliseconds)
        {
            // todo
        }

        #endregion
    }
}

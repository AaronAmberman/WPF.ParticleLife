using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Numerics;
using System.Windows.Media.Imaging;
using WPF.ParticleLife.Updated.Model;
using WPF.ParticleLife.Updated.ViewModel;

namespace WPF.ParticleLife.Updated.Rendering
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
            #region Parallel
            // todo
            #endregion

            #region Synchronous

            #region Attempt 1

            // how can we make the synchronous version more efficient?
            // how can we make the particles at the edges not flash back and forth between the top/bottom or left/right borders if wrapping is turned on?
            //     - (the base Graphics version does not do this so much or at all...but this code is 99% the same and if it is made 100% the same is still has the flicker and does not act like the Graphics version)
            // how come if we turn off wrapping doesn't the velocity *= -1 seem to move the particles in the other direction?
            //     - we don't want them sitting on the edges, we want to move them away from the edges

            //foreach (Particle sourceParticle in Particles)
            //{
            //    double forceX = 0;
            //    double forceY = 0;

            //    foreach (Particle targetParticle in Particles)
            //    {
            //        if (sourceParticle == targetParticle) continue;

            //        double deltaX = sourceParticle.X - targetParticle.X;
            //        double deltaY = sourceParticle.Y - targetParticle.Y;

            //        if (Math.Abs(deltaX) > Universe.ParticleRange || Math.Abs(deltaY) > Universe.ParticleRange)
            //            continue;

            //        if (Universe.Wrap)
            //        {
            //            if (deltaX > Universe.Width * 0.5)
            //                deltaX -= Universe.Width;
            //            else if (deltaX < -Universe.Width * 0.5)
            //                deltaX += Universe.Width;

            //            if (deltaY > Universe.Height * 0.5)
            //                deltaY -= Universe.Height;
            //            else if (deltaY < -Universe.Height * 0.5)
            //                deltaY += Universe.Height;
            //        }

            //        double distance = deltaX * deltaX + deltaY * deltaY;
            //        double forceRange = Math.Sqrt(distance);

            //        if (forceRange > 0 && forceRange < Universe.ForceRange)
            //        {
            //            double force = Forces[sourceParticle.Name][targetParticle.Name];

            //            double attractionRepulsiveForce = force * (1 / forceRange);

            //            forceX += attractionRepulsiveForce * deltaX;
            //            forceY += attractionRepulsiveForce * deltaY;
            //        }

            //        sourceParticle.VelocityX = (sourceParticle.VelocityX + forceX) * (1.0 - Universe.Friction) * deltaMilliseconds / (Universe.TimeFactor * 1000);
            //        sourceParticle.VelocityY = (sourceParticle.VelocityY + forceY) * (1.0 - Universe.Friction) * deltaMilliseconds / (Universe.TimeFactor * 1000);

            //        if (sourceParticle.VelocityX > Universe.MaxVelocity) sourceParticle.VelocityX = Universe.MaxVelocity;
            //        if (sourceParticle.VelocityY > Universe.MaxVelocity) sourceParticle.VelocityY = Universe.MaxVelocity;

            //        sourceParticle.X += sourceParticle.VelocityX;
            //        sourceParticle.Y += sourceParticle.VelocityY;

            //        if (Universe.Wrap)
            //        {
            //            if (sourceParticle.X <= 0.0)
            //                sourceParticle.X += Universe.Width;
            //            else if (sourceParticle.X >= Universe.Width)
            //                sourceParticle.X -= Universe.Width;

            //            if (sourceParticle.Y <= 0.0)
            //                sourceParticle.Y += Universe.Height;
            //            else if (sourceParticle.Y >= Universe.Height)
            //                sourceParticle.Y -= Universe.Height;
            //        }
            //        else
            //        {
            //            if (sourceParticle.X < 0.0)
            //            {
            //                sourceParticle.VelocityX *= -1;
            //                sourceParticle.X = 0;
            //            }
            //            else if (sourceParticle.X + Universe.Diameter > Universe.Width)
            //            {
            //                sourceParticle.VelocityX *= -1;
            //                sourceParticle.X = Universe.Width - Universe.Diameter;
            //            }

            //            if (sourceParticle.Y < 0.0)
            //            {
            //                sourceParticle.VelocityY *= -1;
            //                sourceParticle.Y = 0;
            //            }
            //            else if (sourceParticle.Y + Universe.Diameter > Universe.Height)
            //            {
            //                sourceParticle.VelocityY *= -1;
            //                sourceParticle.Y = Universe.Height - Universe.Diameter;
            //            }
            //        }
            //    }
            //}

            #endregion

            #region Attempt 2

            // how can we make the synchronous version more efficient?
            // how can we make the particles at the edges not flash back and forth between the top/bottom or left/right borders if wrapping is turned on?
            //     - (the base Graphics version does not do this so much or at all...but this code is 99% the same and if it is made 100% the same is still has the flicker and does not act like the Graphics version)
            // how come if we turn off wrapping doesn't the velocity *= -1 seem to move the particles in the other direction?
            //     - we don't want them sitting on the edges, we want to move them away from the edges

            foreach (Particle sourceParticle in Particles)
            {
                double accelerationX = 0;
                double accelerationY = 0;

                foreach (Particle targetParticle in Particles)
                {
                    if (sourceParticle == targetParticle) continue;

                    double deltaX = sourceParticle.X - targetParticle.X;
                    double deltaY = sourceParticle.Y - targetParticle.Y;

                    if (Math.Abs(deltaX) > Universe.ParticleRange || Math.Abs(deltaY) > Universe.ParticleRange)
                        continue;

                    // what is this actually doing? shifting the delta by the entire height or width if it is greater than half of the width or height?
                    // what does that do?
                    //if (Universe.Wrap)
                    //{
                    //    if (deltaX > Universe.Width * 0.5)
                    //        deltaX -= Universe.Width;
                    //    else if (deltaX < -Universe.Width * 0.5)
                    //        deltaX += Universe.Width;

                    //    if (deltaY > Universe.Height * 0.5)
                    //        deltaY -= Universe.Height;
                    //    else if (deltaY < -Universe.Height * 0.5)
                    //        deltaY += Universe.Height;
                    //}

                    double distanceSqaured = deltaX * deltaX + deltaY * deltaY;
                    double distance = Math.Sqrt(distanceSqaured);
                    double force = Forces[sourceParticle.Name][targetParticle.Name];

                    if (distance > 0 && distance < Universe.ForceRange)
                    {
                        //accelerationX += deltaX * force / (distance * distance);
                        //accelerationY += deltaY * force / (distance * distance);
                        accelerationX += deltaX * force / (distance);
                        accelerationY += deltaY * force / (distance);
                    }

                    ////accelerationX += deltaX * force / (distance * distance);
                    ////accelerationY += deltaY * force / (distance * distance);
                    //accelerationX += deltaX * force / (distance);
                    //accelerationY += deltaY * force / (distance);

                    //double velX = (sourceParticle.VelocityX + accelerationX * deltaMilliseconds) * (1 - Universe.Friction) / Universe.TimeFactor;
                    //double velY = (sourceParticle.VelocityY + accelerationY * deltaMilliseconds) * (1 - Universe.Friction) / Universe.TimeFactor;
                    double velX = (sourceParticle.VelocityX + accelerationX * deltaMilliseconds) * (1 - Universe.Friction) / (Universe.TimeFactor * 1000);
                    double velY = (sourceParticle.VelocityY + accelerationY * deltaMilliseconds) * (1 - Universe.Friction) / (Universe.TimeFactor * 1000);

                    if (velX > Universe.MaxVelocity) velX = Universe.MaxVelocity;
                    else if (velX < -Universe.MaxVelocity) velX = -Universe.MaxVelocity;

                    if (velY > Universe.MaxVelocity) velY = Universe.MaxVelocity;
                    else if (velY < -Universe.MaxVelocity) velY = -Universe.MaxVelocity;

                    sourceParticle.VelocityX = velX;
                    sourceParticle.VelocityY = velY;

                    // update position based on velocity
                    sourceParticle.X += sourceParticle.VelocityX;
                    sourceParticle.Y += sourceParticle.VelocityY;

                    if (Universe.Wrap)
                    {
                        if (sourceParticle.X <= 0.0)
                            sourceParticle.X += Universe.Width;
                        else if (sourceParticle.X >= Universe.Width)
                            sourceParticle.X -= Universe.Width;

                        if (sourceParticle.Y <= 0.0)
                            sourceParticle.Y += Universe.Height;
                        else if (sourceParticle.Y >= Universe.Height)
                            sourceParticle.Y -= Universe.Height;
                    }
                    else
                    {
                        if (sourceParticle.X < 0.0)
                        {
                            sourceParticle.VelocityX *= -1;
                            sourceParticle.X = 0;
                        }
                        else if (sourceParticle.X + Universe.Diameter > Universe.Width)
                        {
                            sourceParticle.VelocityX *= -1;
                            sourceParticle.X = Universe.Width - Universe.Diameter;
                        }

                        if (sourceParticle.Y < 0.0)
                        {
                            sourceParticle.VelocityY *= -1;
                            sourceParticle.Y = 0;
                        }
                        else if (sourceParticle.Y + Universe.Diameter > Universe.Height)
                        {
                            sourceParticle.VelocityY *= -1;
                            sourceParticle.Y = Universe.Height - Universe.Diameter;
                        }
                    }
                }
            }

            #endregion

            #region Attempt 3

            // todo
            // this shit sucks

            #endregion

            #endregion
        }

        #endregion
    }
}

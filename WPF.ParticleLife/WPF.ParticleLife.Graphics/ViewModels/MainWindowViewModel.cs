using WPF.ParticleLife.Graphics.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WPF.ParticleLife.Graphics.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        private ICommand addAtomCommand;
        private BitmapSource bitmap;
        private Bitmap bitmapImageSource;
        private System.Drawing.Graphics graphics;
        private long ellapsed;
        private long ellapsedPrevious;
        private int fps;
        private int frameCount;
        private SettingsViewModel settingsViewModel;
        private Stopwatch stopwatch;
        private int totalParticles;
        private UniverseViewModel universeViewModel;

        #endregion

        #region Properties

        public ICommand AddAtomCommand => 
            addAtomCommand ?? (addAtomCommand = new RelayCommand(AddAtom));

        public BitmapSource Bitmap 
        {
            get => bitmap;
            set
            {
                bitmap = value;
                OnPropertyChanged();
            }
        }

        public int FramesPerSecond
        {
            get { return fps; }
            set
            {
                fps = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel SettingsViewModel
        {
            get { return settingsViewModel; }
            set 
            {
                settingsViewModel = value;
                OnPropertyChanged();
            }
        }

        public int TotalParticles
        {
            get { return totalParticles; }
            set
            {
                totalParticles = value;
                OnPropertyChanged();
            }
        }

        public UniverseViewModel UniverseViewModel
        {
            get { return universeViewModel; }
            set
            {
                universeViewModel = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void AddAtom()
        {
            // we are out of colors (there are 138), just leave
            if (UniverseViewModel.Atoms.Count >= ColorHelper.MediaColors.Count) return;

            // get random color
            MediaColor mediaColor = ColorHelper.GetRandomMediaColor();

            while(UniverseViewModel.Atoms.Any(x => x.Name == mediaColor.Name))
                mediaColor = ColorHelper.GetRandomMediaColor();

            // create atom
            AtomViewModel atomViewModel = new AtomViewModel(new Atom
            {
                Color = mediaColor.Color,
                Count = SettingsViewModel.ParticleCount,
                Name = mediaColor.Name,
                Radius = 3.0,
                Diameter = 6.0,
                MaxHeight = UniverseViewModel.Size.Height,
                MaxWidth = UniverseViewModel.Size.Width
            });
            atomViewModel.RemoveAtomAction = RemoveAtomFromUniverse;

            // add particles
            for (int i = 0; i < SettingsViewModel.ParticleCount; i++)
            {
                ParticleViewModel particleViewModel = new ParticleViewModel(new Particle 
                {
                    X = Random.Shared.NextDouble() * atomViewModel.MaxWidth,
                    Y = Random.Shared.NextDouble() * atomViewModel.MaxHeight
                });

                atomViewModel.Particles.Add(particleViewModel);
            }

            // add atom to universe
            UniverseViewModel.Atoms.Add(atomViewModel);

            TotalParticles = UniverseViewModel.Atoms.Sum(x => x.Particles.Count);

            // add a random force for each atom in the universe (including itself)
            foreach (AtomViewModel atom in UniverseViewModel.Atoms)
            {
                // add force to new atom for existing atom
                ForceViewModel forceViewModel = new ForceViewModel(new Force(atom.Model), atom);
                forceViewModel.Attraction = RandomDouble(-1.0, 1.0);

                while (Math.Abs(forceViewModel.Attraction) < 0.01)
                    forceViewModel.Attraction = RandomDouble(-1.0, 1.0);

                atomViewModel.Forces.Add(forceViewModel);

                // add force to existing atom for new atom
                if (atom.Forces.Any(x => x.Target == atomViewModel)) continue; // do not add duplicates

                ForceViewModel forceViewModel2 = new ForceViewModel(new Force(atomViewModel.Model), atomViewModel);
                forceViewModel2.Attraction = RandomDouble(-1.0, 1.0);

                while (Math.Abs(forceViewModel2.Attraction) < 0.01)
                    forceViewModel2.Attraction = RandomDouble(-1.0, 1.0);

                atom.Forces.Add(forceViewModel2);
            }

            DrawAtom(atomViewModel);
        }

        public void Dispose()
        {
            graphics?.Dispose();
            bitmapImageSource?.Dispose();
            UniverseViewModel?.Dispose();
        }

        public void DrawAtom(AtomViewModel atom)
        {
            System.Drawing.Color atomColor = atom.Color.ToDrawingColor();

            if (bitmapImageSource == null)
            {
                bitmapImageSource = new Bitmap((int)UniverseViewModel.Size.Width, (int)UniverseViewModel.Size.Height);
                graphics = System.Drawing.Graphics.FromImage(bitmapImageSource);

                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            }

            foreach (ParticleViewModel particle in atom.Particles)
            {
                DrawParticle(particle, atom.Name, atom.Radius, atom.Diameter, atomColor);
            }
        }

        public void DrawParticle(ParticleViewModel particle, string name, double radius, double diameter, System.Drawing.Color color) 
        {
            double x = particle.X - radius;
            double y = particle.Y - radius;

            if (x < 0) x = 0;
            if (x + diameter > UniverseViewModel.Size.Width) x = UniverseViewModel.Size.Width - diameter;

            if (y < 0) y = 0;
            if (y + diameter > UniverseViewModel.Size.Height) y = UniverseViewModel.Size.Height - diameter;

            graphics.FillEllipse(new SolidBrush(color), (int)x, (int)y, (int)diameter, (int)diameter);
            graphics.DrawEllipse(new System.Drawing.Pen(SettingsViewModel.BorderColorDrawing), (int)x, (int)y, (int)diameter, (int)diameter);
        }

        private double RandomDouble(double min, double max)
        {
            return Random.Shared.NextDouble() * (max - min) + min;
        }

        public void RemoveAtomFromUniverse(AtomViewModel atom)
        {
            // remove atom from collection of atoms
            UniverseViewModel.Atoms.Remove(atom);

            // remove forces for this atom from all other atoms
            foreach (AtomViewModel a in UniverseViewModel.Atoms)
            {
                foreach (ForceViewModel force in a.Forces)
                {
                    if (force.Target.Name == atom.Name)
                    {
                        a.Forces.Remove(force);

                        break;
                    }
                }
            }

            // remove atom from universe drawing
            // there is no need to do anything here because when UpdateRenderEnvironment is called to animate,
            // the removed particle will disappear from the next rendered bitmap because it won't exist in the
            // universe's collection of atoms

            atom.Dispose();

            TotalParticles = UniverseViewModel.Atoms.Sum(x => x.Particles.Count);
        }

        public void UpdateRenderEnvironment()
        {
            if (stopwatch == null)
                stopwatch = Stopwatch.StartNew();

            ellapsed += stopwatch.ElapsedMilliseconds;

            double diff = (ellapsed - ellapsedPrevious) / 1000.0;

            stopwatch.Restart();

            frameCount++;

            if (ellapsed >= 1000)
            {
                FramesPerSecond = frameCount;
                frameCount = 0;

                ellapsed -= 1000;
            }

            CalculateAtomUpdate(diff);
            RenderAtomUpdate();

            ellapsedPrevious = ellapsed;
        }

        private void CalculateAtomUpdate(double delta)
        {
            #region Parallel

            Parallel.ForEach(UniverseViewModel.Atoms, (AtomViewModel atomSource) =>
            {
                Parallel.ForEach(UniverseViewModel.Atoms, (AtomViewModel atomTarget) =>
                {
                    ForceViewModel attForce = atomSource.Forces.FirstOrDefault(x => x.Target == atomTarget);

                    if (attForce == null) return;

                    foreach (ParticleViewModel sourceParticle in atomSource.Particles)
                    {
                        double forceX = 0;
                        double forceY = 0;

                        foreach (ParticleViewModel targetParticle in atomTarget.Particles)
                        {
                            double deltaX = sourceParticle.X - targetParticle.X;
                            double deltaY = sourceParticle.Y - targetParticle.Y;

                            if (SettingsViewModel.Wrap)
                            {
                                if (deltaX > UniverseViewModel.Size.Width * 0.5)
                                {
                                    deltaX -= UniverseViewModel.Size.Width;
                                }
                                else if (deltaX < -UniverseViewModel.Size.Width * 0.5)
                                {
                                    deltaX += UniverseViewModel.Size.Width;
                                }

                                if (deltaY > UniverseViewModel.Size.Height * 0.5)
                                {
                                    deltaY -= UniverseViewModel.Size.Height;
                                }
                                else if (deltaY < -UniverseViewModel.Size.Height * 0.5)
                                {
                                    deltaY += UniverseViewModel.Size.Height;
                                }
                            }

                            double distance = deltaX * deltaX + deltaY * deltaY;
                            double forceRange = Math.Sqrt(distance);

                            if (forceRange > 0 && forceRange < SettingsViewModel.ForceRangeMax)
                            {
                                double force = attForce.Attraction * 1 / forceRange;

                                forceX += force * deltaX;
                                forceY += force * deltaY;
                            }

                            //sourceParticle.VelocityX = (sourceParticle.VelocityX + forceX) * (1.0 - SettingsViewModel.Friction);
                            //sourceParticle.VelocityY = (sourceParticle.VelocityY + forceY) * (1.0 - SettingsViewModel.Friction);
                            sourceParticle.VelocityX = (sourceParticle.VelocityX + forceX) * (1.0 - SettingsViewModel.Friction) * delta; // this is not done in the original
                            sourceParticle.VelocityY = (sourceParticle.VelocityY + forceY) * (1.0 - SettingsViewModel.Friction) * delta; // this is not done in the original

                            sourceParticle.X += sourceParticle.VelocityX;
                            sourceParticle.Y += sourceParticle.VelocityY;

                            if (SettingsViewModel.Wrap)
                            {
                                if (sourceParticle.X < 0)
                                {
                                    sourceParticle.X += UniverseViewModel.Size.Width;
                                }
                                else if (sourceParticle.X >= UniverseViewModel.Size.Width)
                                {
                                    sourceParticle.X -= UniverseViewModel.Size.Width;
                                }

                                if (sourceParticle.Y < 0)
                                {
                                    sourceParticle.Y += UniverseViewModel.Size.Height;
                                }
                                else if (sourceParticle.Y >= UniverseViewModel.Size.Height)
                                {
                                    sourceParticle.Y -= UniverseViewModel.Size.Height;
                                }
                            }
                            else
                            {
                                if (sourceParticle.X < 0.0)
                                {
                                    sourceParticle.VelocityX *= -1;
                                    sourceParticle.X = 0;
                                }
                                else if (sourceParticle.X + atomSource.Diameter > UniverseViewModel.Size.Width)
                                {
                                    sourceParticle.VelocityX *= -1;
                                    sourceParticle.X = UniverseViewModel.Size.Width - atomSource.Diameter;
                                }

                                if (sourceParticle.Y < 0.0)
                                {
                                    sourceParticle.VelocityY *= -1;
                                    sourceParticle.Y = 0;
                                }
                                else if (sourceParticle.Y + atomSource.Diameter > UniverseViewModel.Size.Height)
                                {
                                    sourceParticle.VelocityY *= -1;
                                    sourceParticle.Y = UniverseViewModel.Size.Height - atomSource.Diameter;
                                }
                            }
                        }
                    }
                });
            });

            #endregion

            #region Synchronous

            //foreach (AtomViewModel atomSource in UniverseViewModel.Atoms)
            //{
            //    foreach (AtomViewModel atomTarget in UniverseViewModel.Atoms)
            //    {
            //        ForceViewModel attForce = atomSource.Forces.FirstOrDefault(x => x.Target == atomTarget);

            //        if (attForce == null) return;

            //        foreach (ParticleViewModel sourceParticle in atomSource.Particles)
            //        {
            //            double forceX = 0;
            //            double forceY = 0;

            //            foreach (ParticleViewModel targetParticle in atomTarget.Particles)
            //            {
            //                double deltaX = sourceParticle.X - targetParticle.X;
            //                double deltaY = sourceParticle.Y - targetParticle.Y;

            //                if (SettingsViewModel.Wrap)
            //                {
            //                    if (deltaX > UniverseViewModel.Size.Width * 0.5)
            //                    {
            //                        deltaX -= UniverseViewModel.Size.Width;
            //                    }
            //                    else if (deltaX < -UniverseViewModel.Size.Width * 0.5)
            //                    {
            //                        deltaX += UniverseViewModel.Size.Width;
            //                    }

            //                    if (deltaY > UniverseViewModel.Size.Height * 0.5)
            //                    {
            //                        deltaY -= UniverseViewModel.Size.Height;
            //                    }
            //                    else if (deltaY < -UniverseViewModel.Size.Height * 0.5)
            //                    {
            //                        deltaY += UniverseViewModel.Size.Height;
            //                    }
            //                }

            //                double distance = deltaX * deltaX + deltaY * deltaY;
            //                double forceRange = Math.Sqrt(distance);

            //                if (forceRange > 0 && forceRange < SettingsViewModel.ForceRangeMax)
            //                {
            //                    double force = attForce.Attraction * 1 / forceRange;

            //                    forceX += force * deltaX;
            //                    forceY += force * deltaY;
            //                }

            //                //sourceParticle.VelocityX = (sourceParticle.VelocityX + forceX) * (1.0 - SettingsViewModel.Friction);
            //                //sourceParticle.VelocityY = (sourceParticle.VelocityY + forceY) * (1.0 - SettingsViewModel.Friction);
            //                sourceParticle.VelocityX = (sourceParticle.VelocityX + forceX) * (1.0 - SettingsViewModel.Friction) * delta; // this is not done in the original
            //                sourceParticle.VelocityY = (sourceParticle.VelocityY + forceY) * (1.0 - SettingsViewModel.Friction) * delta; // this is not done in the original

            //                sourceParticle.X += sourceParticle.VelocityX;
            //                sourceParticle.Y += sourceParticle.VelocityY;

            //                if (SettingsViewModel.Wrap)
            //                {
            //                    if (sourceParticle.X < 0)
            //                    {
            //                        sourceParticle.X += UniverseViewModel.Size.Width;
            //                    }
            //                    else if (sourceParticle.X >= UniverseViewModel.Size.Width)
            //                    {
            //                        sourceParticle.X -= UniverseViewModel.Size.Width;
            //                    }

            //                    if (sourceParticle.Y < 0)
            //                    {
            //                        sourceParticle.Y += UniverseViewModel.Size.Height;
            //                    }
            //                    else if (sourceParticle.Y >= UniverseViewModel.Size.Height)
            //                    {
            //                        sourceParticle.Y -= UniverseViewModel.Size.Height;
            //                    }
            //                }
            //                else
            //                {
            //                    if (sourceParticle.X < 0.0)
            //                    {
            //                        sourceParticle.VelocityX *= -1;
            //                        sourceParticle.X = 0;
            //                    }
            //                    else if (sourceParticle.X + atomSource.Diameter > UniverseViewModel.Size.Width)
            //                    {
            //                        sourceParticle.VelocityX *= -1;
            //                        sourceParticle.X = UniverseViewModel.Size.Width - atomSource.Diameter;
            //                    }

            //                    if (sourceParticle.Y < 0.0)
            //                    {
            //                        sourceParticle.VelocityY *= -1;
            //                        sourceParticle.Y = 0;
            //                    }
            //                    else if (sourceParticle.Y + atomSource.Diameter > UniverseViewModel.Size.Height)
            //                    {
            //                        sourceParticle.VelocityY *= -1;
            //                        sourceParticle.Y = UniverseViewModel.Size.Height - atomSource.Diameter;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion
        }

        private void RenderAtomUpdate()
        {
            if (bitmapImageSource == null) return;

            graphics.Clear(SettingsViewModel.BackgroundColorDrawing);

            foreach (AtomViewModel atom in UniverseViewModel.Atoms)
            {
                System.Drawing.Color atomColor = atom.Color.ToDrawingColor();

                foreach (ParticleViewModel particle in atom.Particles)
                {
                    DrawParticle(particle, atom.Name, atom.Radius, atom.Diameter, atomColor);
                }
            }

            Bitmap = CreateBitmapSourceFromGdiBitmap(bitmapImageSource);
            Bitmap.Freeze();
        }

        public BitmapSource CreateBitmapSourceFromGdiBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(bitmap.Width, bitmap.Height, bitmap.HorizontalResolution, bitmap.VerticalResolution,
                    PixelFormats.Bgra32, null, bitmapData.Scan0, size, bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        #endregion
    }
}

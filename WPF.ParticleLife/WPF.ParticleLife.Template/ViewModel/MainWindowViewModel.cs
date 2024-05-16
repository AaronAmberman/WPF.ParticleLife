using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPF.ParticleLife.Template.Model;
using WPF.ParticleLife.Template.Render;
using WPF.ParticleLife.Template.Rendering;
using WPF.ParticleLife.Template.Type;

namespace WPF.ParticleLife.Template.ViewModel
{
    internal class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        private ICommand addAtomCommand;
        private BitmapSource bitmap;
        private int framesPerSecond;
        private RenderFramesPerSecond renderEnvironment;
        private ICommand resetParticlesCommand;
        private int totalParticles;
        private UniverseRenderer universeRenderer;
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
            get { return framesPerSecond; }
            set
            {
                framesPerSecond = value;
                OnPropertyChanged();
            }
        }

        public ICommand ResetParticlesCommand =>
            resetParticlesCommand ?? (resetParticlesCommand = new RelayCommand(ResetParticles));

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

                universeRenderer.Universe = value;

                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public MainWindowViewModel()
        {
            renderEnvironment = new RenderFramesPerSecond();
            renderEnvironment.OnFrame += RenderEnvironment_OnFrame;

            universeRenderer = new UniverseRenderer();
        }

        #endregion

        #region Methods

        private void AddAtom()
        {
            // we are out of colors (there are 138), just leave
            if (UniverseViewModel.Atoms.Count >= ColorHelper.MediaColors.Count) return;

            StopRenderingCycle();

            // get random color
            MediaColor mediaColor = GetRandomMediaColor();
            System.Drawing.Color atomColor = mediaColor.Color.ToDrawingColor();

            // create atom, view models to update the UI
            AtomViewModel atomViewModel = new AtomViewModel(new Atom
            {
                Color = atomColor,
                ColorBrush = new System.Drawing.SolidBrush(atomColor),
                Name = mediaColor.Name
            })
            {
                MediaColor = mediaColor.Color
            };
            atomViewModel.Remove += RemoveAtomFromUniverse;

            // add particles
            for (int i = 0; i < UniverseViewModel.ParticleCount; i++)
            {
                atomViewModel.Particles.Add(new Particle
                {
                    Name = atomViewModel.Name,
                    X = Random.Shared.NextDouble() * UniverseViewModel.Width,
                    Y = Random.Shared.NextDouble() * UniverseViewModel.Height
                });
            }

            // add atom to universe
            UniverseViewModel.Atoms.Add(atomViewModel);

            GenerateRandomForcesForAtom(atomViewModel);

            // get data to high efficiency renderer
            universeRenderer.Forces = UniverseViewModel.Atoms.ToDictionary(x => x.Name, x => x.Forces.ToDictionary(y => y.Target.Name, y => y.Attraction));
            universeRenderer.Particles.AddRange(atomViewModel.Model.Particles);

            TotalParticles = universeRenderer.Particles.Count;

            universeRenderer.DrawAtom(atomViewModel.Model);

            StartRenderingCycle();
        }

        public void Dispose()
        {
            universeRenderer.Dispose();
            UniverseViewModel?.Dispose();
        }

        private void GenerateRandomForcesForAtom(AtomViewModel atomViewModel)
        {
            // add a random force for each atom in the universe (including itself)
            foreach (AtomViewModel atom in UniverseViewModel.Atoms)
            {
                // add force to new atom for existing atom
                ForceViewModel forceViewModel = new ForceViewModel(new Force(atom.Model), atom);
                forceViewModel.Attraction = NumberHelper.RandomDoubleNegative1To1();

                while (Math.Abs(forceViewModel.Attraction) < 0.01)
                    forceViewModel.Attraction = NumberHelper.RandomDoubleNegative1To1();

                atomViewModel.Forces.Add(forceViewModel);

                // add force to existing atom for new atom
                if (atom.Forces.Any(x => x.Target == atomViewModel)) continue; // do not add duplicates

                ForceViewModel forceViewModel2 = new ForceViewModel(new Force(atomViewModel.Model), atomViewModel);
                forceViewModel2.Attraction = NumberHelper.RandomDoubleNegative1To1();

                while (Math.Abs(forceViewModel2.Attraction) < 0.01)
                    forceViewModel2.Attraction = NumberHelper.RandomDoubleNegative1To1();

                atom.Forces.Add(forceViewModel2);
            }
        }

        private MediaColor GetRandomMediaColor()
        {
            MediaColor mediaColor = ColorHelper.GetRandomMediaColor();

            while (UniverseViewModel.Atoms.Any(x => x.Name == mediaColor.Name))
                mediaColor = ColorHelper.GetRandomMediaColor();

            return mediaColor;
        }

        public void InitializeRenderer()
        {
            StopRenderingCycle();

            universeRenderer.Initialize((int)UniverseViewModel.Height, (int)UniverseViewModel.Width);

            StartRenderingCycle();
        }

        private void RemoveAtomFromUniverse(object sender, EventArgs e)
        {
            AtomViewModel atomViewModel = sender as AtomViewModel;

            if (atomViewModel == null) return;

            RemoveAtomFromUniverse(atomViewModel);
        }

        private void RemoveAtomFromUniverse(AtomViewModel atom)
        {
            StopRenderingCycle();

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

            universeRenderer.RemoveParticlesByName(atom.Name);

            atom.Dispose();

            // update forces to reflect removal of atom
            universeRenderer.Forces = UniverseViewModel.Atoms.ToDictionary(x => x.Name, x => x.Forces.ToDictionary(y => y.Target.Name, y => y.Attraction));

            TotalParticles = universeRenderer.Particles.Count;

            Bitmap = universeRenderer.Render();

            StartRenderingCycle();
        }

        private void RenderEnvironment_OnFrame(object sender, double deltaMilliseconds)
        {
            FramesPerSecond = renderEnvironment.FramesPerSecond;

            if (universeRenderer.Particles.Count > 0) 
            {
                universeRenderer.UpdateParticlePositions(deltaMilliseconds);

                Bitmap = universeRenderer.Render();
            }
        }

        public void StartRenderingCycle()
        {
            renderEnvironment.Start();
        }

        public void StopRenderingCycle()
        {
            renderEnvironment.Stop();
        }

        private void ResetParticles()
        {
            StopRenderingCycle();

            foreach (AtomViewModel atom in UniverseViewModel.Atoms)
            {
                atom.Particles.Clear();

                for (int i = 0; i < UniverseViewModel.ParticleCount; i++)
                {
                    atom.Particles.Add(new Particle
                    {
                        Name = atom.Name,
                        X = Random.Shared.NextDouble() * UniverseViewModel.Width,
                        Y = Random.Shared.NextDouble() * UniverseViewModel.Height
                    });
                }
            }

            universeRenderer.Particles.Clear();
            universeRenderer.Particles.AddRange(UniverseViewModel.Atoms.SelectMany(x => x.Particles).ToList());

            Bitmap = universeRenderer.Render();

            StartRenderingCycle();
        }

        #endregion
    }
}

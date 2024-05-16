using WPF.ParticleLife.Graphics.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace WPF.ParticleLife.Graphics.ViewModels
{
    public class AtomViewModel : ViewModelBase<Atom>, IDisposable
    {
        #region Fields

        private ObservableCollection<ForceViewModel> forces;
        private ObservableCollection<ParticleViewModel> particles;
        private ICommand removeCommand;

        #endregion

        #region Properties

        public Color Color 
        {
            get => Model.Color;
            set
            {
                Model.Color = value;
                OnPropertyChanged();
            }
        }

        public int Count
        {
            get => Model.Count;
            set
            {
                Model.Count = value; 
                OnPropertyChanged();
            }
        }

        public double Diameter 
        {
            get => Model.Diameter; 
            set
            {
                Model.Diameter = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ForceViewModel> Forces 
        {
            get => forces;
            set
            {
                forces = value;
                OnPropertyChanged();
            }
        }

        public double MaxHeight
        {
            get => Model.MaxHeight;
            set
            {
                Model.MaxHeight = value;
                OnPropertyChanged();
            }
        }

        public double MaxWidth 
        {
            get => Model.MaxWidth;
            set
            {
                Model.MaxWidth = value;
                OnPropertyChanged();
            }
        }

        public string Name 
        {
            get => Model.Name;
            set
            {
                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ParticleViewModel> Particles 
        {
            get => particles;
            set
            {
                particles = value;
                OnPropertyChanged();
            }
        }

        public double Radius 
        {
            get => Model.Radius;
            set
            {
                Model.Radius = value; 
                OnPropertyChanged();
            }
        }

        public Action<AtomViewModel> RemoveAtomAction { get; set; }

        public ICommand RemoveCommand =>
            removeCommand ?? (removeCommand = new RelayCommand(RemoveAtom));

        #endregion

        #region Constructors

        public AtomViewModel(Atom model)
        {
            Model = model;

            Forces = new ObservableCollection<ForceViewModel>(model.Forces.Select(x => new ForceViewModel(x, this)));
            Particles = new ObservableCollection<ParticleViewModel>(model.Particles.Select(x => new ParticleViewModel(x)));

            Forces.CollectionChanged += Forces_CollectionChanged;
            Particles.CollectionChanged += Particles_CollectionChanged;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Forces.CollectionChanged -= Forces_CollectionChanged;
            Particles.CollectionChanged -= Particles_CollectionChanged;
        }

        private void Forces_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) 
            {
                ForceViewModel newForce = (ForceViewModel)e.NewItems[0];

                Model.Forces.Add(newForce.Model);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ForceViewModel oldForce = (ForceViewModel)e.OldItems[0];

                Model.Forces.Remove(oldForce.Model);
            }
        }

        private void Particles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                ParticleViewModel newParticle = (ParticleViewModel)e.NewItems[0];

                Model.Particles.Add(newParticle.Model);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                ParticleViewModel oldParticle = (ParticleViewModel)e.OldItems[0];

                Model.Particles.Remove(oldParticle.Model);
            }
        }

        private void RemoveAtom()
        {
            RemoveAtomAction?.Invoke(this);
        }

        #endregion
    }
}

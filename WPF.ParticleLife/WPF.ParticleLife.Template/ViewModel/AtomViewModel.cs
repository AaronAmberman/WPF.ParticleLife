using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using WPF.ParticleLife.Template.Model;

namespace WPF.ParticleLife.Template.ViewModel
{
    internal class AtomViewModel : ViewModelBase<Atom>, IDisposable
    {
        #region Fields

        private ObservableCollection<ForceViewModel> forces;
        private System.Windows.Media.Color mediaColor;
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

        public SolidBrush ColorBrush 
        {
            get => Model.ColorBrush;
            set
            {
                Model.ColorBrush = value;
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

        public System.Windows.Media.Color MediaColor
        {
            get => mediaColor;
            set
            {
                mediaColor = value;
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

        public ICommand RemoveCommand =>
            removeCommand ?? (removeCommand = new RelayCommand(RemoveAtom));

        public List<Particle> Particles
        {
            get => Model.Particles;
            set
            {
                Model.Particles = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Events

        public event EventHandler Remove;

        #endregion

        #region Constructors

        public AtomViewModel(Atom model)
        {
            Model = model;

            Forces = new ObservableCollection<ForceViewModel>(model.Forces.Select(x => new ForceViewModel(x, this)));

            Forces.CollectionChanged += Forces_CollectionChanged;
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Forces.CollectionChanged -= Forces_CollectionChanged;
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

        private void RemoveAtom()
        {
            Remove?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

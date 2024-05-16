using WPF.ParticleLife.Ellipses.Models;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace WPF.ParticleLife.Ellipses.ViewModels
{
    public class UniverseViewModel : ViewModelBase<Universe>, IDisposable
    {
        #region Fields

        private ObservableCollection<AtomViewModel> atoms;

        #endregion

        #region Properties

        public ObservableCollection<AtomViewModel> Atoms 
        { 
            get => atoms;
            set
            {
                atoms = value;
                OnPropertyChanged();
            }
        }

        public double Friction
        {
            get => Model.Friction;
            set
            {
                Model.Friction = value;
                OnPropertyChanged();
            }
        }

        public Size Size
        {
            get => Model.Size;
            set
            {
                Model.Size = value;
                OnPropertyChanged();
            }
        }

        public bool Wrap 
        {
            get => Model.Wrap;
            set
            {
                Model.Wrap = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public UniverseViewModel(Universe model)
        {
            Model = model;

            Atoms = new ObservableCollection<AtomViewModel>(model.Atoms.Select(x => new AtomViewModel(x)));
            Atoms.CollectionChanged += Atoms_CollectionChanged;
        }

        #endregion

        #region Methods

        private void Atoms_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                AtomViewModel newAtom = (AtomViewModel)e.NewItems[0];

                Model.Atoms.Add(newAtom.Model);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                AtomViewModel oldAtom = (AtomViewModel)e.OldItems[0];

                Model.Atoms.Remove(oldAtom.Model);
            }
        }

        public void Dispose()
        {
            Atoms.CollectionChanged -= Atoms_CollectionChanged;

            foreach (AtomViewModel atom in Atoms)
            {
                atom.Dispose();
            }
        }

        #endregion
    }
}

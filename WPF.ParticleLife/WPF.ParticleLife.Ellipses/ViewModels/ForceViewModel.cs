using WPF.ParticleLife.Ellipses.Models;

namespace WPF.ParticleLife.Ellipses.ViewModels
{
    public class ForceViewModel : ViewModelBase<Force>
    {
        #region Properties

        public double Attraction
        {
            get => Model.Attraction;
            set
            {
                Model.Attraction = value;
                OnPropertyChanged();
            }
        }

        public AtomViewModel Target { get; }

        #endregion

        #region Constructors

        public ForceViewModel(Force model, AtomViewModel target)
        {
            Model = model;
            Target = target;
        }

        #endregion
    }
}

using WPF.ParticleLife.Ellipses.Models;
using System.Windows.Shapes;

namespace WPF.ParticleLife.Ellipses.ViewModels
{
    public class ParticleViewModel : ViewModelBase<Particle>
    {
        #region Fields

        private Ellipse ellipse;

        #endregion

        #region Properties

        public Ellipse Ellipse 
        { 
            get => ellipse;
            set
            {
                ellipse = value;
                OnPropertyChanged();
            }
        }

        public double VelocityX 
        {
            get => Model.VelocityX;
            set
            {
                Model.VelocityX = value;
                OnPropertyChanged();
            }
        }

        public double VelocityY 
        {
            get => Model.VelocityY;
            set
            {
                Model.VelocityY = value;
                OnPropertyChanged();
            }
        }

        public double X 
        {
            get => Model.X;
            set
            {
                Model.X = value;
                OnPropertyChanged();
            }
        }

        public double Y 
        {
            get => Model.Y;
            set
            {
                Model.Y = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Constructors

        public ParticleViewModel(Particle model)
        {
            Model = model;
        }

        #endregion
    }
}

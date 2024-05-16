using WPF.ParticleLife.Graphics.Models;
using System.Windows.Shapes;

namespace WPF.ParticleLife.Graphics.ViewModels
{
    public class ParticleViewModel : ViewModelBase<Particle>
    {
        #region Properties

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

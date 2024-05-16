using System.Windows.Input;
using System.Windows.Media;

namespace WPF.ParticleLife.Graphics.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        #region Fields

        private Color backgroundColor = (Color)ColorConverter.ConvertFromString("#FF1F1F1F");
        public System.Drawing.Color backgroundColorDrawing = System.Drawing.Color.FromName("#1F1F1F");
        private ICommand backgroundCommand;
        private Color borderColor = Colors.LightGray;
        public System.Drawing.Color borderColorDrawing = System.Drawing.Color.LightGray;
        private ICommand borderCommand;
        private double forceRangeMax = 100.0;
        private Color foregroundColor = Colors.White;
        private ICommand foregroundCommand;
        private double friction = 0.5;
        private int particleCount = 100;
        private ICommand resetParticlesCommand;
        private bool wrap = true;

        #endregion

        #region Properties

        public System.Drawing.Color BackgroundColorDrawing 
        {
            get { return backgroundColorDrawing; }
            set
            {
                backgroundColorDrawing = value;
                OnPropertyChanged();
            }
        }

        public Color BackgroundColor
        {
            get { return backgroundColor; }
            set
            {
                backgroundColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackgroundCommand => 
            backgroundCommand ?? (backgroundCommand = new RelayCommand(BackgroundDialog));

        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                OnPropertyChanged();
            }
        }

        public System.Drawing.Color BorderColorDrawing
        {
            get { return borderColorDrawing; }
            set
            {
                borderColorDrawing = value;
                OnPropertyChanged();
            }
        }

        public ICommand BorderCommand =>
            borderCommand ?? (borderCommand = new RelayCommand(BorderDialog));

        public double ForceRangeMax
        {
            get { return forceRangeMax; }
            set
            {
                forceRangeMax = value;
                OnPropertyChanged();
            }
        }

        public Color ForegroundColor
        {
            get { return foregroundColor; }
            set
            {
                foregroundColor = value;
                OnPropertyChanged();
            }
        }

        public ICommand ForegroundCommand =>
            foregroundCommand ?? (foregroundCommand = new RelayCommand(ForegroundDialog));

        public double Friction
        {
            get { return friction; }
            set 
            {
                friction = value;
                OnPropertyChanged();
            }
        }

        public int ParticleCount
        {
            get { return particleCount; }
            set
            {
                particleCount = value;
                OnPropertyChanged();
            }
        }

        public ICommand ResetParticlesCommand => 
            resetParticlesCommand ?? (resetParticlesCommand = new RelayCommand(ResetParticles));

        public bool Wrap
        {
            get { return wrap; }
            set 
            {
                wrap = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        private void BackgroundDialog()
        {
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog();
            colorPickerDialog.Background = new SolidColorBrush(BackgroundColor);
            colorPickerDialog.Foreground = new SolidColorBrush(ForegroundColor);
            colorPickerDialog.PreviousColor = BackgroundColor;
            colorPickerDialog.SelectedColor = BackgroundColor;
            colorPickerDialog.ShowDialog();

            if (colorPickerDialog.DialogResult.HasValue)
            {
                if (colorPickerDialog.DialogResult.Value)
                {
                    BackgroundColor = colorPickerDialog.SelectedColor;
                    BackgroundColorDrawing = colorPickerDialog.SelectedColor.ToDrawingColor();
                }
            }
        }

        private void BorderDialog()
        {
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog();
            colorPickerDialog.Background = new SolidColorBrush(BackgroundColor);
            colorPickerDialog.Foreground = new SolidColorBrush(ForegroundColor);
            colorPickerDialog.PreviousColor = BorderColor;
            colorPickerDialog.SelectedColor = BorderColor;
            colorPickerDialog.ShowDialog();

            if (colorPickerDialog.DialogResult.HasValue)
            {
                if (colorPickerDialog.DialogResult.Value)
                {
                    BorderColor = colorPickerDialog.SelectedColor;
                    BorderColorDrawing = colorPickerDialog.SelectedColor.ToDrawingColor();
                }
            }
        }

        private void ForegroundDialog()
        {
            ColorPickerDialog colorPickerDialog = new ColorPickerDialog();
            colorPickerDialog.Background = new SolidColorBrush(BackgroundColor);
            colorPickerDialog.Foreground = new SolidColorBrush(ForegroundColor);
            colorPickerDialog.PreviousColor = ForegroundColor;
            colorPickerDialog.SelectedColor = ForegroundColor;
            colorPickerDialog.ShowDialog();

            if (colorPickerDialog.DialogResult.HasValue)
            {
                if (colorPickerDialog.DialogResult.Value)
                {
                    ForegroundColor = colorPickerDialog.SelectedColor;
                }
            }
        }

        private void ResetParticles()
        {

        }

        #endregion
    }
}

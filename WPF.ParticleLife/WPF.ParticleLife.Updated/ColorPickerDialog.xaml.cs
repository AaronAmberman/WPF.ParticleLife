using System.Windows;
using System.Windows.Media;

namespace WPF.ParticleLife.Updated
{
    public partial class ColorPickerDialog : Window
    {
        #region Properties


        public Color PreviousColor
        {
            get { return (Color)GetValue(PreviousColorProperty); }
            set { SetValue(PreviousColorProperty, value); }
        }

        public static readonly DependencyProperty PreviousColorProperty =
            DependencyProperty.Register("PreviousColor", typeof(Color), typeof(ColorPickerDialog), new PropertyMetadata(Colors.Transparent));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerDialog), new PropertyMetadata(Colors.Transparent));

        #endregion

        #region Constructors

        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        #endregion
    }
}

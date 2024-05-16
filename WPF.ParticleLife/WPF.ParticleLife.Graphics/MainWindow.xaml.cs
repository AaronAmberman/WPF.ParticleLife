using WPF.ParticleLife.Graphics.Models;
using WPF.ParticleLife.Graphics.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WPF.ParticleLife.Graphics
{
    // https://github.com/BlinkSun/ParticleLifeSimulation

    public partial class MainWindow : Window
    {
        #region Fields

        private bool isLoaded;
        private MainWindowViewModel viewModel;

        #endregion

        #region Constructors

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            viewModel.SettingsViewModel = new SettingsViewModel();
            viewModel.UniverseViewModel = new UniverseViewModel(new Universe());

            DataContext = viewModel;

            InitializeComponent();

            Render.RenderFrame += Render_RenderFrame;
        }

        #endregion

        #region Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            viewModel.UniverseViewModel.Friction = viewModel.SettingsViewModel.Friction;
            viewModel.UniverseViewModel.Size = new Size(imageBorder.ActualWidth, imageBorder.ActualHeight);
            viewModel.UniverseViewModel.Wrap = viewModel.SettingsViewModel.Wrap;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            viewModel.Dispose();
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isLoaded) return;

            viewModel.UniverseViewModel.Size = new Size(imageBorder.ActualWidth, imageBorder.ActualHeight);
        }

        private void Render_RenderFrame(object sender, RenderingEventArgs e)
        {
            viewModel.UpdateRenderEnvironment();
        }

        #endregion
    }
}

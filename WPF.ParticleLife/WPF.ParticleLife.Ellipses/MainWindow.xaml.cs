using WPF.ParticleLife.Ellipses.Models;
using WPF.ParticleLife.Ellipses.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WPF.ParticleLife.Ellipses
{
    // https://github.com/BlinkSun/WPF.ParticleLife.EllipsesSimulation

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

            viewModel.Canvas = universeCanvas;
            viewModel.Dispatcher = Dispatcher;

            Render.RenderFrame += Render_RenderFrame;
        }

        #endregion

        #region Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            viewModel.UniverseViewModel.Friction = viewModel.SettingsViewModel.Friction;
            viewModel.UniverseViewModel.Size = new Size(universeCanvas.ActualWidth, universeCanvas.ActualHeight);
            viewModel.UniverseViewModel.Wrap = viewModel.SettingsViewModel.Wrap;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            viewModel.Dispose();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!isLoaded) return;

            viewModel.UniverseViewModel.Size = new Size(universeCanvas.ActualWidth, universeCanvas.ActualHeight);
        }

        private void Render_RenderFrame(object sender, RenderingEventArgs e)
        {
            viewModel.UpdateRenderEnvironment();
        }

        #endregion
    }
}

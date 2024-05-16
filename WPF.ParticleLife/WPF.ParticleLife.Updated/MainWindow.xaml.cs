using System;
using System.ComponentModel;
using System.Windows;
using WPF.ParticleLife.Updated.Model;
using WPF.ParticleLife.Updated.ViewModel;

namespace WPF.ParticleLife.Updated
{
    public partial class MainWindow : Window
    {
        #region Fields

        private bool isLoaded;
        private DelayedSingleActionInvoker sizeChangedDelayedActionInvoker;
        private MainWindowViewModel viewModel;

        #endregion

        #region Constructors

        public MainWindow()
        {
            viewModel = new MainWindowViewModel();
            viewModel.UniverseViewModel = new UniverseViewModel(new Universe());

            DataContext = viewModel;

            InitializeComponent();

            sizeChangedDelayedActionInvoker = new DelayedSingleActionInvoker(ResizeUniverse, TimeSpan.FromSeconds(1.5));
        }

        #endregion

        #region Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;

            viewModel.UniverseViewModel.Height = imageBorder.ActualHeight;
            viewModel.UniverseViewModel.Width = imageBorder.ActualWidth;

            viewModel.InitializeRenderer();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            viewModel.StopRenderingCycle();
            viewModel.Dispose();
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            sizeChangedDelayedActionInvoker.BeginInvoke();
        }

        private void ResizeUniverse()
        {
            if (!isLoaded) return;

            viewModel.StopRenderingCycle();

            viewModel.UniverseViewModel.Height = imageBorder.ActualHeight;
            viewModel.UniverseViewModel.Width = imageBorder.ActualWidth;

            viewModel.InitializeRenderer();
        }

        #endregion
    }
}

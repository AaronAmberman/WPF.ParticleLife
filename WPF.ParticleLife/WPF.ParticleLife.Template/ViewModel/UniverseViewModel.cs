using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using WPF.ParticleLife.Template.Model;

namespace WPF.ParticleLife.Template.ViewModel
{
    internal class UniverseViewModel : ViewModelBase<Universe>, IDisposable
    {
        #region Fields

        private ObservableCollection<AtomViewModel> atoms;
        private Color backgroundColor = (Color)ColorConverter.ConvertFromString("#FF1F1F1F");
        private System.Drawing.Color backgroundColorDrawing = System.Drawing.Color.FromName("#1F1F1F");
        private ICommand backgroundCommand;
        private Color borderColor = Colors.LightGray;
        private System.Drawing.Color borderColorDrawing = System.Drawing.Color.LightGray;
        private System.Drawing.Pen borderPen = System.Drawing.Pens.LightGray;
        private ICommand borderCommand;
        private double diameter;
        private Color foregroundColor = Colors.White;
        private ICommand foregroundCommand;


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

        public System.Drawing.Pen BorderPen
        {
            get { return borderPen; }
            set
            {
                borderPen = value;
                OnPropertyChanged();
            }
        }

        public ICommand BorderCommand =>
            borderCommand ?? (borderCommand = new RelayCommand(BorderDialog));

        public double Diameter 
        {
            get
            {
                if (diameter == 0)
                    diameter = Model.Radius * 2;

                return diameter;
            }
            set
            {
                diameter = value;
                OnPropertyChanged();
            }
        }

        public double ForceRange
        {
            get => Model.ForceRange;
            set
            {
                Model.ForceRange = value;
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
            get => Model.Friction;
            set
            {
                Model.Friction = value;
                OnPropertyChanged();
            }
        }

        public double Height
        {
            get => Model.Height;
            set
            {
                Model.Height = value;
                OnPropertyChanged();
            }
        }

        public double MaxVelocity
        {
            get => Model.MaxVelocity;
            set
            {
                Model.MaxVelocity = value;
                OnPropertyChanged();
            }
        }

        public int ParticleCount
        {
            get => Model.ParticleCount;
            set
            {
                Model.ParticleCount = value;
                OnPropertyChanged();
            }
        }

        public double ParticleRange
        {
            get => Model.ParticleRange;
            set
            {
                Model.ParticleRange = value;
                OnPropertyChanged();
            }
        }

        public double Radius
        {
            get => Model.Radius;
            set
            {
                Model.Radius = value;

                Diameter = value * 2;

                OnPropertyChanged();
            }
        }

        public double TimeFactor
        {
            get => Model.TimeFactor;
            set
            {
                Model.TimeFactor = value;
                OnPropertyChanged();
            }
        }

        public double Width
        {
            get => Model.Width;
            set
            {
                Model.Width = value;
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
                    BorderPen = new System.Drawing.Pen(BorderColorDrawing, 1);
                }
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

        #endregion
    }
}

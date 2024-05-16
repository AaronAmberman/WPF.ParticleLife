namespace WPF.ParticleLife.Updated.Model
{
    internal class Force
    {
        #region Fields

        private double attraction;

        #endregion

        #region Properties

        public double Attraction
        {
            get => attraction;
            set
            {
                attraction = value > 1.0 ? 1.0 : value < -1.0 ? -1.0 : value;
            }
        }

        public Atom Target { get; }

        #endregion

        #region Constructors

        public Force(Atom target)
        {
            Target = target;
        }

        #endregion
    }
}

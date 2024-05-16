using System;

namespace WPF.ParticleLife.Ellipses.Models
{
    public class Force
    {
        #region Constants

        public const double MIN_VALUE = -1.0;
        public const double MAX_VALUE = 1.0;

        #endregion

        #region Fields

        private double attraction;

        #endregion

        #region Properties

        public double Attraction
        {
            get => attraction;
            set
            {
                if (attraction != Math.Clamp(value, MIN_VALUE, MAX_VALUE))
                {
                    attraction = Math.Clamp(value, MIN_VALUE, MAX_VALUE);
                }
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

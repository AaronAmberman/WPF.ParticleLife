using System;

namespace WPF.ParticleLife.Template
{
    internal class NumberHelper
    {
        #region Methods

        public static double RandomDouble(double min, double max)
        {
            return Random.Shared.NextDouble() * (max - min) + min;
        }

        public static double RandomDouble0To1()
        {
            return RandomDouble(0.0, 1.0);
        }

        public static double RandomDoubleNegative1To1()
        {
            return RandomDouble(-1.0, 1.0);
        }

        #endregion
    }
}

using System.Collections.Generic;

namespace WPF.ParticleLife.Template.Model
{
    internal class Universe
    {
        #region Properties

        public List<Atom> Atoms { get; set; } = new List<Atom>();

        public int ParticleCount { get; set; } = 100;

        public double ForceRange { get; set; } = 100.0;

        public double Friction { get; set; } = 0.5;

        public double Height { get; set; }

        public double MaxVelocity { get; set; } = 100.0;

        public double ParticleRange { get; set; } = 250.0;

        public double Radius { get; set; } = 3.0;

        public double TimeFactor { get; set; } = 1.0;

        public double Width { get; set; }

        public bool Wrap { get; set; } = true;

        #endregion
    }
}

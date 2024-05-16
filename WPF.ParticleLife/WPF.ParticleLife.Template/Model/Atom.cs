using System.Collections.Generic;
using System.Drawing;

namespace WPF.ParticleLife.Template.Model
{
    internal class Atom
    {
        #region Properties

        public Color Color { get; set; }

        public SolidBrush ColorBrush { get; set; }

        public List<Force> Forces { get; set; } = new List<Force>();

        public string Name { get; set; }

        public List<Particle> Particles { get; set; } = new List<Particle>();

        #endregion
    }
}

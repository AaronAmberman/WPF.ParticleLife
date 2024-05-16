using System.Collections.Generic;
using System.Windows;

namespace WPF.ParticleLife.Graphics.Models
{
    public class Universe
    {
        #region Properties

        public List<Atom> Atoms { get; set; } = new List<Atom>();

        public double Friction { get; set; }

        public Size Size { get; set; }

        public bool Wrap { get; set; } = true;

        #endregion
    }
}

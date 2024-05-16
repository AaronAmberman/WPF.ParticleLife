using System.Collections.Generic;
using System.Windows.Media;

namespace WPF.ParticleLife.Graphics.Models
{
    public class Atom
    {
        #region Properties

        public Color Color { get; set; }

        public int Count { get; set; }

        public double Diameter { get; set; }

        public List<Force> Forces { get; set; } = new List<Force>();

        public double MaxHeight { get; set; }

        public double MaxWidth { get; set; }

        public string Name { get; set; }

        public List<Particle> Particles { get; set; } = new List<Particle>();

        public double Radius { get; set; }

        #endregion

        #region Constructors

        public Atom()
        {            
        }

        public Atom(string name, Color color, double maxWidth, double maxHeight, double radius)
        {
            Name = name;
            Color = color;
            MaxHeight = maxHeight;
            MaxWidth = maxWidth;
            Radius = radius;
            Diameter = radius * 2.0;
        }

        #endregion
    }
}

﻿namespace WPF.ParticleLife.Graphics
{
    public static class ExtensionMethods
    {
        public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color color) 
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}

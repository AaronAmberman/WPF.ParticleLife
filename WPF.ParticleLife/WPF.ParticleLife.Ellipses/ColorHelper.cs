using WPF.ParticleLife.Ellipses.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Media;

namespace WPF.ParticleLife.Ellipses
{
    public class ColorHelper
    {
        #region Properties

        public static List<MediaColor> MediaColors { get; set; }

        #endregion

        #region Constructors

        static ColorHelper()
        {
            MediaColors = GetMediaColors();
        }

        #endregion

        #region Methods

        public static List<MediaColor> GetMediaColors()
        {
            List<PropertyInfo> colorProperties = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public).ToList();
            List<string> propertyNames = colorProperties.Select(x => x.Name).ToList();

            List<MediaColor> result = new List<MediaColor>();

            for (int i = 0; i < propertyNames.Count; i++) 
            {
                // exclude black, white and transparent
                if (propertyNames[i] == "Black" || propertyNames[i] == "White" || propertyNames[i] == "Transparent") continue;

                result.Add(new MediaColor { Name = propertyNames[i], Color = (Color)colorProperties[i].GetValue(null) });
            }

            return result;
        }

        public static MediaColor GetRandomMediaColor()
        {
            return MediaColors.ElementAt(Random.Shared.Next(MediaColors.Count));
        }

        #endregion
    }
}

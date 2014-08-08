using DraftEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DraftClient.Converters
{
    class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((PlayerPosition)value)
            {
                case PlayerPosition.QB:
                    return "Thistle";
                case PlayerPosition.RB:
                    return "Aquamarine";
                case PlayerPosition.WR:
                    return "IndianRed";
                case PlayerPosition.TE:
                    return "PaleGoldenrod";
                case PlayerPosition.K:
                    return "Pink";
                case PlayerPosition.DEF:
                    return "LightGreen";
            }
            return "AntiqueWhite";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

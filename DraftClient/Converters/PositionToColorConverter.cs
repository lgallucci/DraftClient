namespace DraftEntities.Converters
{
    using DraftEntities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Data;

    class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch ((PlayerPosition)value)
            {
                case PlayerPosition.QB:
                    return "SlateBlue";
                case PlayerPosition.RB:
                    return "DarkBlue";
                case PlayerPosition.WR:
                    return "DarkRed";
                case PlayerPosition.TE:
                    return "DarkGoldenrod";
                case PlayerPosition.K:
                    return "MediumOrchid";
                case PlayerPosition.DEF:
                    return "DarkOliveGreen";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

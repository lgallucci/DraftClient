namespace DraftClient.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using DraftEntities;

    class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

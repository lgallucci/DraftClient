﻿namespace DraftClient.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using DraftEntities;

    internal class PositionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((PlayerPosition) value)
            {
                case PlayerPosition.QB:
                    return "#FFFCA910";
                case PlayerPosition.RB:
                    return "#FF80D5EF";
                case PlayerPosition.WR:
                    return "#FFF14D0F";
                case PlayerPosition.TE:
                    return "#FFFFF889";
                case PlayerPosition.K:
                    return "#FFBB88FF";
                case PlayerPosition.DEF:
                    return "#FFAAFF55";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace SISPSLocal.Chessboard
{
    public class ChessPieceConverter : IValueConverter
    {
        private Dictionary<string, string> FenToResource = new Dictionary<string, string>
        {
            { "K", "WK" },
            { "Q", "WQ" },
            { "R", "WR" },
            { "B", "WB" },
            { "N", "WN" },
            { "P", "WP" },
            { "k", "BK" },
            { "q", "BQ" },
            { "r", "BR" },
            { "b", "BB" },
            { "n", "BN" },
            { "p", "BP" },
            { " ", null }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var resName = FenToResource[value.ToString()];
            if (resName == null) return null;
            return Application.Current.TryFindResource(resName);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

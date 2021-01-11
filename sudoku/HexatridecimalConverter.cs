using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Sudoku.Converters {
    public class HexatridecimalConverter : IValueConverter {
        private static readonly char[] hexatridecimalDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(value is null) {
                return null;
            }

            int n;
            if(value is int) {
                n = (int) value;
            } else {
                throw new ArgumentException();
            }

            if(n < hexatridecimalDigits.Count()) {
                return hexatridecimalDigits[n];
            }

            throw new ArgumentOutOfRangeException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(value is null) {
                return null;
            }

            char c;
            if(value is char) {
                c = (char) value;
            } else {
                throw new ArgumentException();
            }

            int n = Array.IndexOf(hexatridecimalDigits, c);
            if(n < 0) {
                throw new ArgumentOutOfRangeException();
            } else {
                return n;
            }
        }
    }
}

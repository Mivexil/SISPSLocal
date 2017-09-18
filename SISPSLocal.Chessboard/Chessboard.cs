using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SISPSLocal.Chessboard
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SISPSLocal.Chessboard"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SISPSLocal.Chessboard;assembly=SISPSLocal.Chessboard"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class Chessboard : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static Chessboard()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chessboard), new FrameworkPropertyMetadata(typeof(Chessboard)));
        }

        public Chessboard()
        {
            Squares = new List<ChessSquare>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Squares.Add(new ChessSquare() { Row = i, Column = j });
                }
            }
            FillSquares(FenNotation);
        }

        public static readonly DependencyProperty FenNotationProperty = DependencyProperty.Register(nameof(FenNotation), typeof(string), typeof(Chessboard), 
            new FrameworkPropertyMetadata("8/8/8/8/8/8/8/8", new PropertyChangedCallback(OnFenNotationChanged)), new ValidateValueCallback(IsValidFen));

        private static void OnFenNotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Chessboard)d).FillSquares(e.NewValue?.ToString());
        }

        private static bool IsValidFen(object value)
        {
            var casted = value as string;
            if (casted == null)
            {
                return false;
            }
            var canonical = FenToCanonical(casted);
            var result = canonical != null && canonical.Count == 8 && canonical.All(x => x != null && x.Length == 8 && x.All(ch => "KQRBNPkqrbnp ".Contains(ch)));
            return result;
        }

        public string FenNotation
        {
            get { return (string)GetValue(FenNotationProperty); }
            set { SetValue(FenNotationProperty, value); }
        }

        public static readonly DependencyProperty SquaresProperty = DependencyProperty.Register(nameof(Squares), typeof(List<ChessSquare>), typeof(Chessboard));

        public List<ChessSquare> Squares
        {
            get { return (List<ChessSquare>)GetValue(SquaresProperty); }
            set { SetValue(SquaresProperty, value); }
        }    
        
        private static List<string> FenToCanonical(string fen)
        {
            var cleanedFen = fen.Split(' ')[0];
            var fenLines = cleanedFen.Split('/');
            var substitutedFen = new List<string>();
            foreach (var line in fenLines)
            {
                string l = line;
                for (int i = 0; i <= 8; i++)
                {
                    l = l.Replace(i.ToString(), new string(' ', i));
                }
                substitutedFen.Add(l);
            }
            return substitutedFen;
        }

        private void FillSquares(string fen)
        {
            var substitutedFen = FenToCanonical(fen);            
            for (int i = 0; i < 64; i++)
            {
                Squares[i].ChessPiece = substitutedFen[i / 8][i % 8].ToString();
            }
        }
    }
}

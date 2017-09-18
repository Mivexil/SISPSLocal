using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Chessboard
{
    public class ChessSquare : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
                
        public int Row { get; set; }

        public int Column { get; set; }

        public bool IsBlack { get { return (Row + Column) % 2 == 1; } }

        private string _chessPiece;
        public string ChessPiece
        {
            get { return _chessPiece; }
            set
            {
                if (_chessPiece != value)
                {
                    _chessPiece = value;
                    NotifyPropertyChanged(nameof(ChessPiece));
                }
            }
        }
    }
}

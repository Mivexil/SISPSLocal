using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPSLocal.Classes
{
    public struct ChessSquarePosition
    {
        public int Rank { get; }
        public int File { get; }
        public int Index
        {
            get
            {
                return ((8 - Rank) * 8) + (File - 1);
            }
        }

        public bool IsBlack
        {
            get
            {
                return ((Rank + File) % 2) == 0;
            }
        }

        public ChessSquarePosition(int index) : this()
        {
            this.Rank = 8 - (index / 8);
            this.File = (index % 8) + 1;
        }

        public ChessSquarePosition(int rank, int file) : this()
        {
            this.Rank = rank;
            this.File = file;
        }

        public ChessSquarePosition(string notation) : this()
        {
            if (notation == null) throw new ArgumentNullException(nameof(notation));
            if (notation.Length != 2) throw new ArgumentException($"Invalid square specifier: {notation}");
            var fileLetter = notation[0];
            var rankNumber = notation[1];
            if (fileLetter >= 'a' && fileLetter <= 'h')
            {
                File = fileLetter - 'a' + 1;
            }
            else if (fileLetter >= 'A' && fileLetter <= 'H')
            {
                File = fileLetter - 'A' + 1;
            }
            else throw new ArgumentException($"Invalid square specifier: {notation}");
            if (rankNumber >= '1' && rankNumber <= '8')
            {
                Rank = rankNumber - '1' + 1;
            }
            else throw new ArgumentException($"Invalid square specifier: {notation}");
        }

        public override bool Equals(object obj)
        {
            return obj is ChessSquarePosition 
                && ((ChessSquarePosition)obj).Rank == Rank 
                && ((ChessSquarePosition)obj).File == File;
        }

        public static bool operator ==(ChessSquarePosition left, ChessSquarePosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ChessSquarePosition left, ChessSquarePosition right)
        {
            return !left.Equals(right);
        }

        public override int GetHashCode()
        {
            return Rank * 17 + File;
        }

        public override string ToString()
        {
            return $"{ (char)(File - 1 + 'a')}{Rank}";
        }
    }
}

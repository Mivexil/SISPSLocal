using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SISPSLocal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _fenNotation;
        public string FenNotation
        {
            get { return _fenNotation; }
            set
            {
                if (value != _fenNotation)
                {
                    _fenNotation = value;
                    NotifyPropertyChanged(nameof(FenNotation));
                }
            }
        }

        public string ChessGame
        {
            get
            {
                return @"[Event ""Test Match Name""]
[Site ""Test Match Site""]
[Date ""1970.01.01""]
[Round ""1""]
[White ""White Player""]
[Black ""Black Player""]
[Result ""1/2-1/2""]

" + @"1. e4 e5 {standard opening} 2. Bc4 (2. d4) Nc6 3. Qh5 Nf6 4. Qxf7 1-0";
            }
        }
    }
}

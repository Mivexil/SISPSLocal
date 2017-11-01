using SISPSLocal.Classes;
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

namespace SISPSLocal.GameGrid
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SISPSLocal.GameGrid"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:SISPSLocal.GameGrid;assembly=SISPSLocal.GameGrid"
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
    public class GameGrid : Control, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ChessGameFactory _gameFactory = new ChessGameFactory();

        public static readonly DependencyProperty GameProperty = DependencyProperty.Register(nameof(Game), 
            typeof(ChessGame), 
            typeof(GameGrid),
            new FrameworkPropertyMetadata(new ChessGame(), new PropertyChangedCallback(OnGameChanged)));

        private static void OnGameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GameGrid)d).Moves = ((ChessGame)e.NewValue).Moves;
        }

        public ChessGame Game
        {
            get { return (ChessGame)GetValue(GameProperty); }
            set { SetValue(GameProperty, value); }
        }

        public static readonly DependencyProperty GameStringProperty = DependencyProperty.Register(nameof(GameString), 
            typeof(string), 
            typeof(GameGrid),
            new FrameworkPropertyMetadata("", new PropertyChangedCallback(OnGameStringChanged)));

        public static readonly DependencyProperty MovesProperty = DependencyProperty.Register(nameof(Moves), 
            typeof(IReadOnlyCollection<ChessMove>), 
            typeof(GameGrid));

        public IReadOnlyCollection<ChessMove> Moves
        {
            get { return (IReadOnlyCollection<ChessMove>)GetValue(MovesProperty); }
            set { SetValue(MovesProperty, value); }
        }

        private static void OnGameStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GameGrid)d).Game = ((GameGrid)d)._gameFactory.GetChessGameFromString((string)e.NewValue);            
        }

        public string GameString
        {
            get { return (string)GetValue(GameStringProperty); }
            set { SetValue(GameStringProperty, value); }
        }

        public int GridRows
        {
            get { return (Game.Moves.Count / 2) + (Game.Moves.Count % 2); }
        }

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        static GameGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GameGrid), new FrameworkPropertyMetadata(typeof(GameGrid)));
        }
    }
}

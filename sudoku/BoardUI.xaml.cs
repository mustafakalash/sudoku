using System;
using System.Collections.Generic;
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

namespace Sudoku {
    /// <summary>
    /// Interaction logic for BoardUI.xaml
    /// </summary>
    public partial class BoardUI : UserControl {
        public BoardUI() {
            App.GameBoard = new Board(9);
            InitializeComponent();
            MainList.DataContext = App.GameBoard;
            App.GameBoard.GenerateGame();
        }
    }
}

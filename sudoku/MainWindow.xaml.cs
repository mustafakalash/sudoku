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

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static int BoardSize;
        public static bool AutoNotes;
        public static int SolveSpeed;

        public MainWindow() {
            InitializeComponent();
            ControlPanel.DataContext = BoardUI.GameBoard;
        }

        public void SetSize(object sender, RoutedEventArgs e) {
            int.TryParse((string) ((RadioButton) sender).Tag, out BoardSize);
        }

        public void SetAutoNotes(object sender, RoutedEventArgs e) {
            AutoNotes = ((CheckBox) sender).IsChecked.HasValue && ((CheckBox) sender).IsChecked.Value;
        }

        public async void NewGame(object sender, RoutedEventArgs e) {
            LoadInstr.Visibility = Visibility.Collapsed;
            BoardUI.GameBoard.GenerateBoard(BoardSize);
            await BoardUI.GameBoard.GenerateGame();
        }
        public async void SolveBoard(object sender, RoutedEventArgs e) {
            BoardUI.GameBoard.Solving = true;
            await (new Solver()).Solve(BoardUI.GameBoard);
            BoardUI.GameBoard.Solving = false;
        }

        public void UpdateSolveSpeed(object sender, RoutedEventArgs e) {
            SolveSpeed = (int) ((Slider) sender).Value;
        }

    }
}

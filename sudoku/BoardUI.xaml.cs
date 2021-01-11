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
        private static readonly Dictionary<Key, int?> numericKeys = new Dictionary<Key, int?> {
            {Key.D0, 0},
            {Key.D1, 1},
            {Key.D2, 2},
            {Key.D3, 3},
            {Key.D4, 4},
            {Key.D5, 5},
            {Key.D6, 6},
            {Key.D7, 7},
            {Key.D8, 8},
            {Key.D9, 9},
            {Key.NumPad0, 0},
            {Key.NumPad1, 1},
            {Key.NumPad2, 2},
            {Key.NumPad3, 3},
            {Key.NumPad4, 4},
            {Key.NumPad5, 5},
            {Key.NumPad6, 6},
            {Key.NumPad7, 7},
            {Key.NumPad8, 8},
            {Key.NumPad9, 9},
            {Key.A, 10},
            {Key.B, 11},
            {Key.C, 12},
            {Key.D, 13},
            {Key.E, 14},
            {Key.F, 15},
            {Key.G, 16},
            {Key.Back, null}
        };

        public Board GameBoard;

        public BoardUI() {
            Loaded += generateGame;
            GameBoard = new Board(9);
            InitializeComponent();
            MainList.DataContext = GameBoard;
        }

        private async void generateGame(object sender, RoutedEventArgs e) {
            Loaded -= generateGame;
            await GameBoard.GenerateGame();
        }

        public void SelectCell(object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            Cell cell = (Cell) btn.DataContext;
            if(cell.ReadOnly) {
                return;
            }
            GameBoard.SelectedCell = cell;
        }

        public void InputNumber(object sender, KeyEventArgs e) {
            if(GameBoard.SelectedCell is null || GameBoard.SelectedCell.ReadOnly || !numericKeys.ContainsKey(e.Key)) {
                return;
            }

            int? n = numericKeys[e.Key];

            if(n.HasValue) {
                if(GameBoard.PossibleValues.Contains(n.Value)) {
                    if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {
                        if(GameBoard.SelectedCell.Notes.Contains(n.Value)) {
                            GameBoard.SelectedCell.Notes.Remove(n.Value);
                        } else {
                            GameBoard.SelectedCell.Notes.Add(n.Value);
                            GameBoard.SelectedCell.AutoRemovedNotes.Remove(n.Value);
                        }
                    } else {
                        if(GameBoard.SelectedCell.Number == n) {
                            GameBoard.SelectedCell.Number = null;
                        } else {
                            GameBoard.SelectedCell.Number = n;
                        }
                    }
                }
            } else {
                GameBoard.SelectedCell.Number = null;
            }
        }
    }
}

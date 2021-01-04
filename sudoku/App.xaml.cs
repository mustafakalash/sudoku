using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Board GameBoard;

        public void SelectCell(object sender, RoutedEventArgs e) {
            Button btn = (Button) sender;
            Cell cell = (Cell) btn.DataContext;
            if(cell.ReadOnly) {
                return;
            }
            GameBoard.SelectedCell = cell;
        }

        public void InputNumber(object sender, KeyEventArgs e) {
            if(GameBoard.SelectedCell.ReadOnly) {
                return;
            }

            if(e.Key == Key.Back) {
                GameBoard.SelectedCell.Number = null;
            }

            int? n = null;
            switch(e.Key) {
                case Key.D1:
                    n = 1;
                    break;
                case Key.D2:
                    n = 2;
                    break;
                case Key.D3:
                    n = 3;
                    break;
                case Key.D4:
                    n = 4;
                    break;
                case Key.D5:
                    n = 5;
                    break;
                case Key.D6:
                    n = 6;
                    break;
                case Key.D7:
                    n = 7;
                    break;
                case Key.D8:
                    n = 8;
                    break;
                case Key.D9:
                    n = 9;
                    break;
            }

            if(n.HasValue) {
                int number = (int) n;
                if(e.KeyboardDevice.Modifiers == ModifierKeys.Shift) {
                    if(GameBoard.SelectedCell.Notes.Contains(number)) {
                        GameBoard.SelectedCell.Notes.Remove(number);
                    } else {
                        GameBoard.SelectedCell.Notes.Add(number);
                    }
                } else {
                    GameBoard.SelectedCell.Number = number;
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sudoku {
    public class Grid : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<ObservableCollection<Cell>> _rows = new ObservableCollection<ObservableCollection<Cell>>();
        public ObservableCollection<ObservableCollection<Cell>> GridRows {
            get {
                return _rows;
            }
            set {
                _rows = value;
            }
        }

        public readonly int Row, Col;

        readonly Board _board;
        public Board Board {
            get {
                return _board;
            }
        }

        public Grid(Board board, int row, int col) {
            _board = board;
            Row = row;
            Col = col;

            for(int r = 0; r < board.Size; r++) {
                ObservableCollection<Cell> colL = new ObservableCollection<Cell>();
                for(int c = 0; c < board.Size; c++) {
                    Cell cell = new Cell(this, r, c);
                    cell.ReadOnly = true;
                    colL.Add(cell);
                }
                GridRows.Add(colL);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

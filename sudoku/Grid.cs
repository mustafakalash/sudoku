using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sudoku {
    public class Grid : INotifyPropertyChanged {
        readonly ObservableCollection<ObservableCollection<Cell>> rows;
        public ObservableCollection<ObservableCollection<Cell>> GridRows {
            get {
                return rows;
            }
        }

        public readonly int Row, Col;

        public readonly Board Board;

        public Grid(Board board, int row, int col) {
            Board = board;
            Row = row;
            Col = col;

            rows = new ObservableCollection<ObservableCollection<Cell>>();
            for(int r = 0; r < board.Size; r++) {
                ObservableCollection<Cell> colL = new ObservableCollection<Cell>();
                for(int c = 0; c < board.Size; c++) {
                    Cell cell = new Cell(this, r, c);
                    cell.PropertyChanged += new PropertyChangedEventHandler(cellPropertyChanged);
                    colL.Add(cell);
                }
                rows.Add(colL);
            }
        }

        void cellPropertyChanged(object sender, PropertyChangedEventArgs e) {
            PropertyChanged(sender, new PropertyChangedEventArgs(e.PropertyName));
        }
        
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

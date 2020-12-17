using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace sudoku {
    public class Board : INotifyPropertyChanged {
        ObservableCollection<ObservableCollection<Grid>> rows;
        public ObservableCollection<ObservableCollection<Grid>> BoardRows {
            get {
                return rows;
            }
        }

        bool isValidValue = true;
        public bool IsValid {
            get {
                return isValidValue;
            }
        }
        public Board(byte size) {
            size = (byte) Math.Sqrt(size);
            rows = new ObservableCollection<ObservableCollection<Grid>>();
            for(int row = 0; row < size; row++) {
                ObservableCollection<Grid> col = new ObservableCollection<Grid>();
                for(int c = 0; c < size; c++) {
                    Grid grid = new Grid(size);
                    grid.PropertyChanged += new PropertyChangedEventHandler(cellPropertyChanged);
                    col.Add(grid);
                }
                rows.Add(col);
            }
        }

        private bool checkIsValid() {
            return true;
        }

        void cellPropertyChanged(object sender, PropertyChangedEventArgs e) {
 
        }

        public Grid this[int row, int col] {
            get {
                if(row < 0 || row >= rows.Count) {
                    throw new ArgumentOutOfRangeException("row", row, "Invalid row index");
                }
                if(col < 0 || col >= rows.Count) {
                    throw new ArgumentOutOfRangeException("col", col, "Invalid column index");
                }
                return rows[row][col];
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

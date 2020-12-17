using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace sudoku {
    public class Grid : INotifyPropertyChanged {
        ObservableCollection<ObservableCollection<Cell>> rows;
        public ObservableCollection<ObservableCollection<Cell>> GridRows {
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
        public Grid(byte size) {
            rows = new ObservableCollection<ObservableCollection<Cell>>();
            for(int row = 0; row < size; row++) {
                ObservableCollection<Cell> col = new ObservableCollection<Cell>();
                for(int c = 0; c < size; c++) {
                    Cell cell = new Cell();
                    cell.PropertyChanged += new PropertyChangedEventHandler(cellPropertyChanged);
                    col.Add(cell);
                }
                rows.Add(col);
            }
        }

        private bool checkIsValid() {
            bool[] used = new bool[rows.Count * rows.Count];
            foreach(ObservableCollection<Cell> col in rows) {
                foreach(Cell cell in col) {
                    if(cell.Number.HasValue) {
                        if(used[cell.Number.Value - 1]) {
                            return false;
                        } else {
                            used[cell.Number.Value - 1] = true;
                        }
                    }
                }
            }
            return true;
        }

        void cellPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == "Number") {
                bool valid = checkIsValid();
                foreach(ObservableCollection<Cell> col in rows) {
                    foreach(Cell cell in col) {
                        cell.IsValid = valid;
                    }
                }

                isValidValue = valid;
                if(PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
                }
            }
        }
        
        public Cell this[int row, int col] {
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

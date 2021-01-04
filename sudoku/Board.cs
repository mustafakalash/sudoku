using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Sudoku {
    public class Board : INotifyPropertyChanged {
        ObservableCollection<ObservableCollection<Grid>> rows;
        public ObservableCollection<ObservableCollection<Grid>> BoardRows {
            get {
                return rows;
            }
        }

        public readonly int Size;
        public readonly int TotalSize;

        Cell selectedCellObject;
        public Cell SelectedCell {
            get {
                return selectedCellObject;
            }
            set {
                if(selectedCellObject != value) {
                    if(selectedCellObject != null) {
                        selectedCellObject.Selected = false;
                        foreach(Cell c in selectedCellObject.GetSibilngs()) {
                            c.SiblingSelected = false;
                        }
                    }
                    selectedCellObject = value;
                    if(selectedCellObject != null) {
                        selectedCellObject.Selected = true;
                        foreach(Cell c in selectedCellObject.GetSibilngs()) {
                            c.SiblingSelected = true;
                        }
                    }
                }
            }
        }

        public Board(int size) {
            TotalSize = size;
            Size = (int) Math.Sqrt(size);
            rows = createBoard();
        }

        ObservableCollection<ObservableCollection<Grid>> createBoard() {
            ObservableCollection<ObservableCollection<Grid>> rows = new ObservableCollection<ObservableCollection<Grid>>();
            for(int row = 0; row < Size; row++) {
                ObservableCollection<Grid> col = new ObservableCollection<Grid>();
                for(int c = 0; c < Size; c++) {
                    Grid grid = new Grid(this, row, c);
                    grid.PropertyChanged += new PropertyChangedEventHandler(gridPropertyChanged);
                    col.Add(grid);
                }
                rows.Add(col);
            }
            return rows;
        }

        private bool checkIsValid(Cell cell) {
            if(cell.Number.HasValue) {
                foreach(Cell c in cell.GetSibilngs()) {
                    if(c.Number.HasValue && c.Number == cell.Number) {
                        return false;
                    }
                }
            }

            return true;
        }

        void gridPropertyChanged(object sender, PropertyChangedEventArgs e) {
            Cell cell = (Cell) sender;
            if(e.PropertyName == Cell.NUMBER_EVENT) {
                cell.IsValid = checkIsValid(cell);
                foreach(Cell c in cell.GetSibilngs()) {
                    c.IsValid = checkIsValid(c);
                }
            }
        }

        public Cell this[int row, int col] {
            get {
                if(row < 0 || row > TotalSize - 1) {
                    throw new ArgumentOutOfRangeException("row", row, "Invalid row index");
                }
                if(col < 0 || col > TotalSize - 1) {
                    throw new ArgumentOutOfRangeException("col", col, "Invalid column index");
                }

                Grid grid = rows[row / Size][col / Size];
                return grid.GridRows[row % Size][col % Size];
            }
        }

        public void GenerateGame() {
            SolveBoard();

            Random random = new Random();
            int numberRemoved = 0;
            List<Cell> ineligibleCells = new List<Cell>();
            while(numberRemoved < 40) {
                Cell cell;
                do {
                    cell = this[random.Next(9), random.Next(9)];
                } while(!cell.Number.HasValue || ineligibleCells.Contains(cell));

                ObservableCollection<ObservableCollection<Grid>> rowsCopy = createBoard();
                for(int row = 0; row < TotalSize; row++) {
                    for(int col = 0; col < TotalSize; col++) {
                        Grid grid = rowsCopy[row / Size][col / Size];
                        Cell c = grid.GridRows[row % Size][col % Size];
                        c.Number = this[row, col].Number;
                    }
                }

                int? removedNumber = cell.Number;
                cell.Number = null;

                int solutionCount = 0;
                TestBoard(rowsCopy, ref solutionCount);
                if(solutionCount > 1) {
                    cell.Number = removedNumber;
                    ineligibleCells.Add(cell);
                } else {
                    numberRemoved++;
                }
            }

            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];
                    if(cell.Number.HasValue) {
                        cell.ReadOnly = true;
                    }
                }
            }
        }

        public bool SolveBoard() {
            int[] possibleValues = Enumerable.Range(1, TotalSize).ToArray();
            Random random = new Random();
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = possibleValues.OrderBy(x => random.Next()).ToList();
                    foreach(int i in remainingValues) {
                        cell.Number = i;
                        if(cell.IsValid) {
                            if((row == 8 && col == 8) || SolveBoard()) {
                                return true;
                            }
                        }
                    }

                    cell.Number = null;
                    return false;
                }
            }

            return false;
        }

        public bool TestBoard(ObservableCollection<ObservableCollection<Grid>> rows, ref int solutionCount) {
            int[] possibleValues = Enumerable.Range(1, TotalSize).ToArray();
            Random random = new Random();
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = possibleValues.OrderBy(x => random.Next()).ToList();
                    foreach(int i in remainingValues) {
                        cell.Number = i;
                        if(cell.IsValid) {
                            if((row == 8 && col == 8) || TestBoard(rows, ref solutionCount)) {
                                solutionCount++;
                                return true;
                            }
                        }
                    }

                    cell.Number = null;
                    return false;
                }
            }

            return false;
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

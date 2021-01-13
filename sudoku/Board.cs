using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Sudoku {
    public class Board : ICloneable, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        ObservableCollection<ObservableCollection<Grid>> _boardRows = new ObservableCollection<ObservableCollection<Grid>>();
        public ObservableCollection<ObservableCollection<Grid>> BoardRows {
            get {
                return _boardRows;
            }
            set {
                _boardRows = BoardRows;
            }
        }

        int _size;
        public int Size {
            get {
                return _size;
            }
        }

        int _totalSize;
        public int TotalSize {
            get {
                return _totalSize;
            }
        }

        public List<int> PossibleValues;

        bool _gameGenerated = false;
        public bool GameGenerated {
            get {
                return _gameGenerated;
            }
            set {
                if(_gameGenerated != value) {
                    _gameGenerated = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool _gameLoading = false;
        public bool GameLoading {
            get {
                return _gameLoading;
            }
            set {
                if(_gameLoading != value) {
                    _gameLoading = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool _solving = false;
        public bool Solving {
            get {
                return _solving;
            }
            set {
                if(_solving != value) {
                    _solving = value;
                    NotifyPropertyChanged();
                }
            }
        }

        Cell _selectedCell;
        public Cell SelectedCell {
            get {
                return _selectedCell;
            }
            set {
                if(_selectedCell != value) {
                    if(_selectedCell != null) {
                        _selectedCell.Selected = false;
                        foreach(Cell c in _selectedCell.GetSibilngs()) {
                            c.SiblingSelected = false;
                        }
                    }
                    _selectedCell = value;
                    if(_selectedCell != null) {
                        _selectedCell.Selected = true;
                        foreach(Cell c in _selectedCell.GetSibilngs()) {
                            c.SiblingSelected = true;
                        }
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        public void GenerateBoard(int size) {
            GameGenerated = false;

            _size = (int) Math.Sqrt(size);
            _totalSize = Size * Size;
            PossibleValues = Enumerable.Range(1, TotalSize).ToList();

            BoardRows.Clear();
            for(int row = 0; row < Size; row++) {
                ObservableCollection<Grid> col = new ObservableCollection<Grid>();
                for(int c = 0; c < Size; c++) {
                    Grid grid = new Grid(this, row, c);
                    col.Add(grid);
                }
                BoardRows.Add(col);
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Cell this[int row, int col] {
            get {
                if(row < 0 || row > TotalSize - 1) {
                    throw new ArgumentOutOfRangeException("row", row, "Invalid row index");
                }
                if(col < 0 || col > TotalSize - 1) {
                    throw new ArgumentOutOfRangeException("col", col, "Invalid column index");
                }

                Grid grid = BoardRows[row / Size][col / Size];
                return grid.GridRows[row % Size][col % Size];
            }
        }

        public async Task GenerateGame() {
            if(GameLoading) {
                return;
            }
            GameLoading = true;

            await (new Solver()).Solve(this);

            Random random = new Random();
            int numberRemoved = 0;
            List<Cell> ineligibleCells = new List<Cell>();
            int totalCellCount = TotalSize * TotalSize;
            while(ineligibleCells.Count < totalCellCount / 2) {
                Cell cell;
                do {
                    cell = this[random.Next(TotalSize), random.Next(TotalSize)];
                } while(!cell.Number.HasValue || ineligibleCells.Contains(cell));

                int? removedNumber = cell.Number;
                cell.Number = null;

                Solver solver = new Solver();
                await solver.Test(this.Copy());

                if(solver.SolutionCount != 1) {
                    cell.Number = removedNumber;
                } else {
                    numberRemoved++;
                }
                ineligibleCells.Add(cell);
            }

            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];
                    if(!cell.Number.HasValue) {
                        cell.ReadOnly = false;
                    }
                }
            }

            GameGenerated = true;
            GameLoading = false;
        }

        public bool IsSolved() {
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];
                    if(!cell.Number.HasValue || !cell.IsValid) {
                        return false;
                    }
                }
            }
            return true;
        }

        public object Clone() {
            Board copy = new Board();
            copy.GenerateBoard(TotalSize);

            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];

                    Cell cellCopy = copy[row, col];
                    cellCopy.Number = cell.Number;
                    cellCopy.ReadOnly = cell.ReadOnly;
                }
            }

            copy.GameGenerated = GameGenerated;

            return copy;
        }

        public Board Copy() {
            return (Board) Clone();
        }
    }
}

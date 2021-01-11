using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Sudoku {
    public class Board : ICloneable, INotifyPropertyChanged {
        List<List<Grid>> rows;
        public List<List<Grid>> BoardRows {
            get {
                return rows;
            }
        }

        public const string GAME_GENERATED_EVENT = "GameGenerated";

        public readonly int Size;
        public readonly int TotalSize;
        public readonly List<int> PossibleValues;

        bool gameGeneratedValue = false;
        public bool GameGenerated {
            get {
                return gameGeneratedValue;
            }
            set {
                if(gameGeneratedValue != value) {
                    gameGeneratedValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(GAME_GENERATED_EVENT));
                    }
                }
            }
        }

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
            PossibleValues = Enumerable.Range(1, TotalSize).ToList();
            rows = createBoard();
        }

        List<List<Grid>> createBoard() {
            List<List<Grid>> rows = new List<List<Grid>>();
            for(int row = 0; row < Size; row++) {
                List<Grid> col = new List<Grid>();
                for(int c = 0; c < Size; c++) {
                    Grid grid = new Grid(this, row, c);
                    col.Add(grid);
                }
                rows.Add(col);
            }
            return rows;
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

        public async Task GenerateGame() {
            if(GameGenerated) {
                GameGenerated = false;
                for(int row = 0; row < TotalSize; row++) {
                    for(int col = 0; col < TotalSize; col++) {
                        this[row, col].Number = null;
                        this[row, col].ReadOnly = true;
                    }
                }
            }
            await SolveBoard();

            Random random = new Random();
            int numberRemoved = 0;
            List<Cell> ineligibleCells = new List<Cell>();
            int totalCellCount = TotalSize * TotalSize;
            while(numberRemoved < totalCellCount * .375 && ineligibleCells.Count < totalCellCount - numberRemoved) {
                Cell cell;
                do {
                    cell = this[random.Next(TotalSize), random.Next(TotalSize)];
                } while(!cell.Number.HasValue || ineligibleCells.Contains(cell));

                int? removedNumber = cell.Number;
                cell.Number = null;

                Board boardCopy = this.Copy();

                Tester.SolutionCount = 0;
                await Tester.TestBoard(boardCopy);

                if(Tester.SolutionCount != 1) {
                    Console.Write(String.Format("{0} solutions for cell at {1}, {2}.", Tester.SolutionCount, cell.Row, cell.Col));
                    cell.Number = removedNumber;
                    ineligibleCells.Add(cell);
                } else {
                    numberRemoved++;
                }
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

        public async Task<bool> SolveBoard() {
            await Task.Delay(200);

            Random random = new Random();
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = PossibleValues.OrderBy(x => random.Next()).ToList();
                    foreach(int i in remainingValues) {
                        cell.Number = i;
                        if(cell.IsValid) {
                            if((row == TotalSize - 1 && col == TotalSize - 1) || await SolveBoard()) {
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

        class Tester {
            public static int SolutionCount;
            public static async Task TestBoard(Board board) {
                await Task.Delay(200);

                if(SolutionCount >= 2) {
                    return;
                }

                Random random = new Random();
                for(int row = 0; row < board.TotalSize; row++) {
                    for(int col = 0; col < board.TotalSize; col++) {
                        Cell cell = board[row, col];

                        if(cell.Number.HasValue) {
                            continue;
                        }

                        List<int> remainingValues = board.PossibleValues.OrderBy(x => random.Next()).ToList();
                        foreach(int i in remainingValues) {
                            cell.Number = i;
                            if(cell.IsValid) {
                                if(board.IsSolved()) {
                                    SolutionCount++;
                                    return;
                                }
                                Board boardCopy = board.Copy();
                                await TestBoard(boardCopy);
                            }
                        }

                        cell.Number = null;
                        return;
                    }
                }
            }
        }

        public object Clone() {
            Board copy = new Board(TotalSize);

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

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Sudoku {
    public class Board : ICloneable {
        List<List<Grid>> rows;
        public List<List<Grid>> BoardRows {
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

        public void GenerateGame() {
            SolveBoard();

            Random random = new Random();
            int numberRemoved = 0;
            List<Cell> ineligibleCells = new List<Cell>();
            while(numberRemoved < 35 && ineligibleCells.Count < (TotalSize * TotalSize) - numberRemoved) {
                Cell cell;
                do {
                    cell = this[random.Next(9), random.Next(9)];
                } while((!cell.Number.HasValue || ineligibleCells.Contains(cell)));

                int? removedNumber = cell.Number;
                cell.Number = null;

                Board boardCopy = this.Copy();

                int solutionCount = 0;
                TestBoard(boardCopy, ref solutionCount);
                if(solutionCount != 1) {
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
            Random random = new Random();
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = this[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = cell.PossibleValues.OrderBy(x => random.Next()).ToList();
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

        public void TestBoard(Board board, ref int solutionCount) {
            if(solutionCount >= 2) {
                return;
            }

            Random random = new Random();
            for(int row = 0; row < TotalSize; row++) {
                for(int col = 0; col < TotalSize; col++) {
                    Cell cell = board[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = cell.PossibleValues.OrderBy(x => random.Next()).ToList();
                    foreach(int i in remainingValues) {
                        cell.Number = i;
                        if(cell.IsValid) {
                            if(row == 8 && col == 8) {
                                solutionCount++;
                                return;
                            }
                            Board boardCopy = board.Copy();
                            TestBoard(boardCopy, ref solutionCount);
                        }
                    }
                    return;
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

            return copy;
        }

        public Board Copy() {
            return (Board) Clone();
        }
    }
}

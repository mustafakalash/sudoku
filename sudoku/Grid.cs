using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Sudoku {
    public class Grid {
        readonly List<List<Cell>> rows;
        public List<List<Cell>> GridRows {
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

            rows = new List<List<Cell>>();
            for(int r = 0; r < board.Size; r++) {
                List<Cell> colL = new List<Cell>();
                for(int c = 0; c < board.Size; c++) {
                    Cell cell = new Cell(this, r, c);
                    cell.ReadOnly = true;
                    colL.Add(cell);
                }
                rows.Add(colL);
            }
        }
    }
}

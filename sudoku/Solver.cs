using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku {
    class Solver {
        public int SolutionCount = 0;

        public async Task<bool> Solve(Board board) {
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
                            if(board.IsSolved() || await Solve(board)) {
                                return true;
                            }
                        }
                        await Task.Delay(MainWindow.SolveSpeed);
                    }

                    cell.Number = null;
                    return false;
                }
            }

            return false;
        }

        public async Task<bool> Test(Board board) {
            if(SolutionCount >= 2) {
                return false;
            }

            Random random = new Random();
            for(int row = 0; row < board.TotalSize; row++) {
                for(int col = 0; col < board.TotalSize; col++) {
                    Cell cell = board[row, col];

                    if(cell.Number.HasValue) {
                        continue;
                    }

                    List<int> remainingValues = board.PossibleValues.OrderBy(x => random.Next()).ToList();
                    List<Task<bool>> forks = new List<Task<bool>>();
                    foreach(int i in remainingValues) {
                        cell.Number = i;
                        if(cell.IsValid) {
                            if(board.IsSolved()) {
                                SolutionCount++;
                                return true;
                            }

                            Board boardCopy = board.Copy();
                            Task<bool> fork = Test(boardCopy);
                            forks.Add(fork);

                            await Task.Delay(MainWindow.SolveSpeed);
                        }
                    }

                    bool[] solved = await Task.WhenAll(forks);
                    return solved.Contains(true);
                }
            }

            return true;
        }
    }
}

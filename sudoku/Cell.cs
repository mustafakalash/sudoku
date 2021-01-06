using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku {
    public class Cell : INotifyPropertyChanged {
        public const string READ_ONLY_EVENT = "ReadOnly";
        public const string NUMBER_EVENT = "Number";
        public const string IS_VALID_EVENT = "IsValid";
        public const string SELECTED_EVENT = "Selected";
        public const string SIBLING_SELECTED_EVENT = "SiblingSelected";

        bool readOnlyValue = false;
        public bool ReadOnly {
            get {
                return readOnlyValue;
            }
            set {
                if(readOnlyValue != value) {
                    readOnlyValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(READ_ONLY_EVENT));
                    }
                }
            }
        }

        int? numberValue = null;
        public int? Number {
            get {
                return numberValue;
            }
            set {
                if(numberValue != value) {
                    numberValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(NUMBER_EVENT));
                    }
                }
            }
        }

        bool isValidValue = true;

        public bool IsValid {
            get {
                return isValidValue;
            }
            set {
                if(isValidValue != value) {
                    isValidValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(IS_VALID_EVENT));
                    }
                }
            }
        }

        public readonly List<int> PossibleValues;

        ObservableCollection<int> notesList = new ObservableCollection<int>();
        public ObservableCollection<int> Notes {
            get {
                return notesList;
            }
            set {
                notesList = value;
            }
        }

        public List<int> AutoRemovedNotes = new List<int>();

        bool selectedValue = false;
        public bool Selected {
            get {
                return selectedValue;
            }
            set {
                if(selectedValue != value) {
                    selectedValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(SELECTED_EVENT));
                    }
                }
            }
        }

        bool siblingSelectedValue = false;
        public bool SiblingSelected {
            get {
                return siblingSelectedValue;
            }
            set {
                if(siblingSelectedValue != value) {
                    siblingSelectedValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs(SIBLING_SELECTED_EVENT));
                    }
                }
            }
        }

        public readonly int Row, Col, GridRow, GridCol;
        public readonly Grid Grid;

        public Cell(Grid grid, int row, int col) {
            GridRow = row;
            GridCol = col;
            Grid = grid;
            Row = GridRow + (Grid.Row * Grid.Board.Size);
            Col = GridCol + (Grid.Col * Grid.Board.Size);
            PossibleValues = Enumerable.Range(1, grid.Board.TotalSize).ToList();

            PropertyChanged += new PropertyChangedEventHandler(propertyChanged);
        }

        public HashSet<Cell> GetSibilngs() {
            HashSet<Cell> cells = new HashSet<Cell>();

            foreach(List<Cell> col in Grid.GridRows) {
                foreach(Cell c in col) {
                    cells.Add(c);
                }
            }

            for(int i = 0; i < Grid.Board.TotalSize; i++) {
                cells.Add(Grid.Board[i, Col]);
                cells.Add(Grid.Board[Row, i]);
            }

            cells.Remove(this);

            return cells;
        }

        private bool checkIsValid(int? number = null) {
            if(!number.HasValue) {
                number = Number;
            }
            if(number.HasValue) {
                foreach(Cell c in GetSibilngs()) {
                    if(c.Number.HasValue && c.Number == number) {
                        return false;
                    }
                }
            }

            return true;
        }

        private void updateNotes() {
            foreach(int i in Notes.ToList()) {
                if(!checkIsValid(i)) {
                    Notes.Remove(i);
                    AutoRemovedNotes.Add(i);
                }
            }
            foreach(int i in AutoRemovedNotes.ToList()) {
                if(checkIsValid(i)) {
                    Notes.Add(i);
                    AutoRemovedNotes.Remove(i);
                }
            }
        }

        void propertyChanged(object sender, PropertyChangedEventArgs e) {
            Cell cell = (Cell) sender;
            if(e.PropertyName == Cell.NUMBER_EVENT) {
                bool validCheck = checkIsValid();
                cell.IsValid = validCheck;
                updateNotes();
                foreach(Cell c in cell.GetSibilngs()) {
                    c.IsValid = c.checkIsValid();
                    c.updateNotes();
                }
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

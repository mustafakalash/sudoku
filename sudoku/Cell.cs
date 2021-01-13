using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku {
    public class Cell : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        bool _readOnly = false;
        public bool ReadOnly {
            get {
                return _readOnly;
            }
            set {
                if(_readOnly != value) {
                    _readOnly = value;
                    NotifyPropertyChanged();
                }
            }
        }

        int? _number = null;
        public int? Number {
            get {
                return _number;
            }
            set {
                if(_number != value) {
                    _number = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool _isValid = true;
        public bool IsValid {
            get {
                return _isValid;
            }
            set {
                if(_isValid != value) {
                    _isValid = value;
                    NotifyPropertyChanged();
                }
            }
        }

        readonly ObservableCollection<int> _notes = new ObservableCollection<int>();
        public ObservableCollection<int> Notes {
            get {
                return _notes;
            }
        }

        public readonly List<int> AutoRemovedNotes = new List<int>();

        bool _selected = false;
        public bool Selected {
            get {
                return _selected;
            }
            set {
                if(_selected != value) {
                    _selected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        bool _siblingSelected = false;
        public bool SiblingSelected {
            get {
                return _siblingSelected;
            }
            set {
                if(_siblingSelected != value) {
                    _siblingSelected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public readonly int Row, Col, GridRow, GridCol;

        Grid _grid;
        public Grid Grid {
            get {
                return _grid;
            }
            set {
                _grid = value;
            }
        }

        public Cell(Grid grid, int row, int col) {
            GridRow = row;
            GridCol = col;
            _grid = grid;
            Row = GridRow + (Grid.Row * Grid.Board.Size);
            Col = GridCol + (Grid.Col * Grid.Board.Size);

            PropertyChanged += new PropertyChangedEventHandler(propertyChanged);
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public HashSet<Cell> GetSibilngs() {
            HashSet<Cell> cells = new HashSet<Cell>();

            foreach(ObservableCollection<Cell> col in Grid.GridRows) {
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
            if(e.PropertyName == nameof(Number)) {
                bool validCheck = checkIsValid();
                IsValid = validCheck;
                if(MainWindow.AutoNotes) updateNotes();
                foreach(Cell c in GetSibilngs()) {
                    c.IsValid = c.checkIsValid();
                    if(MainWindow.AutoNotes) c.updateNotes();
                }
            }
        }
    }
}

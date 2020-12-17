using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku {
    public class Cell : INotifyPropertyChanged {
        bool readOnlyValue = false;
        public bool ReadOnly {
            get {
                return readOnlyValue;
            }
            set {
                if(readOnlyValue != value) {
                    readOnlyValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("ReadOnly"));
                    }
                }
            }
        }

        byte? numberValue = null;
        public byte? Number {
            get {
                return numberValue;
            }
            set {
                if(numberValue != value) {
                    numberValue = value;
                    if(PropertyChanged != null) {
                        PropertyChanged(this, new PropertyChangedEventArgs("Number"));
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
                        PropertyChanged(this, new PropertyChangedEventArgs("IsValid"));
                    }
                }
            }
        }

        Collection<byte> possibleValuesList = new Collection<byte> {1, 2, 3, 4, 5, 6, 7, 8, 9};
        public Collection<byte> PossibleValues {
            get {
                return possibleValuesList;
            }
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DesktopPresentationWPF.Models {
    public class WpfMateriaModel : INotifyPropertyChanged {
        public int Id { get; set; }

        public int? Year { get; set; }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        private bool? _isElectiva;

        public bool? IsElectiva {
            get
            {
                return _isElectiva;
            }
            set
            {
                _isElectiva = value;
                CallPropertyChanged(nameof(IsElectivaDisplay));
            }
        }

        public string IsElectivaDisplay
        {
            get
            {
                if (IsElectiva == null)
                {
                    return "Undefined";
                }

                if (IsElectiva == true)
                {
                    return "Electiva";
                }
                else
                {
                    return "Obligatoria";
                }
            }
        }

        public int? DepartamentoId { get; set; }
        public virtual WpfDepartamentoModel Departamento { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

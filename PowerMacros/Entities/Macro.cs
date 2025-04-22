using System.Collections.Generic;
using System.ComponentModel;

namespace PowerMacros.Entities
{
    public class Macro : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private MacroType _macroType = Entities.MacroType.Code;
        private string _code;
        private string _shortcut;
        private List<MacroAction> _actions = new List<MacroAction>();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public MacroType MacroType
        {
            get => _macroType;
            set
            {
                _macroType = value;
                OnPropertyChanged(nameof(MacroType));
            }
        }

        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        public string Shortcut
        {
            get => _shortcut;
            set
            {
                _shortcut = value;
                OnPropertyChanged(nameof(Shortcut));
            }
        }

        public List<MacroAction> Actions
        {
            get => _actions;
            set
            {
                _actions = value;
                OnPropertyChanged(nameof(Actions));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Name} - {Description} - {Shortcut}";
        }
    }

    public enum MacroType
    {
        Code,
        Action
    }

    public class MacroAction
    {
        public string Key { get; set; }
        public string Modifier { get; set; }
        public double Delay { get; set; }
    }
}
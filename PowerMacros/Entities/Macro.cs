using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace PowerMacros.Entities
{
    public class Macro : INotifyPropertyChanged
    {
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private MacroType _macroType = Entities.MacroType.Code;
        public MacroType MacroType
        {
            get => _macroType;
            set
            {
                _macroType = value;
                OnPropertyChanged(nameof(MacroType));
            }
        }

        private string _code;
        public string Code
        {
            get => _code;
            set
            {
                _code = value;
                OnPropertyChanged(nameof(Code));
            }
        }

        private string _shortcut;
        public string Shortcut
        {
            get => _shortcut;
            set
            {
                _shortcut = value;
                OnPropertyChanged(nameof(Shortcut));
            }
        }

        private List<MacroAction> _actions = new List<MacroAction>();
        public List<MacroAction> Actions
        {
            get => _actions;
            set
            {
                _actions = value;
                OnPropertyChanged(nameof(Actions));
            }
        }

        private string _preview;
        public string Preview
        {
            get => _preview;
            set
            {
                _preview = value;
                OnPropertyChanged(nameof(Preview));
            }
        }

        public void UpdatePreview()
        {
            if (MacroType == MacroType.Code)
            {
                if (string.IsNullOrWhiteSpace(Code))
                {
                    Preview = string.Empty;
                    return;
                }

                var lines = Code.Split(new[] { '\r', '\n' });
                var preview = lines[0];

                if (preview.Length > 50)
                {
                    preview = preview.Substring(0, 50) + "...";
                }
                else if (lines.Length > 1)
                {
                    preview += "...";
                }

                Preview = preview;
            }
            else if (MacroType == MacroType.Action)
            {
                Preview = $"Actions: {Actions.Count}";
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
        public MacroActionType Type { get; set; }
        public Keys KeyCode { get; set; }
        public Keys Modifiers { get; set; }
        public int Delay { get; set; }
    }

    public enum MacroActionType
    {
        KeyDown,
        KeyUp
    }
}
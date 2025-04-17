using System.Collections.Generic;

namespace PowerMacros.Entities
{
    public class Macro
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public MacroType MacroType { get; set; } = MacroType.Code;
        public string Code { get; set; }
        public string Shortcut { get; set; }
        public List<MacroAction> Actions { get; set; } = new List<MacroAction>();

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

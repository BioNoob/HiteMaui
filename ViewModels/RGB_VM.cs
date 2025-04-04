using HiteMaui.Models;
using PropertyChanged;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class RGB_VM
    {
        public Color CLR
        {
            get;
            set;
        }
        public RGB RGB_switch { get; set; }
        public bool ColorPalleteIsOpen { get; set; }
        public RGB_VM()
        {
            RGB_switch = new RGB() { Dimm = 50, Colour = "#FF0000" };
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            RGB_switch = (RGB)query["RGB_element"];
        }
        public bool IsLightOn
        {
            get => RGB_switch.LightState; 
            set 
            {
                isLightOn = value;
                RGB_switch.Dimm = value ?  RGB_switch.LastDim : 0;
            } 
        }
        private CommandHandler _openpall;
        private bool isLightOn;

        public CommandHandler OpenPalletCmd
        {
            get
            {
                return _openpall ??= new CommandHandler(obj =>
                {
                },
                (obj) => true);
            }
        }
    }
}

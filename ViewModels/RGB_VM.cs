using CommunityToolkit.Maui.Core.Extensions;
using HiteMaui.Models;
using PropertyChanged;
namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class RGB_VM
    {
        public Color CLR
        {
            get => cLR;
            set
            {
                if (value != null)
                {
                    cLR = value;
                    EntryColor = value.ToArgbHex(false);
                    //var t = value.Red
                    //InvertCLR = Color.FromRgb((byte)~value.Red, ~value.Green, ~value.Blue);
                    //InvertCLR = CLR.//ToInverseColor();
                    if (!isINIT)
                    {
                        RGB_switch.Colour = value?.ToArgbHex(false).Remove(0, 1);
                        //RGB_switch.SwitchDeviceCmd.Execute(null);
                    }
                }
            }
        }
        public Color InvertCLR { get; set; }
        public string EntryColor
        {
            get => entryColor; set
            {
                entryColor = value;
            }
        }
        private bool isLightOn;
        private bool isINIT;
        private CommandHandler _openpall;
        private Color cLR;
        private int dimmer_val;
        private string entryColor;

        public RGB RGB_switch { get; set; }
        public bool ColorPalleteIsOpen { get; set; }
        public RGB_VM()
        {
            isINIT = true;
            CLR = new Color();
            RGB_switch = new RGB() { Dimm = 50, Colour = "#FF0000", Name = "Jopa" };
            Dimmer_val = 50;
            IsLightOn = true;

            isINIT = false;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            isINIT = true;
            RGB_switch = (RGB)query["RGB_element"];
            IsLightOn = RGB_switch.LightState;
            CLR = RGB_switch.Colour_!;
            isINIT = false;
        }
        //НУЖНО СОБЫТИЕ РЕФРЕША ТОГДА ДЛЯ ОБНОВЛЕНИЯ С ТОГО ЧТО ПОЛУЧИЛОСЬ ПОСЛЕ обновления
        public int Dimmer_val
        {
            get => dimmer_val; set
            {
                dimmer_val = value;
                if (!isINIT)
                {
                    RGB_switch.Dimm = value;
                    //RGB_switch.SwitchDeviceCmd.Execute(null);
                }
            }
        }
        public bool IsLightOn
        {
            get => isLightOn;
            set
            {
                isLightOn = value;
                if (!isINIT)
                {
                    Dimmer_val = !value ? 0 : RGB_switch.LastDim;
                }
            }
        }


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

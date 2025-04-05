using HiteMaui.Models;
using PropertyChanged;
namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class RGB_VM : IQueryAttributable
    {

        private bool isLightOn;
        private bool isINIT;
        private CommandHandler _openpall;
        private CommandHandler _refresh;
        private CommandHandler _entrydone;
        private CommandHandler _backward;
        private CommandHandler _colorpalpick;
        private Color cLR;
        private int dimmer_val;
        private string entryColor;
        bool entupd = false;
        bool dimupd = false;
        public RGB RGB_switch { get; set; }
        public bool ColorPalleteIsOpen { get; set; }
        public bool Updating { get; set; }
        public Color CLR
        {
            get => cLR;
            set
            {
                if (value != null)
                {
                    cLR = value;
                    if (!entupd)
                        EntryColor = value.ToArgbHex(false);
                    CLR.ToRgb(out byte r, out byte g, out byte b);
                    InvertCLR = (((r + g + b) / 3) > 128) ? Colors.Black : Colors.White;
                    if (!isINIT)
                    {
                        RGB_switch.Colour = value?.ToArgbHex(false).Remove(0, 1);
                        Refresh();
                    }
                }
            }
        }
        public Color InvertCLR { get; set; }
        [OnChangedMethod(nameof(EntryUpdate))]
        public string EntryColor
        {
            get => entryColor; set
            {
                entryColor = value;
            }
        }
        public void EntryUpdate()
        {
            entupd = true;
            CLR = Color.FromRgba(EntryColor);
            entupd = false;
        }
        public RGB_VM()
        {
            isINIT = true;
            CLR = new Color();
            RGB_switch = new RGB() { Dimm = 50, Colour = "#FF0000", Name = "Jopa" };
            CLR = RGB_switch.Colour_;
            Dimmer_val = 50;
            IsLightOn = true;
            ColorPalleteIsOpen = false;
            isINIT = false;
            Updating = false;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            isINIT = true;
            RGB_switch = (RGB)query["Dev"];
            RGB_switch.OnUpdateRecived += RGB_switch_OnUpdateRecived;
            IsLightOn = RGB_switch.LightState;
            Dimmer_val = RGB_switch.Dimm;
            CLR = RGB_switch.Colour_!;
            isINIT = false;
        }

        private void RGB_switch_OnUpdateRecived()
        {
            isINIT = true;
            IsLightOn = RGB_switch.LightState;
            Dimmer_val = RGB_switch.Dimm;
            CLR = RGB_switch.Colour_!;
            isINIT = false;
            Updating = false;
        }

        public void Refresh()
        {
            isINIT = true;
            Updating = true;
            RGB_switch.SwitchDeviceCmd.Execute(null);
            //IsLightOn = RGB_switch.LightState;
            //Dimmer_val = RGB_switch.Dimm;
            //CLR = RGB_switch.Colour_!;
            isINIT = false;
        }
        public bool IsLightOn
        {
            get => isLightOn;
            set
            {
                isLightOn = value;
                if (!isINIT && !dimupd)
                    Dimmer_val = !value ? 0 : RGB_switch.LastDim;
            }
        }
        [OnChangedMethod(nameof(DimmUpdate))]
        public int Dimmer_val
        {
            get => dimmer_val; set
            {
                dimmer_val = value;
                if (!isINIT)
                {
                    RGB_switch.Dimm = value;
                    Refresh();
                }
            }
        }
        void DimmUpdate()
        {
            dimupd = true;
            IsLightOn = Dimmer_val > 0;
            dimupd = false;
        }

        public CommandHandler EntryDoneCmd
        {
            get
            {
                return _entrydone ??= new CommandHandler(obj =>
                {
                },
                (obj) => true);
            }
        }
        public CommandHandler BackCmd
        {
            get
            {
                return _backward ??= new CommandHandler(obj =>
                {
                    Shell.Current.GoToAsync($"//List",true);
                },
                (obj) => true);
            }
        }
        public CommandHandler ColorPalPickCmd
        {
            get
            {
                return _colorpalpick ??= new CommandHandler(obj =>
                {
                    CLR = (obj as Color)!;
                    ColorPalleteIsOpen = false;
                },
                (obj) => true);
            }
        }
        public CommandHandler OpenPalletCmd
        {
            get
            {
                return _openpall ??= new CommandHandler(obj =>
                {
                    ColorPalleteIsOpen = true;
                },
                (obj) => true);
            }
        }
        public CommandHandler RefreshCmd
        {
            get
            {
                return _refresh ??= new CommandHandler(obj =>
                {
                    Refresh();
                },
                (obj) => true);
            }
        }
    }
}

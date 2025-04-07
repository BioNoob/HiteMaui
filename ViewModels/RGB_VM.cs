using HiteMaui.Models;
using PropertyChanged;
namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public partial class RGB_VM : Dimmer_VM, IQueryAttributable
    {
        private bool isINIT;
        private CommandHandler _openpall;
        private CommandHandler _colorpalpick;
        private Color cLR;
        private string entryColor;
        bool entupd = false;
        //public RGB RGB_switch { get => Device! as RGB; }
        public bool ColorPalleteIsOpen { get; set; }
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
                        Device.Colour = value?.ToArgbHex(false).Remove(0, 1);
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
            //Device = new RGB() { Dimm = 50, Colour = "#FF0000", Name = "Jopa" };
            Dimmer_val = 50;
            IsLightOn = true;
            ColorPalleteIsOpen = false;
            isINIT = false;
            Updating = false;
        }

        public new void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            isINIT = true;
            Device = null;
            Device = (RGB)query["Dev"];
            Device.OnUpdateRecived += RGB_switch_OnUpdateRecived;
            IsLightOn = Device.LightState;
            Dimmer_val = Device.Dimm;
            CLR = ((RGB)Device).Colour_!;
            isINIT = false;
        }

        private void RGB_switch_OnUpdateRecived()
        {
            isINIT = true;
            IsLightOn = Device.LightState;
            Dimmer_val = Device.Dimm;
            CLR = ((RGB)Device).Colour_!;
            isINIT = false;
            Updating = false;
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
    }
}

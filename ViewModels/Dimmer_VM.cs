using HiteMaui.Models;
using PropertyChanged;

namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class Dimmer_VM : IQueryAttributable
    {
        private bool isLightOn;
        private bool isINIT;
        private int dimmer_val;
        bool dimupd = false;
        private CommandHandler _refresh;
        private CommandHandler _backward;
        public Dimmer Device { get; set; }
        public Dimmer_VM()
        {
            isINIT = true;
            Device = new Dimmer() { Dimm = 50, Name = "Jopa" }; ;
            Dimmer_val = 50;
            IsLightOn = true;
            isINIT = false;
            Updating = false;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            isINIT = true;
            Device = null;
            Device = (Dimmer)query["Dev"];
            Device.OnUpdateRecived += Dims_OnUpdateRecived;
            IsLightOn = Device.LightState;
            Dimmer_val = Device.Dimm;
            isINIT = false;
        }

        private void Dims_OnUpdateRecived()
        {
            isINIT = true;
            IsLightOn = Device.LightState;
            Dimmer_val = Device.Dimm;
            isINIT = false;
            Updating = false;
        }
        public bool Updating { get; set; }
        public CommandHandler BackCmd
        {
            get
            {
                return _backward ??= new CommandHandler(obj =>
                {
                    Shell.Current.GoToAsync($"//List", true);
                },
                (obj) => true);
            }
        }
        public void Refresh()
        {
            isINIT = true;
            Updating = true;
            //Device.UptadeDevInfoCmd.Execute(null);
            isINIT = false;
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

        public bool IsLightOn
        {
            get => isLightOn;
            set
            {
                isLightOn = value;
                if (!isINIT && !dimupd)
                    Dimmer_val = !value ? 0 : Device.LastDim;
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
                    Device.Dimm = value;
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
    }
}



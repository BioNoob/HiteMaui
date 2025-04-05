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
        public Dimmer Dims { get; set; }
        public Dimmer_VM()
        {
            isINIT = true;
            Dims = new Dimmer() { Dimm = 50, Name = "Jopa" }; ;
            Dimmer_val = 50;
            IsLightOn = true;
            isINIT = false;
            Updating = false;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            isINIT = true;
            Dims = (Dimmer)query["Dev"];
            Dims.OnUpdateRecived += Dims_OnUpdateRecived;
            IsLightOn = Dims.LightState;
            Dimmer_val = Dims.Dimm;
            isINIT = false;
        }

        private void Dims_OnUpdateRecived()
        {
            isINIT = true;
            IsLightOn = Dims.LightState;
            Dimmer_val = Dims.Dimm;
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
            Dims.SwitchDeviceCmd.Execute(null);

            //IsLightOn = Dims.LightState;
            //Dimmer_val = Dims.Dimm;
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
                    Dimmer_val = !value ? 0 : Dims.LastDim;
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
                    Dims.Dimm = value;
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



using HiteMaui.Models;
using PropertyChanged;
using System.Collections.ObjectModel;
using Device = HiteMaui.Models.Device;

namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ListDevModel : IQueryAttributable
    {
        public ObservableCollection<Device> Devices { get; set; } = new ObservableCollection<Device>();
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Devices = (ObservableCollection<Device>)query["Devices"];
        }
        private CommandHandler _openedit;
        public CommandHandler OpenEditCmd
        {
            get
            {
                return _openedit ??= new CommandHandler(async obj =>
                {
                    var iam = new Dictionary<string, object>() { { "Dev", obj } };
                    if (obj is null)
                        return;
                    switch (obj.GetType().Name)
                    {
                        case "Dimmer":
                            await Shell.Current.GoToAsync($"//DIMM", true, iam);
                            break;
                        case "RGB":
                            await Shell.Current.GoToAsync($"//RGB", true, iam);
                            break;
                        case "Switcher":
                            //await Shell.Current.GoToAsync($"//SWITCH", true, iam);
                            break;
                        default:
                            break;
                    }
                },
                (obj) => true
                );
            }
        }
    }
}

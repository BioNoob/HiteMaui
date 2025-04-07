using PropertyChanged;
using static HiteMaui.Models.HiteModel;

namespace HiteMaui.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Switcher : Device
    {
        public enum States
        {
            On = 1,
            Off = 2
        }
        public States State
        {
            get
            {
                return Status switch
                {
                    int => (int)Status == 1 ? States.On : States.Off,
                    string => (string)Status == "true" ? States.On : States.Off,
                    bool => (bool)Status ? States.On : States.Off,
                    _ => States.Off
                };
            }
        }
        public override async Task<bool> ChangeState()
        {
            try
            {
                int state = State == States.Off ? (int)States.On : (int)States.Off;
                using HttpRequestMessage request = new(HttpMethod.Put,
                    $"{Url_req}{Id}/{state}");
                var resp = await client.SendAsync(request);
                var a = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Switcher()
        {

        }

    }
}

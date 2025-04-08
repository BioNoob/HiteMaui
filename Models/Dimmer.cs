using PropertyChanged;
using static HiteMaui.Models.HiteModel;

namespace HiteMaui.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Dimmer : Device, ISwitcher
    {
        private int dimm;
        public int LastDim { get; set; }
        public int Dimm
        {
            get => dimm; set
            {
                dimm = value;
                if (dimm > 0)
                    LastDim = value;
            }
        }

        public Dimmer() : base()
        {
            Dimm = 0;
            LastDim = Dimm;
            
        }
        public override async Task<bool> ChangeState()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Put, $"{Url_req}{Id}/{(Dimm == 0 ? 100 : 0)}");
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
    }
}

using PropertyChanged;
using static HiteMaui.Models.HiteModel;
using Color = Microsoft.Maui.Graphics.Color;

namespace HiteMaui.Models
{
    [AddINotifyPropertyChangedInterface]
    public class RGB : Dimmer
    {
        public Color? Colour_ => string.IsNullOrEmpty(Colour) ? null : Color.FromArgb($"#{Colour}");
        public RGB()
        {
            Colour = string.Empty;
        }
        
        public override async Task<bool> ChangeState()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Put, $"{Url_req}{Id}/" +
                    $"{Dimm}/?color={Colour_?.ToArgbHex(false).Remove(0, 1)}");//{ToHex(color)}");
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

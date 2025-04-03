using PropertyChanged;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using static HiteMaui.Models.HiteModel;
using Color = System.Drawing.Color;

namespace HiteMaui.Models
{
    public interface ISwitcher
    {
        public Task<bool> ChangeState();
    }

    [AddINotifyPropertyChangedInterface]
    public abstract class Device : ISwitcher
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }
        [JsonPropertyName("name")]
        public string Name { get; init; }
        [JsonPropertyName("type")]
        public string Type_str { get; init; }
        public Typer Type_
        {
            get
            {
                return GetTypeByStr(Type_str, Colour);
            }
        }
        [JsonPropertyName("status")]
        public object Status { get; set; }
        [JsonPropertyName("color")]
        public string? Colour { get; set; }
        public Device()
        {
            Id = 0;
            Name = "";
            Status = 0;
            Type_str = "";
            Colour = null;
            IsChanging = false;
        }
        public static bool GetLightState(JsonNode? status, ref object stat)
        {
            switch (status!.GetValueKind())
            {
                default:
                case JsonValueKind.String:
                    stat = status.GetValue<string>();
                    return false;
                case JsonValueKind.Number:
                    stat = status.GetValue<int>();
                    return (int)stat > 0;
                case JsonValueKind.False:
                case JsonValueKind.True:
                    stat = status.GetValue<bool>();
                    return (bool)stat;
            }
        }
        public static Typer GetTypeByStr(string t, string? color = null)
        {
            switch (t)
            {
                case "switch":
                    return Typer.swtch;
                case "drive":
                    return Typer.drive;
                case "dimmer":
                    if (string.IsNullOrEmpty(color))
                        return Typer.dimmer;
                    else
                        return Typer.rgb;
                case "rgb":
                    return Typer.rgb;
                case "checker":
                    return Typer.checker;
                case "water":
                    return Typer.water;
                case "power":
                    return Typer.power;
                case "motion":
                    return Typer.motion;
                case "illumination":
                    return Typer.illumination;
                case "temperature":
                    return Typer.temperature;
                case "humidity":
                default:
                    return Typer.nothing;
            }
        }
        public async Task<bool> RefreshInfo()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Get, $"{Url_req}{Id}/");
                var resp = await client.SendAsync(request);
                var a = await resp.Content.ReadAsStringAsync();
                JsonNode node = JsonNode.Parse(a)!;
                var status = node["status"]!;
                var color = node["color"]!;
                object stat = string.Empty;
                LightState = GetLightState(status, ref stat);
                Status = stat;
                //switch (status.GetValueKind())
                //{
                //    case JsonValueKind.String:
                //        Status = status.GetValue<string>();
                //        break;
                //    case JsonValueKind.Number:
                //        Status = status.GetValue<int>();
                //        LightState = (int)Status > 0;
                //        break;
                //    case JsonValueKind.False:
                //    case JsonValueKind.True:
                //        Status = status.GetValue<bool>();
                //        LightState = (bool)Status;
                //        break;
                //}
                if (color != null)
                    Colour = color.GetValue<string>();
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public bool LightState { get; set; }
        abstract public Task<bool> ChangeState();
        public bool IsChanging { get; set; }
        private CommandHandler _switchdev;
        public CommandHandler SwitchDeviceCmd
        {
            get
            {
                return _switchdev ??= new CommandHandler(async obj =>
                {
                    IsChanging = true;
                    try
                    {
                        if (await ChangeState())
                        {
                            _ = await RefreshInfo();
                            IsChanging = false;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                },
                (obj) => !IsChanging
                );
            }
        }
    }
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
    [AddINotifyPropertyChangedInterface]
    public class Dimmer : Device, ISwitcher
    {
        public int Dimm { get; set; }

        public Dimmer()
        {
            Dimm = 0;
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
    [AddINotifyPropertyChangedInterface]
    public class RGB : Dimmer
    {
        public Color Colour_ => string.IsNullOrEmpty(Colour) ? Color.Empty : ColorTranslator.FromHtml($"#{Colour}");
        public RGB()
        {
            Colour = string.Empty;
        }
        public async Task<bool> SetColor(Color color, int dimmer = -1)
        {
            if (Type_ != Typer.dimmer)
                return false;
            else
            {
                using HttpRequestMessage request = new(HttpMethod.Put, $"{Url_req}{Id}/" +
                    $"{(dimmer == -1 ? Dimm : dimmer)}/?color={ToHex(color)}");
                var resp = await client.SendAsync(request);
                var a = await resp.Content.ReadAsStringAsync();
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                    return true;
                else return false;
            }
        }
        public override async Task<bool> ChangeState()
        {
            try
            {
                using HttpRequestMessage request = new(HttpMethod.Put,
                    $"{Url_req}{Id}/{(Dimm == 0 ? 100 : 0)}//?color={ToHex(Colour_)}");
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

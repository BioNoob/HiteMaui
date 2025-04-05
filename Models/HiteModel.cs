using PropertyChanged;
using Syncfusion.Maui.DataSource.Extensions;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Color = System.Drawing.Color;

namespace HiteMaui.Models
{        //https://www.hite-pro.ru/wp-content/uploads/manual/api.pdf
    public class UserException : Exception
    {
        public UserException(string? message) : base(message)
        {
        }
    }
    [AddINotifyPropertyChangedInterface]
    public class HiteModel
    {
        public static HttpClient client;
        public static string ToHex(Color c) => $"{c.R:X2}{c.G:X2}{c.B:X2}";
        private const string Url_http = "http://";
        private const string Url_end = "/rest/devices/";
        public const string Url_def = "192.168.100.1";
        public static string Url_req = "";
        private string ip;
        public enum Typer
        {
            [EnumMember(Value = "switch")]
            swtch,
            [EnumMember(Value = "drive")]
            drive,
            [EnumMember(Value = "dimmer")]
            dimmer,
            //custom
            [EnumMember(Value = "rgb")]
            rgb,
            //датчики
            [EnumMember(Value = "checker")]
            checker,
            [EnumMember(Value = "water")]
            water,
            [EnumMember(Value = "power")]
            power,
            [EnumMember(Value = "motion")]
            motion,
            [EnumMember(Value = "illumination")]
            illumination,
            [EnumMember(Value = "temperature")]
            temperature,
            [EnumMember(Value = "humidity")]
            humidity,
            //null
            [EnumMember(Value = "err")]
            nothing

        }
        public string Ip { get => ip; set { ip = value; Url_req = Url_http + value + Url_end; } }
        public string Usr { get; set; }
        public string Pass { get; set; }
        string Coded =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Usr}:{Pass}"));
        public ObservableCollection<Device> Devices { get; set; }
        public HiteModel()
        {
            Ip = "172.16.0.109";
            Usr = "root";
            Pass = "YarJarikBig2012";
            Devices = new ObservableCollection<Device>();
        }
        public void CloseClient() => client?.Dispose();
        public async Task<bool> GetDevices()
        {
            try
            {
                if (client is null)
                {
                    CloseClient();
                }
                    var socketsHandler = new SocketsHttpHandler
                    {
                        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                    };
                    client = new HttpClient(socketsHandler);
                    client.DefaultRequestHeaders.Authorization = new("Basic", Coded);
                //Url_req = "http://hitepro.local/devices/";
                using HttpRequestMessage request = new(HttpMethod.Get, Url_req);
                CancellationTokenSource s_cts = new CancellationTokenSource();
                s_cts.CancelAfter(4000);
                var resp = await client.SendAsync(request, s_cts.Token);
                if (resp.IsSuccessStatusCode)
                {
                    string body = await resp.Content.ReadAsStringAsync();
                    JsonNode node = JsonNode.Parse(body)!;
                    JsonArray arr = new JsonArray();
                    if (node.GetValueKind() == JsonValueKind.Array)
                        arr = node!.AsArray();
                    else
                        arr.Add(node);
                    arr.ForEach(t =>
                    {
                        object status = string.Empty;
                        var status_s = t["status"]!;
                        var color = t["color"]!;
                        var type = t["type"]!.GetValue<string>();
                        var id = t["id"]!.GetValue<int>(); 
                        var name = t["name"]!.GetValue<string>();
                        bool lightst = Device.GetLightState(status_s, ref status);
                        string? color_s = null;
                        if (color != null)
                            color_s = color.GetValue<string>();
                        switch (Device.GetTypeByStr(type, color_s))
                        {
                            case Typer.swtch:
                                Devices.Add(new Switcher()
                                {
                                    Colour = null,
                                    Id = id,
                                    Type_str = type,
                                    Name = name,
                                    Status = status,
                                    LightState = lightst
                                });
                                break;
                            case Typer.drive:
                                break;
                            case Typer.dimmer:
                                Devices.Add(new Dimmer()
                                {
                                    Colour = null,
                                    Dimm = (int)status,
                                    Id = id,
                                    Type_str = type,
                                    Name = name,
                                    Status = status,
                                    LightState = lightst
                                });
                                break;
                            case Typer.rgb:
                                Devices.Add(new RGB()
                                {
                                    Colour = color_s,
                                    Dimm = (int)status,
                                    Id = id,
                                    Type_str = "rgb",
                                    Name = name,
                                    Status = status,
                                    LightState = lightst
                                });
                                break;
                            case Typer.checker:
                                break;
                            case Typer.water:
                                break;
                            case Typer.power:
                                break;
                            case Typer.motion:
                                break;
                            case Typer.illumination:
                                break;
                            case Typer.temperature:
                                break;
                            case Typer.humidity:
                                break;
                            case Typer.nothing:
                                break;
                            default:
                                break;
                        }

                    });
                    return true;
                }
                else
                {
                    throw new UserException("Invalid login or pass");
                }
            }
            catch (OperationCanceledException)
            {
                throw new Exception("Connection TimeOut");
            }
            //catch (Exception)
            //{
            //    throw new Exception("Socket error");
            //}

        }
    }
}

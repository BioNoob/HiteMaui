using HiteMaui.Models;
using PropertyChanged;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace HiteMaui.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LoginModel : INotifyPropertyChanged
    {
        private CommandHandler _login;
        private string ip;
        private bool useLocal;
        string lastip = string.Empty;
        private string login;
        private string passwd;
        private bool ipIsValid;
        private bool isLocked;

        public string Ip
        {
            get => ip;
            set
            {
                ip = value;
                if (UseLocal)
                    IpIsValid = true;
                else
                    IpIsValid = Regex.IsMatch(value, @"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.?\b){4}$");
                if (IpIsValid && value != HiteModel.Url_def)
                    lastip = value;
            }
        }
        public void CheckAll() => AllIsValid = !IsLocked && IpIsValid && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Passwd);
        public string Login { get => login; set { login = value; CheckAll(); } }
        public string Passwd { get => passwd; set { passwd = value; CheckAll(); } }
        public bool IpIsValid { get => ipIsValid; set { ipIsValid = value; CheckAll(); } }
        public bool AllIsValid { get; set; }
        public string Messge { get; set; }
        public bool IsLocked { get => isLocked; set { isLocked = value; CheckAll(); } }
        public bool UseLocal { get => useLocal; set { useLocal = value; if (value) Ip = HiteModel.Url_def; else Ip = lastip; } }
        public CommandHandler DoLoginCmd
        {
            get
            {
                return _login ??= new CommandHandler(async obj =>
                {
                    try
                    {
                        HiteModel hite = new() { Ip = Ip, Pass = Passwd, Usr = Login };
                        IsLocked = true;
                        if (await hite.GetDevices())
                            await Shell.Current.GoToAsync($"//List", true,
                                new Dictionary<string, object>() { { "Devices", hite.Devices } });
                    }
                    catch (Exception e)
                    {
                        Messge = e.Message;
                    }
                    finally
                    {
                        IsLocked = false;
                    }

                    //else
                    //    var a = "ERR";
                },
                (obj) => true
                );
            }
        }
        public LoginModel()
        {
            Ip = string.Empty;
            Login = string.Empty;
            Passwd = string.Empty;
            IpIsValid = false;
            UseLocal = false;
            Messge = string.Empty;
            IsLocked = false;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}

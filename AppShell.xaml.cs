namespace HiteMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        protected override bool OnBackButtonPressed()
        {
            var z = Shell.Current.CurrentPage;
            switch (z.GetType().Name)
            {
                case "DimPage":
                case "RGB_Page":
                    Shell.Current.GoToAsync($"//List", true);
                    return true;
                case "ListDevPage":
                default:
                    return base.OnBackButtonPressed();
                    
            }
        }
    }
}

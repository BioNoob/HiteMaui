namespace HiteMaui
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var wins = new Window(new AppShell());
            //wins.Height = wins.MinimumHeight = wins.MaximumHeight = newheight;
            wins.Width = wins.MinimumWidth = 250;
            return wins;
        }
    }
}
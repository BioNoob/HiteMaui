using Syncfusion.Maui.Buttons;

namespace HiteMaui.Pages;

public partial class DimPage : ContentPage
{
	public DimPage()
	{
		InitializeComponent();
	}
    private async void SfButton_Clicked(object sender, EventArgs e)
    {
        var q = (sender as SfButton);
        await q.RotateTo(360);
        q.Rotation = 0;
    }
}
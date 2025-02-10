namespace QuickstartMaui;

using GoogleGson.Annotations;
using HyperTrack;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		DeviceIdLabel.Text = HyperTrack.DeviceId;
	}

	private void OnActionClicked(object sender, EventArgs e)
	{
		Dictionary<string, object?> data = new Dictionary<string, object?>
        {
            { "testKey", "testValue" }
        };
		HyperTrack.Json.Object json = HyperTrack.Json.FromMap(data);

		ActionBtn.Text = HyperTrack.AddGeotag(
			"orderHandle",
			new HyperTrack.OrderStatus.ClockIn(),
			json
		).ToString();
	}
}


namespace QuickstartMaui;

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
		HyperTrack.Json.Object json = HyperTrack.Json.FromDictionary(data);

		HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> result = HyperTrack.AddGeotag(
			"orderHandle",
			new HyperTrack.OrderStatus.ClockIn(),
			json
		);
		
		if (result.IsSuccess)
		{
			HyperTrack.Location location = result.Success;
			ResultLabel.Text = "Location: " + "Latitude: " + location.Latitude + ", Longitude: " + location.Longitude;
		}
		else
		{
			ResultLabel.Text = result.Failure != null ? result.Failure.ToString() : "Error";
			// ResultLabel.Text = result.Failure.ToString();
		}
	}
}


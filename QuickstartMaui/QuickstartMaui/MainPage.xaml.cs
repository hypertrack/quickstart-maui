namespace QuickstartMaui;

using System.Text;
using HyperTrack;
using Microsoft.Maui.ApplicationModel.DataTransfer;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

# if ANDROID
		var osSuffix = "android";
# endif
# if IOS
		var osSuffix = "ios";
# endif
		HyperTrack.WorkerHandle = "test_worker_quickstart_maui_" + osSuffix;

		DeviceIdLabel.Text = HyperTrack.DeviceId;
		WorkerHandleLabel.Text = HyperTrack.WorkerHandle;
	}

	private void OnAddGeotagClicked(object sender, EventArgs e)
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
		}
	}

	private void OnGetOrdersClicked(object sender, EventArgs e)
	{
		Dictionary<string, HyperTrack.Order> orders = HyperTrack.Orders;
		ResultLabel.Text = "Orders:\n" + GetOrdersText(orders);
	}

	private void OnDeviceIdLabelTapped(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(DeviceIdLabel.Text);
		ResultLabel.Text = "Device ID copied to clipboard.";
	}

	private string GetOrdersText(Dictionary<string, HyperTrack.Order> orders)
	{
		StringBuilder sb = new StringBuilder();
		foreach (KeyValuePair<string, HyperTrack.Order> kv in orders)
		{
			HyperTrack.Order order = kv.Value;
			sb.Append(order.OrderHandle + ",\nisInsideGeofence: " + GetIsInsideGeofenceText(order.IsInsideGeofence) + "\n");
		}
		return sb.ToString();
	}

	private string GetIsInsideGeofenceText(HyperTrack.Result<bool, HyperTrack.LocationError> isInsideGeofence)
	{
		if (isInsideGeofence.IsSuccess)
		{
			return isInsideGeofence.Success.ToString();
		}
		else
		{
			return GetLocationErrorText(isInsideGeofence.Failure);
		}
	}

	private string GetLocationErrorText(HyperTrack.LocationError locationError)
	{
		switch (locationError)
		{
			case HyperTrack.LocationError.NotRunning:
			case HyperTrack.LocationError.Starting:
				return locationError.ToString();
			case HyperTrack.LocationError.Errors errors:
				return string.Join(", ", errors.ErrorSet.Select(error => error.ToString()));
			default:
				throw new InvalidOperationException("Unknown location error type.");
		}
	}


}


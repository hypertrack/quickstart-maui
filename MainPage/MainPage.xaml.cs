using System.Diagnostics.CodeAnalysis;

namespace QuickstartMaui;

using System.Text;
using HyperTrack;
using Microsoft.Maui.ApplicationModel.DataTransfer;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

# if ANDROID
		const string osSuffix = "android";
# endif
# if IOS
		const string osSuffix = "ios";
# endif
		HyperTrack.WorkerHandle = "test_worker_quickstart_maui_" + osSuffix;

		DeviceIdLabel.Text = HyperTrack.DeviceId;
		WorkerHandleLabel.Text = HyperTrack.WorkerHandle;
	}

	private void OnAddGeotagClicked(object sender, EventArgs e)
	{
		var data = new Dictionary<string, object?>
		{
			{ "testKey", "testValue" }
		};
		var json = HyperTrack.Json.FromDictionary(data);

		var result = HyperTrack.AddGeotag(
			"orderHandle",
			new HyperTrack.OrderStatus.ClockIn(),
			json
		);

		if (result.IsSuccess)
		{
			var location = result.Success;
			ResultLabel.Text = "Location: " + "Latitude: " + location.Latitude + ", Longitude: " + location.Longitude;
		}
		else
		{
			ResultLabel.Text = result.Failure != null ? result.Failure.ToString()! : "Error";
		}
	}

	private void OnGetOrdersClicked(object sender, EventArgs e)
	{
		var orders = HyperTrack.Orders;
		ResultLabel.Text = "Orders:\n" + GetOrdersText(orders);
	}

	private void OnDeviceIdLabelTapped(object sender, EventArgs e)
	{
		Clipboard.SetTextAsync(DeviceIdLabel.Text);
		ResultLabel.Text = "Device ID copied to clipboard.";
	}

	private string GetOrdersText(Dictionary<string, HyperTrack.Order> orders)
	{
		var sb = new StringBuilder();
		foreach (var order in orders.Select(kv => kv.Value))
		{
			sb.Append(order.OrderHandle + ",\nisInsideGeofence: " + GetIsInsideGeofenceText(order.IsInsideGeofence) + "\n");
		}
		return sb.ToString();
	}

	private static string GetIsInsideGeofenceText(HyperTrack.Result<bool, HyperTrack.LocationError> isInsideGeofence)
	{
		return isInsideGeofence.IsSuccess ? isInsideGeofence.Success.ToString() : GetLocationErrorText(isInsideGeofence.Failure);
	}

	private static string GetLocationErrorText(HyperTrack.LocationError locationError)
	{
		return locationError switch
		{
			HyperTrack.LocationError.NotRunning or HyperTrack.LocationError.Starting => locationError.ToString(),
			HyperTrack.LocationError.Errors errors => string.Join(", ",
				errors.ErrorSet.Select(error => error.ToString())),
			_ => throw new InvalidOperationException("Unknown location error type.")
		};
	}

}


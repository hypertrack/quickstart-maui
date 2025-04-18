using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Maui.Devices; // Add this using directive

namespace QuickstartMaui;

using System.Text;
using HyperTrack;
using Microsoft.Maui.ApplicationModel.DataTransfer;

public partial class MainPage : ContentPage
{
	public static MainPage? Instance { get; private set; }

	private HyperTrack.ICancellable? _ordersSubscription;
	private HyperTrack.ICancellable? _locationSubscription;
	private HyperTrack.ICancellable? _isTrackingSubscription;
	private HyperTrack.ICancellable? _isAvailableSubscription;
	private HyperTrack.ICancellable? _errorsSubscription;

	public MainPage()
	{
		Instance = this;
		InitializeComponent();
		InitializeControls();
		SetupSubscriptions();
	}
	
	private void InitializeControls()
	{
		DeviceIdLabel.Text = HyperTrack.DeviceId;
		IsTrackingLabel.Text = HyperTrack.IsTracking.ToString();
		IsAvailableLabel.Text = HyperTrack.IsAvailable.ToString();

		// Set initial name
		HyperTrack.Name = "Quickstart Maui";

		// Set initial metadata
		var metadataValue = new Dictionary<string, object?>
		{
			{ "source", "Quickstart Maui" },
			{ "employee_id", Random.Shared.Next(10000) }
		};
		var metadata = HyperTrack.Json.FromDictionary(metadataValue);
		// the value will be null if provided dictionary is not JSON serializable
		if (metadata != null)
		{
			HyperTrack.Metadata = metadata;
		}
		else
		{
			// Handle error
			throw new InvalidOperationException("Failed to set metadata");
		}

		// Set worker handle
		HyperTrack.WorkerHandle = $"test_worker_quickstart_maui_{DeviceInfo.Platform.ToString().ToLower()}";
		WorkerHandleLabel.Text = HyperTrack.WorkerHandle;
	}

	private void SetupSubscriptions()
	{
		_ordersSubscription = HyperTrack.SubscribeToOrders(orders =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				OrdersLabel.Text = GetOrdersText(orders);
			});
		});

		_isTrackingSubscription = HyperTrack.SubscribeToIsTracking(isTracking =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				IsTrackingLabel.Text = isTracking.ToString();
			});
		});

		_isAvailableSubscription = HyperTrack.SubscribeToIsAvailable(isAvailable =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				IsAvailableLabel.Text = isAvailable.ToString();
			});
		});

		_errorsSubscription = HyperTrack.SubscribeToErrors(errors =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				ErrorsLabel.Text = GetErrorsText(errors);
			});
		});

		_locationSubscription = HyperTrack.SubscribeToLocation(locationResult =>
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (locationResult.IsSuccess)
				{
					LocationLabel.Text = $"Location: [{locationResult.Success.Latitude}, {locationResult.Success.Longitude}]";
				}
				else
				{
					LocationLabel.Text = GetLocationErrorText(locationResult.Failure);
				}
			});
		});
	}

	private async void OnDeviceIdLabelTapped(object sender, EventArgs e)
	{
		await Clipboard.SetTextAsync(DeviceIdLabel.Text);
		await DisplayAlert("Success", "Device ID copied to clipboard", "OK");
	}

	private void OnStartTrackingClicked(object sender, EventArgs e)
	{
		HyperTrack.IsTracking = true;
	}

	private void OnStopTrackingClicked(object sender, EventArgs e)
	{
		HyperTrack.IsTracking = false;
	}

	private void OnSetAvailableClicked(object sender, EventArgs e)
	{
		HyperTrack.IsAvailable = true;
	}

	private void OnSetUnavailableClicked(object sender, EventArgs e)
	{
		HyperTrack.IsAvailable = false;
	}

	private async void OnAddGeotagClicked(object sender, EventArgs e)
	{
		var geotagPayload = new Dictionary<string, object>
		{
			{ "payload", "Quickstart Maui" },
			{ "value", Random.Shared.NextDouble() }
		};

		var result = HyperTrack.AddGeotag(
			"test_order",
			new HyperTrack.OrderStatus.Custom("test_status"),
			HyperTrack.Json.FromDictionary(geotagPayload)
		);

		if (result.IsSuccess)
		{
			await DisplayAlert("Success",
				$"Geotag added at:\nLatitude: {result.Success.Latitude}\nLongitude: {result.Success.Longitude}",
				"OK");
		}
		else
		{
			await DisplayAlert("Error",
				$"Failed to add geotag: {result.Failure}",
				"OK");
		}
	}

	private async void OnAddGeotagWithExpectedLocationClicked(object sender, EventArgs e)
	{
		var geotagPayload = new Dictionary<string, object>
		{
			{ "payload", "Quickstart Maui" },
			{ "value", Random.Shared.NextDouble() }
		};

		var result = HyperTrack.AddGeotag(
			"test_order",
			new HyperTrack.OrderStatus.Custom("test_status"),
			HyperTrack.Json.FromDictionary(geotagPayload),
			new HyperTrack.Location(37.7749, -122.4194)
		);

		if (result.IsSuccess)
		{
			await DisplayAlert("Success",
				$"Geotag added at:\nLatitude: {result.Success.Location.Latitude}\nLongitude: {result.Success.Location.Longitude}\nDeviation: {result.Success.Deviation}",
				"OK");
		}
		else
		{
			await DisplayAlert("Error",
				$"Failed to add geotag: {result.Failure}",
				"OK");
		}
	}

	private async void OnLocateClicked(object sender, EventArgs e)
	{
		HyperTrack.Locate(async (result) =>
		{
			if (result.IsSuccess)
			{
				await DisplayAlert("Location",
					$"Location: [{result.Success.Latitude}, {result.Success.Longitude}]",
					"OK");
			}
			else
			{
				await DisplayAlert("Error", GetErrorsText(result.Failure), "OK");
			}
		});

	}

	private async void OnGetErrorsClicked(object sender, EventArgs e)
	{
		var errors = HyperTrack.Errors;
		await DisplayAlert("Errors", GetErrorsText(errors), "OK");
	}

	private async void OnGetIsAvailableClicked(object sender, EventArgs e)
	{
		var isAvailable = HyperTrack.IsAvailable;
		await DisplayAlert("isAvailable", isAvailable.ToString(), "OK");
	}

	private async void OnGetIsTrackingClicked(object sender, EventArgs e)
	{
		var isTracking = HyperTrack.IsTracking;
		await DisplayAlert("isTracking", isTracking.ToString(), "OK");
	}

	private async void OnGetLocationClicked(object sender, EventArgs e)
	{
		var location = HyperTrack.GetLocation();
		if (location.IsSuccess)
		{
			await DisplayAlert("Location",
				$"Location: [{location.Success.Latitude}, {location.Success.Longitude}]",
				"OK");
		}
		else
		{
			await DisplayAlert("Error", GetLocationErrorText(location.Failure), "OK");
		}
	}

	private async void OnGetMetadataClicked(object sender, EventArgs e)
	{
		var metadata = HyperTrack.Metadata;
		await DisplayAlert("Metadata", metadata.ToString(), "OK");
	}

	private async void OnGetNameClicked(object sender, EventArgs e)
	{
		var name = HyperTrack.Name;
		await DisplayAlert("Name", name, "OK");
	}

	private async void OnGetOrdersClicked(object sender, EventArgs e)
	{
		var orders = HyperTrack.Orders;
		await DisplayAlert("Orders", GetOrdersText(orders), "OK");
	}

	private void OnAllowMockLocationClicked(object sender, EventArgs e)
	{
		HyperTrack.AllowMockLocation = true;
	}

	private void OnDisallowMockLocationClicked(object sender, EventArgs e)
	{
		HyperTrack.AllowMockLocation = false;
	}

	private async void OnGetAllowMockLocationClicked(object sender, EventArgs e)
	{
		var allowMockLocation = HyperTrack.AllowMockLocation;
		await DisplayAlert("AllowMockLocation", allowMockLocation.ToString(), "OK");
	}

	private static string GetOrdersText(Dictionary<string, HyperTrack.Order> orders)
	{
		if (orders.Count == 0) return "No orders";

		var sb = new StringBuilder();
		foreach (var (handle, order) in orders)
		{
			sb.AppendLine($"Order: {handle}");
			var isInsideGeofence = order.IsInsideGeofence;
			sb.AppendLine($"IsInsideGeofence: {(isInsideGeofence.IsSuccess ? isInsideGeofence.Success.ToString() : GetLocationErrorText(isInsideGeofence.Failure))}");
		}
		return sb.ToString().TrimEnd();
	}

	private static string GetLocationErrorText(HyperTrack.LocationError error)
	{
		return error switch
		{
			HyperTrack.LocationError.NotRunning => "Not running",
			HyperTrack.LocationError.Starting => "Starting",
			HyperTrack.LocationError.Errors errors => $"Errors:\n{GetErrorsText(errors.ErrorSet)}", // Fix errors.Value to errors.ErrorSet
			_ => "Unknown error"
		};
	}

	private static string GetErrorsText(HashSet<HyperTrack.Error> errors)
	{
		return errors.Count == 0 ? "No errors" : string.Join("\n", errors);
	}
}

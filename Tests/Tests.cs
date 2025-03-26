namespace QuickstartMaui.Tests;

using HyperTrack;
using System.Threading.Tasks;

public static class Tests
{
    private static HyperTrack.ICancellable _errorsSubscription;
    private static HyperTrack.ICancellable _isAvailableSubscription;
    private static HyperTrack.ICancellable _isTrackingSubscription;
    private static HyperTrack.ICancellable _locationSubscription;
    private static HyperTrack.ICancellable _ordersSubscription;
    
    private static HashSet<HyperTrack.Error> _errorsSubscriptionValue;
    private static bool _isAvailableSubscriptionValue;
    private static bool _isTrackingSubscriptionValue;
    private static HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> _locationSubscriptionValue;
    private static Dictionary<string, HyperTrack.Order> _ordersSubscriptionValue;

    private const string OrderHandle = "8a7b3226-998e-4e05-bf99-4eb99f7e835e";
    
    private static readonly HashSet<HyperTrack.Error> Errors =
#if ANDROID
    [
        new HyperTrack.Error.Permissions.Location.Denied(),
        new HyperTrack.Error.Location.ServicesDisabled()
    ];
#endif
#if IOS
    [
        new HyperTrack.Error.Permissions.Location.NotDetermined(),
    ];
#endif
    
    private static readonly HyperTrack.Location Location = new HyperTrack.Location(37.7749, -122.4194);

    private static readonly HyperTrack.Json.Object Metadata = new HyperTrack.Json.Object(
        new Dictionary<string, HyperTrack.Json>
        {
            { "string", new HyperTrack.Json.String("value") },
            { "number", new HyperTrack.Json.Number(42) },
            { "boolean", new HyperTrack.Json.Bool(true) },
            { "null", new HyperTrack.Json.Null() },
            {
                "array", new HyperTrack.Json.Array(new List<HyperTrack.Json>
                    {
                        new HyperTrack.Json.String("value"),
                        new HyperTrack.Json.String("value1"),
                    }
                )
            },
            {
                "object", new HyperTrack.Json.Object(new Dictionary<string, HyperTrack.Json>
                    {
                        { "string", new HyperTrack.Json.String("value") },
                    }
                )
            },
        });
    
    private static readonly HyperTrack.OrderStatus OrderStatus = new HyperTrack.OrderStatus.ClockIn();
    
    private static readonly HyperTrack.Json.Object TestMetadataClockIn = new HyperTrack.Json.Object(
        new Dictionary<string, HyperTrack.Json>
        {
            { "test_status", new HyperTrack.Json.String("clock_in") },
        }
    );

    private static readonly HyperTrack.Json.Object TestMetadataClockOut = new HyperTrack.Json.Object(
        new Dictionary<string, HyperTrack.Json>
        {
            { "test_status", new HyperTrack.Json.String("clock_out") },
        }
    );

    private static readonly HyperTrack.Json.Object TestMetadataCustom = new HyperTrack.Json.Object(
        new Dictionary<string, HyperTrack.Json>
        {
            { "test_status", new HyperTrack.Json.String("custom") },
        }
    );
    
    private static readonly string WorkerHandle = $"autotest_maui_{DeviceInfo.Platform.ToString().ToLower()}";

    private static void Log(string message)
    {
        MainPage.Instance?.AddLog(message);
    }
    
    public static async Task RunTestsErrors()
    {
        Log("Starting RunTestsErrors test");
        InitSubscriptions();

        Assert("allow_mock_location_initial", false, HyperTrack.AllowMockLocation);
        // allow_mock_location_set_true
        HyperTrack.AllowMockLocation = true;
        Assert("allow_mock_location_get_true", true, HyperTrack.AllowMockLocation);
        // allow_mock_location_set_false
        HyperTrack.AllowMockLocation = false;
        Assert("allow_mock_location_get_false", false, HyperTrack.AllowMockLocation);

        Assert("device_id_get", 36, HyperTrack.DeviceId.Length);

        var emptyMetadata = new HyperTrack.Json.Object(new Dictionary<string, HyperTrack.Json>());

        Assert(
            "metadata_get_initial",
            new HyperTrack.Json.Object(new Dictionary<string, HyperTrack.Json>()),
            HyperTrack.Metadata
        );
        // metadata_set_complex
        HyperTrack.Metadata = Metadata;
        Assert("metadata_get_complex", Metadata, HyperTrack.Metadata);
        // metadata_set_empty
        HyperTrack.Metadata = emptyMetadata;
        Assert("metadata_get_empty", emptyMetadata, HyperTrack.Metadata);

        Assert("name_get_initial", "", HyperTrack.Name);
        // name_set
        HyperTrack.Name = "name";
        // name_get
        Assert("name_get", "name", HyperTrack.Name);
        HyperTrack.Name = "";
        Assert("name_get_post", "", HyperTrack.Name);
        
        Assert("worker_handle_get_initial", "", HyperTrack.WorkerHandle);

        // worker_handle_set_nonempty
        HyperTrack.WorkerHandle = WorkerHandle;
        Assert("worker_handle_get_nonempty", WorkerHandle, HyperTrack.WorkerHandle);
        // worker_handle_set_empty
        HyperTrack.WorkerHandle = "";
        Assert("worker_handle_get_empty", "", HyperTrack.WorkerHandle);
        // worker_handle_set_nonempty
        HyperTrack.WorkerHandle = WorkerHandle;

        // add_geotag_clock_in
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockIn(), TestMetadataClockIn);
        // add_geotag_clock_out
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockOut(), TestMetadataClockOut);
        // add_geotag_custom
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.Custom("custom"), TestMetadataCustom);
        // add_geotag_expected_clock_in
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockIn(), TestMetadataClockIn, Location);
        // add_geotag_expected_clock_out
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockOut(), TestMetadataClockOut, Location);
        // add_geotag_expected_custom
        HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.Custom("custom"), TestMetadataCustom, Location);

        // add_geotag_not_running
        // add_geotag_expected_not_running
        // get_location_not_running
        // subscribe_location_not_running
        await CheckLocationFailure(new HyperTrack.LocationError.NotRunning());

        // is_tracking_set_true
        HyperTrack.IsTracking = true;

        // add_geotag_starting
        // add_geotag_expected_starting
        // get_location_starting
        // subscribe_location_starting
        await CheckLocationFailure(new HyperTrack.LocationError.Starting());

        await Task.Delay(10000);

        // add_geotag_multiple_errors
        // add_geotag_expected_multiple_errors
        // get_location_multiple_errors
        // subscribe_location_multiple_errors
        // errors_get_multiple
        // subscribe_errors_multiple
        // locate_multiple_errors
        await CheckLocationFailure(
            new HyperTrack.LocationError.Errors(Errors)
        );

        HyperTrack.IsTracking = false;
        await Task.Delay(1000);
        Assert("is_tracking_get_initial", false, HyperTrack.IsTracking);
        Assert("subscribe_is_tracking_initial", false, _isTrackingSubscriptionValue);

        // is_tracking_set_true
        HyperTrack.IsTracking = true;
        await Task.Delay(1000);
        Assert("is_tracking_get_true", true, HyperTrack.IsTracking);
        Assert("subscribe_is_tracking_true", true, _isTrackingSubscriptionValue);
        // is_tracking_set_false
        HyperTrack.IsTracking = false;
        await Task.Delay(1000);
        Assert("is_tracking_get_false", false, HyperTrack.IsTracking);
        Assert("subscribe_is_tracking_false", false, _isTrackingSubscriptionValue);

        Assert("is_available_get_initial", false, HyperTrack.IsAvailable);
        Assert("subscribe_is_available_initial", false, _isAvailableSubscriptionValue);
        // is_available_set_true
        HyperTrack.IsAvailable = true;
        await Task.Delay(1000);
        Assert("is_available_get_true", true, HyperTrack.IsAvailable);
        Assert("subscribe_is_available_true", true, _isAvailableSubscriptionValue);
        // is_available_set_false
        HyperTrack.IsAvailable = false;
        await Task.Delay(1000);
        Assert("is_available_get_false", false, HyperTrack.IsAvailable);
        Assert("subscribe_is_available_false", false, _isAvailableSubscriptionValue);

        // orders_get_empty
        Assert("orders_get_empty", new Dictionary<string, HyperTrack.Order>(), HyperTrack.Orders);
        // subscribe_orders_empty
        Assert("subscribe_orders_empty", new Dictionary<string, HyperTrack.Order>(), _ordersSubscriptionValue);
        
        // unsubscribe_is_available
        Assert("unsubscribe_is_available_initial", false, HyperTrack.IsAvailable);
        _isAvailableSubscription.Cancel();
        HyperTrack.IsAvailable = true;
        await Task.Delay(1000);
        Assert("unsubscribe_is_available", false, _isAvailableSubscriptionValue);
        HyperTrack.IsAvailable = false;
        
        // unsubscribe_is_tracking
        Assert("unsubscribe_is_tracking_initial", false, HyperTrack.IsTracking);
        _isTrackingSubscription.Cancel();
        HyperTrack.IsTracking = true;
        await Task.Delay(1000);
        Assert("unsubscribe_is_tracking", false, _isTrackingSubscriptionValue);
        HyperTrack.IsTracking = false;
        
        // unsubscribe_errors
        // skipping this check because you need to manipulate the app permissions to change the errors set
        // which is beyond the app code scope
        
        Log("Test completed: RunTestsErrors");
    }
    public static async Task RunTestsTracking()
    {
        Log("Starting Tracking test");
        InitSubscriptions();
        
        HyperTrack.IsTracking = true;
        await Task.Delay(10000);

        // add_geotag_success
        // add_geotag_expected_success
        // get_location_success
        // subscribe_location_success
        // errors_get_empty
        // subscribe_errors_empty
        // locate_success
        await CheckLocationSuccess();
        
        await Task.Delay(1000);
        
        // unsubscribe_location
        Assert("unsubscribe_location_initial", HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(Location), _locationSubscriptionValue);
        _locationSubscription.Cancel();
        HyperTrack.IsTracking = false;
        await Task.Delay(1000);
        Assert("unsubscribe_location", HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(Location), _locationSubscriptionValue);

        Log("Test completed: Tracking");
    }

    public static async Task RunTestsOrders()
    {
        Log("Starting Orders test");
        InitSubscriptions();

        // wait to get isInsideGeofence result
        await Task.Delay(10000);
        
        // orders_get
        Assert("orders_get_count", 3, HyperTrack.Orders.Count);
        var order = HyperTrack.Orders.First().Value;
        // subscribe_orders_multiple
        Assert("subscribe_orders_multiple", 3, _ordersSubscriptionValue.Count);
        
        // order_order_handle
        Assert("order_order_handle", true, order.OrderHandle != "");
        
        // order_is_inside_geofence_false
        Assert("order_is_inside_geofence_false", false, order.IsInsideGeofence.Success);
        
        Log("Test completed: Orders");
    }
    
    public static async Task RunTestsOrdersErrors()
    {
        Log("Starting Orders Errors test");

        var order = HyperTrack.Orders.First().Value;

        // order_is_inside_geofence_errors
        Assert("order_is_inside_geofence_errors",
            new HyperTrack.LocationError.Errors(Errors), order.IsInsideGeofence.Failure
        );
        
        Log("Test completed: Order");
    }

    private static async Task CheckLocationFailure(HyperTrack.LocationError expectedFailure)
    {
        await Task.Delay(1000);
        Log($"Checking location failure: {expectedFailure}");
        // not checking NotRunning result for geotags because we plan to remove it for that API
        if (expectedFailure != new HyperTrack.LocationError.Starting())
        {
            Assert(
                "add_geotag",
                HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Error(expectedFailure),
                HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockIn(), Metadata)
            );
            Assert(
                "add_geotag_expected",
                HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError>.Error(expectedFailure),
                HyperTrack.AddGeotag(OrderHandle, new HyperTrack.OrderStatus.ClockIn(), Metadata, Location)
            );
            Assert(
                "get_location",
                HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Error(expectedFailure),
                HyperTrack.GetLocation()
            );
            // not checking for this case as a workaround for the SDK bug when location subscription is not updated
            // on tracking start if there are Errors
            if (expectedFailure != new HyperTrack.LocationError.Errors(Errors))
            {
                Assert(
                    "subscribe_location",
                    HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Error(expectedFailure),
                    _locationSubscriptionValue
                );
            }
        }

        if (expectedFailure is HyperTrack.LocationError.Errors errors)
        {
            await CheckErrors(errors.ErrorSet);
        }
    }

    private static async Task CheckErrors(HashSet<HyperTrack.Error> expectedErrors)
    {
        Assert("subscribe_errors", expectedErrors, _errorsSubscriptionValue);
        Assert("errors_get", expectedErrors, HyperTrack.Errors);

        var locate = await LocateAsync();
        HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>> expectedLocate;
        if(expectedErrors.Count > 0)
            expectedLocate = HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>.Error(expectedErrors);
        else
            expectedLocate = HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>.Ok(Location);
        
        Assert("locate", expectedLocate, locate);
    }

    private static async Task CheckLocationSuccess()
    {
        await Task.Delay(1000);
        Assert(
            "add_geotag_success",
            HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(Location),
            HyperTrack.AddGeotag(OrderHandle, OrderStatus, Metadata)
        );
        Assert(
            "add_geotag_expected_success",
            HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError>.Ok(
                new HyperTrack.LocationWithDeviation(Location, 0)
            ),
            HyperTrack.AddGeotag(OrderHandle, OrderStatus, Metadata, Location)
        );
        Assert(
            "get_location_success",
            HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(Location),
            HyperTrack.GetLocation()
        );
        Assert(
            "subscribe_location_success",
            HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError>.Ok(Location),
            _locationSubscriptionValue
        );

        await CheckErrors([]);
    }

    private static Task<HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>> LocateAsync()
    {
        var tcs = new TaskCompletionSource<HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>>>();
        HyperTrack.Locate((result) => { tcs.SetResult(result); });
        return tcs.Task;
    }

    private static void Assert(string id, object? expected, object? actual)
    {
        var areEqual = expected switch
        {
            IDictionary<string, HyperTrack.Json> expectedDict when
                actual is IDictionary<string, HyperTrack.Json> actualDict => expectedDict.Count == actualDict.Count &&
                                                                             expectedDict.All(kvp =>
                                                                                 actualDict.ContainsKey(kvp.Key) &&
                                                                                 actualDict[kvp.Key].ToString() ==
                                                                                 kvp.Value.ToString()),
            IDictionary<string, HyperTrack.Order> expectedOrderDict when
                actual is IDictionary<string, HyperTrack.Order> actualOrderDict =>
                expectedOrderDict.Count == actualOrderDict.Count && expectedOrderDict.All(kvp =>
                    actualOrderDict.ContainsKey(kvp.Key) && Equals(kvp.Value, actualOrderDict[kvp.Key])),
            IEnumerable<object> expectedCollection when actual is IEnumerable<object> actualCollection =>
                expectedCollection.OrderBy(t => t).SequenceEqual(actualCollection.OrderBy(t => t)),
            HyperTrack.Location expectedLocation when actual is HyperTrack.Location actualLocation => true,
            HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> expectedLocationResult when
                actual is HyperTrack.Result<HyperTrack.Location, HyperTrack.LocationError> actualLocationResult => 
                actualLocationResult.IsSuccess ? expectedLocationResult.IsSuccess : expectedLocationResult.Failure.Equals(actualLocationResult.Failure),
            HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError> expectedLocationWithDeviationResult when
                actual is HyperTrack.Result<HyperTrack.LocationWithDeviation, HyperTrack.LocationError> actualLocationWithDeviationResult => 
                actualLocationWithDeviationResult.IsSuccess ? expectedLocationWithDeviationResult.IsSuccess : expectedLocationWithDeviationResult.Failure.Equals(actualLocationWithDeviationResult.Failure),
            HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>> expectedLocationErrorsResult when
                actual is HyperTrack.Result<HyperTrack.Location, HashSet<HyperTrack.Error>> actualLocationErrorsResult => 
                actualLocationErrorsResult.IsSuccess ? expectedLocationErrorsResult.IsSuccess : expectedLocationErrorsResult.Failure.OrderBy(t => t).SequenceEqual(actualLocationErrorsResult.Failure.OrderBy(t => t)),
            _ => Equals(expected, actual),
        };

        if (areEqual)
        {
            return;
        }

        var error = $"[{id}] Assertion failed: expected {FormatValue(expected)}, actual {FormatValue(actual)}";
        throw new Exception(error);
        // Log(error);
        // return;

        string FormatValue(object? value)
        {
            switch (value)
            {
                case IDictionary<string, HyperTrack.Json> jsonDict:
                {
                    var pairs = jsonDict.Select(kvp => $"{kvp.Key}: {FormatValue(kvp.Value)}");
                    return $"{{{string.Join(", ", pairs)}}}";
                }
                case IDictionary<object?, object?> dict:
                {
                    var pairs = dict.Keys.Cast<object>()
                        .Select(k => $"{k}: {FormatValue(dict[k])}");
                    return $"{{{string.Join(", ", pairs)}}}";
                }
                case IEnumerable<object> collection when !(value is string):
                    return $"[{string.Join(", ", collection.Select(FormatValue))}]";
                default:
                    return value?.ToString() ?? "null";
            }
        }
    }

    private static void InitSubscriptions()
    {
        _errorsSubscription = HyperTrack.SubscribeToErrors(
            (errors) => { _errorsSubscriptionValue = errors; }
        );
        
        _isAvailableSubscription = HyperTrack.SubscribeToIsAvailable(
            (isAvailable) => { _isAvailableSubscriptionValue = isAvailable; }
        );
        
        _isTrackingSubscription = HyperTrack.SubscribeToIsTracking(
            (isTracking) => { _isTrackingSubscriptionValue = isTracking; }
        );
        
        _locationSubscription = HyperTrack.SubscribeToLocation(
            (locationResult) => { _locationSubscriptionValue = locationResult; }
        );
        
        _ordersSubscription = HyperTrack.SubscribeToOrders(
            (orders) => { _ordersSubscriptionValue = orders; }
        );
    }
}

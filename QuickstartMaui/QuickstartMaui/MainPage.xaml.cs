namespace QuickstartMaui;

#if ANDROID
using Com.Hypertrack.Sdk.Android;
#endif

#if IOS
// using UIKit;
#endif

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();

		#if ANDROID
		DeviceIdLabel.Text = HyperTrack.DeviceID;
		#endif

		#if IOS
		DeviceIdLabel.Text = "iOS Device ID";
		#endif
	}

	// private void OnCounterClicked(object sender, EventArgs e)
	// {
	// 	count++;

	// 	if (count == 1)
	// 		CounterBtn.Text = $"Clicked {count} time";
	// 	else
	// 		CounterBtn.Text = $"Clicked {count} times";

	// 	SemanticScreenReader.Announce(CounterBtn.Text);
	// }
}


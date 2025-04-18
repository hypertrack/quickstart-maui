# MAUI Quickstart for HyperTrack SDK

[![GitHub](https://img.shields.io/github/license/hypertrack/quickstart-maui?color=orange)](./LICENSE)
[![NuGet](https://img.shields.io/nuget/v/HyperTrack.SDK.MAUI.svg)](https://www.nuget.org/packages/HyperTrack.SDK.MAUI)

[HyperTrack](https://www.hypertrack.com/) lets you add live location tracking to your mobile app. Live location is made available along with ongoing activity, tracking controls and tracking outage with reasons.

This repo contains an example MAUI app that has everything you need to get started.

For information about how to get started with MAUI SDK, please check this [Guide](https://www.hypertrack.com/docs/install-sdk-maui).

## How to get started?

### Create HyperTrack Account

[Sign up](https://dashboard.hypertrack.com/signup) for HyperTrack and get your publishable key from the [Setup page](https://dashboard.hypertrack.com/setup).

### Setup the environment

You need to [set up the development environment for .NET MAUI](https://learn.microsoft.com/en-us/dotnet/maui/get-started/installation)

### Setup the project

- Run `dotnet restore`
- Run `dotnet build`

### Set your publishable key

Follow the [instructions on setting up publishable key](https://hypertrack.com/docs/install-sdk-maui#set-the-publishable-key) in our docs

### Setup silent push notifications

Follow the [instructions on setting up silent push notifications](https://hypertrack.com/docs/install-sdk-maui#set-up-silent-push-notifications) in our docs.

HyperTrack SDK needs Firebase Cloud Messaging and APNS to manage on-device tracking as well as enable using HyperTrack cloud APIs from your server to control the tracking.

### Run the app

- Android: run `dotnet build -t:Run -f net9.0-android`
- iOS: run `dotnet build -t:Run -f net9.0-ios`

### Grant permissions

[Grant required permissions to the app](https://hypertrack.com/docs/install-sdk-maui#grant-the-permissions-to-the-app)

### Start tracking

Press `Start tracking` button.

To see the device on a map, open the [HyperTrack dashboard](https://dashboard.hypertrack.com/).

The app will create a driver with driver handle `test_driver_quickstart_maui_<your platform>`

## Support

Join our [Slack community](https://join.slack.com/t/hypertracksupport/shared_invite/enQtNDA0MDYxMzY1MDMxLTdmNDQ1ZDA1MTQxOTU2NTgwZTNiMzUyZDk0OThlMmJkNmE0ZGI2NGY2ZGRhYjY0Yzc0NTJlZWY2ZmE5ZTA2NjI) for instant responses. You can also email us at help@hypertrack.com

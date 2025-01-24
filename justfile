alias al := add-plugin-local
alias alr := add-plugin-local-release
alias ap := add-plugin
alias b := build
alias ba := build-android
alias bi := build-ios
alias c := clean
alias ga := get-sdk-aars
alias ra := run-android
alias ri := run-ios
alias rls := remove-local-source

add-plugin version: clean
    dotnet remove QuickstartMaui.csproj reference ../sdk-maui/HyperTrackSdkMaui/HyperTrackSdkMaui.csproj || true
    dotnet add package HyperTrack.SDK.MAUI --version {{version}}

add-plugin-local: clean
    dotnet remove QuickstartMaui.csproj reference HyperTrack.SDK.MAUI
    dotnet add QuickstartMaui.csproj reference ../sdk-maui/HyperTrackSdkMaui/HyperTrackSdkMaui.csproj

add-plugin-local-release version: clean
    #!/usr/bin/env sh
    set -euo pipefail
    PWD=$(pwd)

    just remove-local-source || true
    dotnet nuget add source "$PWD/../sdk-maui/HyperTrackSdkMaui/bin/Release" --name HyperTrackSdkMauiLocalSource
    dotnet add package HyperTrack.SDK.MAUI --version {{version}}

build:
    dotnet build

build-android:
    # assuming JAVA_HOME="/opt/homebrew/opt/openjdk@17"
    dotnet build -t:Run -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="$JAVA_HOME/libexec/openjdk.jdk/Contents/Home"

build-ios:
    # dotnet build -t:Run -v diag --debug -f net9.0-ios
    dotnet build -t:Run -f net9.0-ios

clean:
    dotnet clean
    rm -rf bin
    rm -rf obj
    rm -rf Debug

get-sdk-aars:
    #!/usr/bin/env sh
    set -euo pipefail

    just -f ../mobile/android/sdk/justfile export-sdk-aars

    cp -rf ../mobile/android/sdk/sdk-aars/* ../sdk-maui/HyperTrackSdkMaui.AndroidBinding/Aars

    just -f ../sdk-maui/justfile clean
    just clean
    just -f ../sdk-maui/justfile build-android

remove-local-source:
    dotnet nuget remove source HyperTrackSdkMauiLocalSource || true

run-android: build-android

run-ios: build-ios

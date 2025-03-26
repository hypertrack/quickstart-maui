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

JAVA_HOME := "/opt/homebrew/opt/openjdk@17"

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
    dotnet build -t:Build -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_HOME}}"

build-ios:
    # add -v diag --debug for verbosity
    # add -r ios-arm64 to build for read device
    dotnet build QuickstartMaui.csproj -t:Build -f net9.0-ios -p:Configuration=Release -p:MtouchUseLlvm=false
    
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

run-android: 
    dotnet build -t:Build -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_HOME}}"

run-ios: #build-ios
    #!/usr/bin/env sh
    set -euo pipefail

    dotnet build QuickstartMaui.csproj -t:Run -f net9.0-ios -p:Configuration=Release -p:MtouchUseLlvm=false -r ios-arm64
    # /usr/local/share/dotnet/packs/Microsoft.iOS.Sdk.net9.0_18.2/18.2.9173/tools/bin/mlaunch --installdev bin/Release/net9.0-ios/ios-arm64/QuickstartMaui.app/ --wait-for-exit:false
    # /usr/local/share/dotnet/packs/Microsoft.iOS.Sdk.net9.0_18.2/18.2.9173/tools/bin/mlaunch --launchdev bin/Release/net9.0-ios/ios-arm64/QuickstartMaui.app/ --devname 55513da5cd9f0628de47a812a956ad9495132c4c --stdout /dev/ttys001 --stderr /dev/ttys001

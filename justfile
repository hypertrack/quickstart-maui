alias al := add-plugin-local
alias alr := add-plugin-local-release
alias ap := add-plugin
alias b := build
alias ba := build-android
alias bi := build-ios
alias c := clean
alias ra := run-android
alias ri := run-ios
alias rls := remove-local-source

JAVA_HOME := "/opt/homebrew/opt/openjdk@17/libexec/openjdk.jdk/Contents/Home"
LOCAL_REPO_NAME := "HyperTrackSdkMauiLocalSource"
MAUI_SDK := "../sdk-maui/HyperTrackSdkMaui"
MAUI_SDK_PROJECT := "HyperTrackSdkMaui.csproj"
MOBILE_ROOT := "../../.."
PROJECT_NAME := "QuickstartMaui"

add-plugin version: clean
    dotnet remove {{PROJECT_NAME}}.csproj reference {{MAUI_SDK}}/{{MAUI_SDK_PROJECT}} || true
    dotnet add package HyperTrack.SDK.MAUI --version {{version}}

add-plugin-local: clean
    dotnet remove {{PROJECT_NAME}}.csproj reference HyperTrack.SDK.MAUI
    dotnet add {{PROJECT_NAME}}.csproj reference {{MAUI_SDK}}/{{MAUI_SDK_PROJECT}}

add-plugin-local-release version: clean
    #!/usr/bin/env sh
    set -euo pipefail

    just remove-local-source || true
    dotnet nuget add source "$(pwd)/{{MAUI_SDK}}/bin/Release" --name {{LOCAL_REPO_NAME}}
    dotnet add package HyperTrack.SDK.MAUI --version {{version}}

build: build-android build-ios

build-android:
    dotnet build -t:Build -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_HOME}}"

build-ios:
    # add -v diag --debug for verbosity
    # add -r ios-arm64 to build for real device
    dotnet build {{PROJECT_NAME}}.csproj -t:Build -f net9.0-ios -p:Configuration=Release -p:MtouchUseLlvm=false

clean:
    dotnet clean
    rm -rf bin
    rm -rf obj
    rm -rf Debug

_get_commit_hash:
    @git -C {{MOBILE_ROOT}} rev-parse --short HEAD

publish:
    #!/usr/bin/env sh
    set -euo pipefail

    MOBILE_ROOT=$(realpath "{{MOBILE_ROOT}}")
    QUICKSTART_ROOT="$MOBILE_ROOT/wrappers/maui/quickstart-maui"
    QUICKSTART_REPO_PATH="$MOBILE_ROOT/tmp/public-quickstart-maui"
    COMMIT_HASH=$(just _get_commit_hash)

    cd $QUICKSTART_REPO_PATH

    git fetch

    SOURCE_BRANCH="main-test"
    if git ls-remote --heads origin | grep -q $SOURCE_BRANCH; then
        git checkout $SOURCE_BRANCH
        git pull origin $SOURCE_BRANCH
    else
        git checkout --orphan $SOURCE_BRANCH
        git rm -rf ./* || true
    fi

    rm -rf *
    cp -rf $QUICKSTART_ROOT/* .
    git add -A
    git commit -m "Update the sources ($COMMIT_HASH)" || true
    git push --set-upstream origin $SOURCE_BRANCH

remove-local-source:
    dotnet nuget remove source {{LOCAL_REPO_NAME}} || true

run-android:
    dotnet build -t:Run -f net9.0-android -p:Configuration=Debug -p:JavaSdkDirectory="{{JAVA_HOME}}"

run-ios:
    #!/usr/bin/env sh
    set -euo pipefail

    dotnet build {{PROJECT_NAME}}.csproj -t:Run -f net9.0-ios -p:Configuration=Release -p:MtouchUseLlvm=false -r ios-arm64
    # /usr/local/share/dotnet/packs/Microsoft.iOS.Sdk.net9.0_18.2/18.2.9173/tools/bin/mlaunch --installdev bin/Release/net9.0-ios/ios-arm64/QuickstartMaui.app/ --wait-for-exit:false
    # /usr/local/share/dotnet/packs/Microsoft.iOS.Sdk.net9.0_18.2/18.2.9173/tools/bin/mlaunch --launchdev bin/Release/net9.0-ios/ios-arm64/QuickstartMaui.app/ --devname 55513da5cd9f0628de47a812a956ad9495132c4c --stdout /dev/ttys001 --stderr /dev/ttys001

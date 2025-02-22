alias b := build
alias ba := build-android
alias bi := build-ios
alias c := clean

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



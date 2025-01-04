@echo off

if not exist .\nuget mkdir .\nuget

cd TwitchLib.Api
dotnet restore
dotnet build -c Release --no-restore
dotnet pack TwitchLib.Api.sln -v normal -c Release -o nugets --no-build
xcopy .\nugets\*.nupkg ..\nuget /y

cd ..

cd TwitchLib.Client
dotnet restore
dotnet build -c Release --no-restore
dotnet pack TwitchLib.Client.sln -v normal -c Release -o nugets --no-build
xcopy .\nugets\*.nupkg ..\nuget /y

cd ..

cd TwitchLib.EventSub.Websockets
dotnet restore
dotnet build -c Release --no-restore
dotnet pack TwitchLib.EventSub.Websockets.sln -v normal -c Release -o nugets --no-build
xcopy .\nugets\*.nupkg ..\nuget /y

cd ..
nuget sources | findstr pleebo-tv-bot-packages || (dotnet nuget add source %cd%\nuget --name pleebo-tv-bot-packages)
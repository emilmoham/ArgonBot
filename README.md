# ArgonBot

There are many chat bots to choose from when joining the world of streaming, but
popular off the shelf solutions trade off control for convenience. Undobutedly, 
there exist numerous chat bot projects which offer both extreme levels of control 
and convenient UIs, but in my experience they always want something crazy, like
money. This project aims to implement some basic chat bot functions while 
offering complete control over how it is integrated with the stream.


## Development
This project depends on [TwitchLib](https://github.com/TwitchLib/TwitchLib).
However, several events and definitions are not available in the release version
of the the TwitchLib NuGet package available on nuget.org. The dev branches of 
these projects have the definitions required and have been added to this project
as submodules for development. This enables us to build with the latest features
of the libraries but means that getting the project set up takes a few extra
steps.

To get started:

1. Clone the repository, including the submodules
    ```sh
    git clone --recurse-submodules https://github.com/emilmoham/ArgonBot.git ArgonBot
    ```

2. Build the submodules into nuget packages[^1]
    * We're using submodules here because we need the dev versions of the 
      TwitchLib packages.
    ```sh
    .\build-nuget-packages.cmd
    ```

3. Open the solution in your editor of choice
4. Configure: TODO
5. Build and run.

[^1]: The `build-nuget-packages.cmd` will build and collect the nuget packages
for the included TwitchLib dependencies. It will also add a nuget package source
pointing to this repository with the name `argon-bot-packages`. If for any 
reason you ever want to have this nuget source removed. Run the 
`remove-custom-nuget-source.cmd` script.
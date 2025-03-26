namespace Trains.Scenes;

class RootScreen : ScreenObject
{
    private ScreenSurface mainSurface;

    public RootScreen()
    {
        mainSurface = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        mainSurface.Fill(Color.White, Color.Black);

        mainSurface.Print(1, 1, "hellooo :3");
        
        Children.Add(mainSurface);

    }
}
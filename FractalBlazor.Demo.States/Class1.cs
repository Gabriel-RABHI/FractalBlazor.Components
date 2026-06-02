namespace FractalBlazor.Demo.States
{
    public class RootState
    {
        public TopBarState TopBar { get; }

        public IEnumerable<MainMenuGroupState> MenuGroups { get; }

        public RootState()
        {
            TopBar = new TopBarState();
        }
    }

    public class TopBarState
    {

    }

    public class MainMenuGroupState
    {

    }

    public class MainMenuState
    {

    }
}

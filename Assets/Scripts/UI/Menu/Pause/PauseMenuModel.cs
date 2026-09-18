namespace UI.Menu.Pause
{
    public class PauseMenuModel
    {
        public bool IsOpen { get; private set; }

        public void SetOpen(bool value) => IsOpen = value;
    }
}
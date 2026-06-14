namespace TheAdventure;

public class Input
{
    private readonly HashSet<KeyCode> _down = new();
    private readonly HashSet<KeyCode> _pressedThisFrame = new();

    public void NewFrame()
    {
        _pressedThisFrame.Clear();
    }

    public void OnKeyDown(KeyCode key)
    {
        if (_down.Add(key))
        {
            _pressedThisFrame.Add(key);
        }
    }

    public void OnKeyUp(KeyCode key)
    {
        _down.Remove(key);
    }

    public bool IsDown(KeyCode key) => _down.Contains(key);

    public bool WasPressed(KeyCode key) => _pressedThisFrame.Contains(key);
}

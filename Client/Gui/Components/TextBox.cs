using System.Numerics;
using Client.Gui.Panels;
using Client.Managers;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components;

public class TextBox : IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode? Node { get; init; }
    public Action? Callback { get; init; }
    public string Placeholder { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Active { get; set; }
    public Vector2 Size { get; init; }
    public bool Hidden { get; init; }
    public int CharacterLimit { get; init; }
    public string TexturePath { get; set; } = string.Empty;
    private string _hiddenText = string.Empty;

    public void Clear()
    {
    }

    public void Draw(Vector2 parentPos, float frameTime)
    {
        Bounds = new Rectangle(0, 0, Size.X, Size.Y);
        ScreenOffset = new Vector2(parentPos.X + Position.X,
            parentPos.Y + Position.Y);
        DstRectangle = new Rectangle(
            ScreenOffset.X, 
            ScreenOffset.Y, 
            Size.X * AppConfig.ScaleFactor, 
            Size.Y * AppConfig.ScaleFactor); // TODO: create event system to do this there instead
        
        Raylib.DrawRectangleLinesEx(DstRectangle, Active ? 2.0f : 0.0f, Raylib.WHITE);

        if (!Hidden)
        {
            Raylib.DrawText(Text == string.Empty ? Placeholder : Text, (int)DstRectangle.x + 5,
                (int)DstRectangle.y + 10, 16, Raylib.BLACK);
        }
        else
        {
            Raylib.DrawText(_hiddenText == string.Empty ? Placeholder : _hiddenText, (int)DstRectangle.x + 5,
                (int)DstRectangle.y + 10, 16, Raylib.BLACK);
        }
    }

    public void Update(Vector2 parentPos, float frameTime)
    {
        var input = ServiceLocator.Get<InputManager>();
        if (!Active) return;
        var keyPressed = Raylib.GetKeyPressed();

        if (input.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
        {
            var length = Text.Length;
            if (length == 0) return;
            Text = Text.Remove(length - 1);
            if (Hidden)
                _hiddenText = _hiddenText.Remove(length - 1);
        }

        if (Text.Length >= CharacterLimit) return;
        char character;
        switch (keyPressed)
        {
            case >= 1 and <= 92:
                if (input.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || input.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT))
                {
                    character = Convert.ToChar(keyPressed);
                    Text += char.ToUpper(character);
                }
                else
                {
                    character = Convert.ToChar(keyPressed);
                    Text += char.ToLower(character);
                }

                if (Hidden)
                    _hiddenText += '*';
                break;
            default:
                return;
        }
    }
}
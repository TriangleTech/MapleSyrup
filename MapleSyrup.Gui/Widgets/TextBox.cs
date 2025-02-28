using System.Numerics;
using MapleSyrup.Gui.Enums;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public class TextBox : WidgetBase
{
    public string Text { get; set; } = string.Empty;
    public float Roundness { get; init; } = 0.0f;
    public int CharacterLimit { get; init; } = 32;
    public required Color BoxColor { get; init; }
    public required Color FontColor { get; init; }
    public required float FontSize { get; init; }
    
    public TextBox(string name) 
        : base(name)
    {
        State = WidgetState.Normal;
    }

    public override void Draw()
    {
        Raylib.DrawRectangleRounded(Bounds, Roundness, 4, BoxColor);
        
        switch (State)
        {
            case WidgetState.Normal:
                Raylib.DrawRectangleRoundedLines(Bounds, Roundness, 4, 1.0f, Raylib.BLACK);
                break;
            case WidgetState.Active:
                Raylib.DrawRectangleRoundedLines(Bounds, Roundness, 4, 1.0f, Raylib.WHITE);
                break;
        }

        Raylib.DrawText(Text, Bounds.X + 5f, Bounds.Y + 5f, FontSize, FontColor);
    }

    private void OnBoxClicked(Vector2 mousePosition)
    {
        if (State == WidgetState.Active)
        {
            if (!Raylib.CheckCollisionPointRec(mousePosition, Bounds))
            {
                if (Raylib.IsMouseButtonDown(Raylib.MOUSE_LEFT_BUTTON))
                {
                    if (State != WidgetState.Normal)
                    {
                        State = WidgetState.Normal;
                        Parent.RemoveActive(this);
                    }
                }
            }
        }
        else
        {
            if (Raylib.CheckCollisionPointRec(mousePosition, Bounds))
            {
                if (Raylib.IsMouseButtonDown(Raylib.MOUSE_LEFT_BUTTON))
                {
                    if (State != WidgetState.Active)
                    {
                        State = WidgetState.Active;
                        Parent.SetActive(this);
                    }
                }
            }
        }
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        var mousePosition = Raylib.GetMousePosition();
        OnBoxClicked(mousePosition);

        if (State != WidgetState.Active) 
            return;
        Thread.SpinWait(10);
        
        var keyPressed = Raylib.GetKeyPressed();
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
        {
            var length = Text.Length;
            if (length == 0) return;
            Text = Text.Remove(length - 1);
            return;
        }
        
        if (Text.Length >= CharacterLimit) 
            return;
        
        switch (keyPressed)
        {
            case >= 1 and <= 92:
                char character;
                if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_SHIFT) || Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT_SHIFT))
                {
                    character = Convert.ToChar(keyPressed);
                    Text += char.ToUpper(character);
                }
                else
                {
                    character = Convert.ToChar(keyPressed);
                    Text += char.ToLower(character);
                }
                break;
            default:
                return;
        }
    }
}
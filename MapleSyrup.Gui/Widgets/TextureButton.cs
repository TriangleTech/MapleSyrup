using MapleSyrup.Gui.Enums;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Widgets;

public class TextureButton : WidgetBase
{
    public string NormalPath { get; init; } = string.Empty;
    public string HoverPath { get; init; } = string.Empty;
    public string PressedPath { get; init; } = string.Empty;
    public string DisabledPath { get; init; } = string.Empty;
    private Action _onClick;

    public Dictionary<WidgetState, Texture> ButtonStates { get; }

    public TextureButton(string name) : base(name)
    {
        ButtonStates = new Dictionary<WidgetState, Texture>();
    }

    public void AddState(WidgetState state, Texture texture)
    {
        ButtonStates.Add(state, texture);
    }

    public void SetCallback(Action onClickCallback)
    {
        _onClick = onClickCallback;
    }

    public override void Draw()
    {
        if (!Visible)
            return;
        
        Raylib.DrawTexture(ButtonStates[State], (int)Bounds.X, (int)Bounds.Y, Raylib.WHITE);
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        
        var mousePosition = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mousePosition, Bounds))
        {
            if (HoverPath != string.Empty)
                State = WidgetState.Hover;
            if (Raylib.IsMouseButtonDown(Raylib.MOUSE_LEFT_BUTTON))
            {
                if (PressedPath != string.Empty)
                    State = WidgetState.Pressed;
                _onClick?.Invoke();
            }
        }
    }
}
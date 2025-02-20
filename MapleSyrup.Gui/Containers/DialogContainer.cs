using System.Numerics;
using MapleSyrup.Gui.Enums;
using ZeroElectric.Vinculum;

namespace MapleSyrup.Gui.Containers;

public class DialogContainer : Container
{
    public required Color HeaderColor = Raylib.BLACK;
    public required string Header = string.Empty;
    public required float FontSize = 12;
    public required Color FontColor = Raylib.WHITE;
    public Color OutlineColor = Raylib.BLACK;
    public Color CloseButtonColor = Raylib.RED;
    public Color MinButtonColor = Raylib.GREEN;
    public bool CanClose = true;
    public bool CanMinimize = true;
    private bool _showPanel = true;
    public float Roundness = 0f;
    private WidgetState _closeBtnState = WidgetState.Active;
    private WidgetState _minBtnState = WidgetState.Active;
    private Rectangle _headerRect;
    private Rectangle _closeBtnRect;
    private Rectangle _minBtnRect;
    private float offsetX = 0f;
    
    public DialogContainer(string name, int width, int height) 
        : base(name, width, height)
    {
        IsVisible = true;
    }

    public override void Draw()
    {
        if (!IsVisible) return;
        
        // Panel
        if (_showPanel)
            Raylib.DrawRectangleRounded(Bounds, Roundness, 1, new Color(BackgroundColor.r, BackgroundColor.g, BackgroundColor.b, Alpha));
        
        // Header
        Raylib.DrawRectangleRounded(_headerRect, Roundness - 0.1f, 1, new Color(HeaderColor.r, HeaderColor.g, HeaderColor.b, Alpha));
        Raylib.DrawRectangleRoundedLines(_headerRect, Roundness - 0.1f, 1, 1f, new Color(OutlineColor.r, OutlineColor.g, OutlineColor.b, Alpha));
        Raylib.DrawText(Header, Position.X + 10f, Position.Y + 10f, FontSize, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));

        if (CanClose)
        {
            Raylib.DrawRectangleRounded(_closeBtnRect, Roundness - 0.1f, 1,
                new Color(CloseButtonColor.r, CloseButtonColor.g, CloseButtonColor.b, Alpha));
            
            switch (_closeBtnState)
            {
                case WidgetState.Active:
                    Raylib.DrawText("X", Position.X + Width - 25f, Position.Y + 10f, FontSize, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
                case WidgetState.Hovered:
                    Raylib.DrawText("X", Position.X + Width - 25f, Position.Y + 10f, FontSize + 2, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
                case WidgetState.Pressed:
                    Raylib.DrawText("X", Position.X + Width - 25f, Position.Y + 10f, FontSize - 2, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
            }
        }

        if (CanMinimize)
        {
            offsetX = Position.X + Width - 35f;
            if (CanClose)
            {
                offsetX -= 40f;
                _minBtnRect = new Rectangle(offsetX, Position.Y, 35, 35);
            }
            
            Raylib.DrawRectangleRounded(_minBtnRect, Roundness - 0.1f, 1,
                new Color(MinButtonColor.r, MinButtonColor.g, MinButtonColor.b, Alpha));
            
            switch (_minBtnState)
            {
                case WidgetState.Active:
                    Raylib.DrawText("-", offsetX + 15f, Position.Y + 10f, FontSize, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
                case WidgetState.Hovered:
                    Raylib.DrawText("-", offsetX + 15f, Position.Y + 10f, FontSize + 2, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
                case WidgetState.Pressed:
                    Raylib.DrawText("-", offsetX + 15f, Position.Y + 10f, FontSize - 2, new Color(FontColor.r, FontColor.g, FontColor.b, Alpha));
                    break;
            }
        }
        
        if (!IsVisible || !_showPanel) return;
        foreach (var (_, widget) in Widgets)
            widget.Draw();
    }

    public override void Update(float timeDelta)
    {
        if (!IsVisible) return;
        var mousePos = Raylib.GetMousePosition();
        if (Raylib.CheckCollisionPointRec(mousePos, _headerRect))
        {
            if (Raylib.IsMouseButtonDown(Raylib.MOUSE_LEFT_BUTTON))
            {
                var mouseDelta = Raylib.GetMouseDelta();
                var x = Position.X + mouseDelta.X;
                var y = Position.Y + mouseDelta.Y;
                Position = new Vector2(x, y);
                Bounds = new Rectangle(Position.X, Position.Y, Width, Height);
            }
        }
        
        _headerRect = new Rectangle(Position.X, Position.Y, Width, 35);
        
        if (CanClose)
        {
            _closeBtnRect = new Rectangle(Position.X + Width - 35f, Position.Y, 35, 35);
            var mousePosition = Raylib.GetMousePosition();
            if (Raylib.CheckCollisionPointRec(mousePosition, _closeBtnRect))
            {
                _closeBtnState = WidgetState.Hovered;
                if (Raylib.IsMouseButtonPressed(Raylib.MOUSE_LEFT_BUTTON))
                {
                    _closeBtnState = WidgetState.Pressed;
                    IsVisible = !IsVisible;
                }
            }
            else
            {
                _closeBtnState = WidgetState.Active;
            }
        }
        
        if (CanMinimize)
        {
            _minBtnRect = new Rectangle(offsetX, Position.Y, 35, 35);
            var mousePosition = Raylib.GetMousePosition();
            if (Raylib.CheckCollisionPointRec(mousePosition, _minBtnRect))
            {
                _minBtnState = WidgetState.Hovered;
                if (Raylib.IsMouseButtonPressed(Raylib.MOUSE_LEFT_BUTTON))
                {
                    _minBtnState = WidgetState.Pressed;
                    _showPanel = !_showPanel;
                }
            }
            else
            {
                _minBtnState = WidgetState.Active;
            }
        }
        
        foreach (var (_, widget) in Widgets)
            widget.Update(timeDelta);
    }
}
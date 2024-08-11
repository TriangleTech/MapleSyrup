using System.Numerics;
using Client.Gui.Panels;
using Client.Managers;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components.Buttons;

public class TextureButton : IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode Node { get; init; }
    public Action? OnHover { get; init; }
    public Action? OnClick { get; init; }
    public bool Active { get; set; }
    public string TexturePath { get; set; }
    public ButtonState State { get; set; } = ButtonState.Normal;
    public Vector2 ScreenOffset { get; set; }

    private readonly Dictionary<ButtonState, Texture> _textures;

    public TextureButton(NxNode node)
    {
        Node = node;
        TexturePath = node.FullPath;
        _textures = new();

        if (Node.Has("normal"))
        {
            var normal = Node["normal"];
            if (normal.NodeType == NodeType.Bitmap)
                _textures[ButtonState.Normal] = Node.GetTexture("normal");
            else
                _textures[ButtonState.Normal] = normal.GetTexture("0");
            
        }

        if (Node.Has("mouseOver"))
        {
            var mouseOver = Node["mouseOver"];
            if (mouseOver.NodeType == NodeType.Bitmap)
                _textures[ButtonState.MouseOver] = Node.GetTexture("mouseOver");
            else
                _textures[ButtonState.MouseOver] = mouseOver.GetTexture("0");
        }

        if (Node.Has("pressed"))
        {
            var pressed = Node["pressed"];
            _textures[ButtonState.Pressed] = Node["pressed"].GetTexture("0");
        }

        if (Node.Has("disabled"))
        {
            var disabled = Node["disabled"];
            if (disabled.NodeType == NodeType.Bitmap)
                _textures[ButtonState.Disabled] = Node.GetTexture("disabled");
            else
                _textures[ButtonState.Disabled] = disabled.GetTexture("0");
        }
    }

    public void Draw(Vector2 parentPos, float frameTime)
    {
        Bounds = new Rectangle(0, 0, _textures[State].width, _textures[State].height);
        ScreenOffset = new Vector2(parentPos.X + Position.X, parentPos.Y + Position.Y);
        DstRectangle = new Rectangle(
            ScreenOffset.X,
            ScreenOffset.Y,
            _textures[State].width * AppConfig.ScaleFactor,
            _textures[State].height * AppConfig.ScaleFactor); // TODO: create event system to do this there instead
        Raylib.DrawTexturePro(_textures[State], Bounds, DstRectangle, Vector2.Zero, 0f, Raylib.WHITE);
    }

    public void Update(Vector2 parentPos, float frameTime)
    {
        if (State == ButtonState.Disabled) return;
        var world = ServiceLocator.Get<WorldManager>();
        if (Raylib.CheckCollisionPointRec(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), world.GetCamera()), DstRectangle))
        {
            if (_textures.ContainsKey(ButtonState.MouseOver))
                State = ButtonState.MouseOver;
            OnHover?.Invoke();

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                if (_textures.ContainsKey(ButtonState.Pressed))
                    State = ButtonState.Pressed;
                OnClick?.Invoke();
            }
        }
        else
        {
            State = ButtonState.Normal;
        }
    }

    public void Clear()
    {
         foreach (var (_, tex) in _textures) 
             Raylib.UnloadTexture(tex);
    }

    public void ProcessPacket(InPacket response)
    {
        
    }
}
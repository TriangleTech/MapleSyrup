using System.Numerics;
using Client.Gui.Panels;
using Client.Managers;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Gui.Components;

public class Checkbox : IUIComponent
{
    public IUIPanel Parent { get; init; }
    public int ID { get; init; }
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 ScreenOffset { get; set; }
    public Rectangle Bounds { get; set; }
    public Rectangle DstRectangle { get; set; }
    public NxNode Node { get; init; }
    public bool Active { get; set; }
    public string TexturePath { get; set; }
    
    private bool _isChecked;
    private List<Texture> _states;
    private CheckboxState _state;

    public Checkbox(NxNode node)
    {
        Node = node;
        TexturePath = node.FullPath;
        _isChecked = false;
        _states = new(2);
        _states.Add(node.GetTexture("0"));
        _states.Add(node.GetTexture("1"));
        _state = CheckboxState.Unchecked;
    }
    
    public void Draw(Vector2 parentPos, float frameTime)
    {
        ScreenOffset = new Vector2(parentPos.X + Position.X, parentPos.Y + Position.Y);
        Bounds = new Rectangle(0, 0, _states[(int)_state].width, _states[(int)_state].height);
        DstRectangle = new Rectangle(
            ScreenOffset.X,
            ScreenOffset.Y,
            _states[(int)_state].width * AppConfig.ScaleFactor,
            _states[(int)_state].height * AppConfig.ScaleFactor); // TODO: create event system to do this there instead        Raylib.DrawTextureEx(_isChecked ? _check : _noCheck, parentPos + Position, 0f, 1.0f, Raylib.WHITE);
    }

    public void Update(Vector2 parentPos, float frameTime)
    {
        var world = ServiceLocator.Get<WorldManager>();
        if (Raylib.CheckCollisionPointRec(Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), world.GetCamera()),
                Bounds))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                _isChecked = !_isChecked;
                _state  = (CheckboxState)Convert.ToInt32(_isChecked);
            }
        }
    }

    public void Clear()
    {
        Raylib.UnloadTexture(_states[0]);
        Raylib.UnloadTexture(_states[1]);
        _states.Clear();
    }
}

public enum CheckboxState
{
    Unchecked,
    Checked
}
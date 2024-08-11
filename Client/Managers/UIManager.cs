using System.Numerics;
using Client.Gui.Components;
using Client.Gui.Components.Buttons;
using Client.Gui.Enums;
using Client.Gui.Panels;
using Client.NX;
using Raylib_CsLo;

namespace Client.Managers;

/// <summary>
/// The UIManager class manages the user interface of the application.
/// </summary>
public class UIManager : IManager
{
    private List<IUIPanel> _interfaces;
    private uint _uiCount;
    private bool _cleared;
    private object _threadLock;
    private Queue<IUIPanel> _addQueue, _removeQueue, _changeQueue;

    /// <summary>
    /// Default Constructor
    /// </summary>
    public UIManager()
    {
        _interfaces = new();
        _uiCount = 0;
        _cleared = false;
        _threadLock = new();
        _addQueue = new();
        _removeQueue = new();
        _changeQueue = new();
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        ClearUI();
        _interfaces.Clear();
    }

    #region Clear/Remove/Add

    /// <summary>
    /// Clears all user interface panels managed by the UIManager.
    /// </summary>
    public void ClearUI()
    {
        if (_cleared) return;
        
        foreach (var ui in _interfaces)
            ui.Clear();
        _uiCount = 0;
        _cleared = true;
    }
    
    /// <summary>
    /// Adds an interface panel to the UI manager.
    /// </summary>
    /// <param name="panel">The interface panel to be added.</param>
    /// <param name="instant">Flag indicating whether the panel should be added instantly without any delay. The default value is false.</param>
    public void AddInterface(IUIPanel panel, bool instant = false)
    {
        lock (_threadLock)
        {
            if (GetUI(panel.Name) != null) return;
            if (!instant) _addQueue.Enqueue(panel);
            else _interfaces.Add(panel);
        }
    }

    /// <summary>
    /// Updates the specified interface panel in the UI manager.
    /// </summary>
    /// <param name="panel">The interface panel to be updated.</param>
    public void UpdateInterface(IUIPanel panel)
    {
        lock (_threadLock)
        {
            _changeQueue.Enqueue(panel);
        }
    }

    /// <summary>
    /// Removes an interface panel from the UI manager.
    /// </summary>
    /// <param name="panel">The interface panel to be removed.</param>
    public void RemoveInterface(IUIPanel panel)
    {
        lock (_threadLock)
        {
            _removeQueue.Enqueue(panel);
        }
    }
    
    #endregion
    
    #region UI

    #region Containers

    /// <summary>
    /// Creates a static panel for the UI with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the panel.</param>
    /// <param name="texture">The texture for the panel.</param>
    /// <param name="texturePath">The path to the texture.</param>
    /// <param name="position">The position of the panel.</param>
    /// <param name="visible">Indicates whether the panel is visible. Default is true.</param>
    /// <param name="priority">The priority of the panel. Default is GuiPriority.Normal.</param>
    /// <returns>The created StaticPanel object.</returns>
    public StaticPanel CreateStaticPanel(string name, Texture texture, string texturePath, Vector2 position, bool visible = true, GuiPriority priority = GuiPriority.Normal)
    {
        return new StaticPanel
        {
            ID = _uiCount++, 
            Name = name, 
            Position = position, 
            Texture = texture, 
            TexturePath = texturePath,
            Visible = visible,
            Priority = priority,
        };
    }

    /// <summary>
    /// Creates a frame panel with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the FramePanel.</param>
    /// <param name="texture">The texture of the FramePanel.</param>
    /// <param name="texturePath">The path to the texture of the FramePanel.</param>
    /// <param name="position">The position of the FramePanel.</param>
    /// <param name="visible">Specifies whether the FramePanel is initially visible. Default is true.</param>
    /// <param name="priority">The priority of the FramePanel. Default is GuiPriority.Above.</param>
    /// <returns>A new instance of the FramePanel class.</returns>
    public FramePanel CreateScreenFrame(string name, Texture texture, string texturePath, Vector2 position, bool visible = true, GuiPriority priority = GuiPriority.Above)
    {
        return new FramePanel(texturePath)
        {
            ID = _uiCount++, 
            Name = name, 
            Position = position, 
            Texture = texture, 
            Visible = visible,
            Priority = priority,
        };
    }

    /// <summary>
    /// Creates a new modal panel.
    /// </summary>
    /// <param name="name">The name of the modal panel.</param>
    /// <param name="texture">The texture of the modal panel.</param>
    /// <param name="texturePath">The path to the texture of the modal panel.</param>
    /// <param name="position">The position of the modal panel.</param>
    /// <param name="visible">Whether the modal panel is visible or not. Default is false.</param>
    /// <param name="priority">The priority of the modal panel. Default is GuiPriority.Above.</param>
    /// <returns>A new instance of the <see cref="Modal"/> class.</returns>
    public Modal CreateModal(string name, Texture texture, string texturePath, Vector2 position, bool visible = false, GuiPriority priority = GuiPriority.Above)
    {
        return new Modal(texturePath)
        {
            ID = _uiCount++, 
            Name = name, 
            Position = position, 
            Texture = texture, 
            Visible = visible,
            Priority = priority, };
    }

    /// <summary>
    /// Creates a ButtonPanel object with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the ButtonPanel.</param>
    /// <param name="texture">The texture for the ButtonPanel.</param>
    /// <param name="texturePath">The path to the texture for the ButtonPanel.</param>
    /// <param name="position">The position of the ButtonPanel.</param>
    /// <param name="layout">The layout of the ButtonPanel (enum of type GridLayout).</param>
    /// <param name="offsetX">The X offset between each element in the ButtonPanel. Default is 0.</param>
    /// <param name="offsetY">The Y offset between each element in the ButtonPanel. Default is 0.</param>
    /// <param name="priority">The priority of the ButtonPanel (enum of type GuiPriority). Default is GuiPriority.Normal.</param>
    /// <param name="numberOfRows">The number of rows in the ButtonPanel. Default is 1.</param>
    /// <param name="numberOfColumns">The number of columns in the ButtonPanel. Default is 1.</param>
    /// <param name="visible">The initial visibility state of the ButtonPanel. Default is false.</param>
    /// <returns>The created ButtonPanel object</returns>
    public ButtonPanel CreateButtonPanel(string name, Texture texture, string texturePath, Vector2 position, GridLayout layout, int offsetX = 0, int offsetY = 0, GuiPriority priority = GuiPriority.Normal, int numberOfRows = 1,
        int numberOfColumns = 1, bool visible = false)
    {
        return new ButtonPanel(texturePath)
        {
            ID = _uiCount++,
            Name = name,
            Priority = priority,
            Texture = texture,
            Position = position,
            Layout = layout,
            OffsetX = offsetX,
            OffsetY = offsetY,
            RowCount = numberOfRows,
            ColumnCount = numberOfColumns,
            Visible = visible
        };
    }
    
    /// <summary>
    /// Creates a StackPanel object with the specified parameters.
    /// </summary>
    /// <param name="name">The name of the ButtonPanel.</param>
    /// <param name="texture">The texture for the ButtonPanel.</param>
    /// <param name="texturePath">The path to the texture for the ButtonPanel.</param>
    /// <param name="position">The position of the ButtonPanel.</param>
    /// <param name="layout">The layout of the ButtonPanel (enum of type GridLayout).</param>
    /// <param name="offsetX">The X offset between each element in the ButtonPanel. Default is 0.</param>
    /// <param name="offsetY">The Y offset between each element in the ButtonPanel. Default is 0.</param>
    /// <param name="priority">The priority of the ButtonPanel (enum of type GuiPriority). Default is GuiPriority.Normal.</param>
    /// <param name="numberOfRows">The number of rows in the ButtonPanel. Default is 1.</param>
    /// <param name="numberOfColumns">The number of columns in the ButtonPanel. Default is 1.</param>
    /// <param name="visible">The initial visibility state of the ButtonPanel. Default is false.</param>
    /// <returns>The created ButtonPanel object</returns>
    public StackPanel CreateStackPanel(string name, Texture texture, string texturePath, Vector2 position, GridLayout layout, float offsetX = 0, float offsetY = 0, GuiPriority priority = GuiPriority.Normal, int numberOfRows = 1,
        int numberOfColumns = 1, bool visible = false)
    {
        return new StackPanel
        {
            ID = _uiCount++,
            Name = name,
            Priority = priority,
            Texture = texture,
            Position = position,
            Layout = layout,
            OffsetX = offsetX,
            OffsetY = offsetY,
            RowCount = numberOfRows,
            ColumnCount = numberOfColumns,
            Visible = visible,
            TexturePath = texturePath
        };
    }
    
    #endregion

    #region Components

    public Label CreateLabel(IUIPanel parent, Vector2 position, string text, int fontSize, Color fontColor)
    {
        return new Label
        {
            Parent = parent,
            Position = position,
            Text = text,
            FontSize = fontSize,
            Color = fontColor
        };
    }

    /// <summary>
    /// Creates a new TextureButton and adds it to the specified parent IUIPanel.
    /// </summary>
    /// <param name="parent">The parent IUIPanel to add the button to.</param>
    /// <param name="button">The NxNode representing the button.</param>
    /// <param name="position">The position of the button.</param>
    /// <param name="hover">An optional callback to be executed when you hover over the button.</param>
    /// <param name="click">An optional callback to be executed when you click on the button.</param>
    /// <returns>A new TextureButton instance.</returns>
    public TextureButton CreateButton(IUIPanel parent, NxNode button, Vector2 position, Action? hover = null, Action? click = null)
    {
        return new TextureButton(button)
        {
            Parent = parent, 
            ID = parent.Nodes.Count, 
            Name = button.Name, 
            Position = position, 
            OnHover = hover,
            OnClick = click
        };
    }

    /// <summary>
    /// Creates a new Checkbox component within a specified parent panel.
    /// </summary>
    /// <param name="parent">The parent panel that will contain the checkbox.</param>
    /// <param name="checkBox">The NxNode representing the checkbox.</param>
    /// <param name="position">The position of the checkbox within the parent panel.</param>
    /// <param name="callback">An optional callback function that will be invoked when the checkbox is clicked.</param>
    /// <returns>The newly created Checkbox component.</returns>
    public Checkbox CreateCheckbox(IUIPanel parent, NxNode checkBox, Vector2 position, Action? callback)
    {
        return new Checkbox(checkBox)
        {
            Parent = parent, 
            ID = parent.Nodes.Count, 
            Name = checkBox.Name, 
            Position = position
        };
    }

    /// <summary>
    /// Creates a decal UI component and adds it to the specified parent panel.
    /// </summary>
    /// <param name="parent">The parent panel to add the decal to.</param>
    /// <param name="texture">The texture for the decal.</param>
    /// <param name="texturePath">The path to the texture.</param>
    /// <param name="name">The name of the decal.</param>
    /// <param name="position">The position of the decal.</param>
    /// <returns>The created decal.</returns>
    public Decal CreateDecal(IUIPanel parent, Texture texture, string texturePath, string name, Vector2 position)
    {
        return new Decal
        {
            Parent = parent, 
            ID = parent.Nodes.Count, 
            Texture = texture,
            TexturePath = texturePath,
            Name = name,
            Position = position
        };
    }

    /// <summary>
    /// Creates a TextBox UI component.
    /// </summary>
    /// <param name="parent">The parent IUIPanel that the TextBox belongs to.</param>
    /// <param name="name">The name of the TextBox.</param>
    /// <param name="size">The size of the TextBox.</param>
    /// <param name="placeholder">The placeholder text to display in the TextBox.</param>
    /// <param name="characterLimit">The character limit for the TextBox.</param>
    /// <param name="isHidden">Whether the TextBox is hidden or visible.</param>
    /// <param name="position">The position of the TextBox.</param>
    /// <returns>The created TextBox.</returns>
    public TextBox CreateTextbox(IUIPanel parent, string name, Vector2 size, string placeholder, int characterLimit, bool isHidden, Vector2 position)
    {
        return new TextBox()
        {
            Parent = parent,
            ID = parent.Nodes.Count,
            Name = name,
            Size = size,
            Placeholder = placeholder,
            CharacterLimit = characterLimit,
            Hidden = isHidden,
            Position = position,
            Text = string.Empty
        };
    }
    
    #endregion

    #endregion

    /// <summary>
    /// Retrieves the UI panel with the specified name.
    /// </summary>
    /// <param name="name">The name of the UI panel to retrieve.</param>
    /// <returns>The UI panel with the specified name, or null if not found.</returns>
    public IUIPanel? GetUI(string name)
    {
        return _interfaces.Find(x => x.Name == name) ?? null;
    }

    /// <summary>
    /// Checks whether the given panel has focus based on the mouse input.
    /// If the mouse is not within the bounds of the panel and the left mouse button is pressed,
    /// the panel is set to inactive.
    /// If the mouse is within the bounds of the panel and the left mouse button is pressed,
    /// the panel is set to active.
    /// </summary>
    /// <param name="input">The input manager for retrieving mouse position and button state.</param>
    /// <param name="panel">The panel to check focus for.</param>
    public void CheckPanelFocus(InputManager input, IUIPanel panel, Rectangle bounds)
    {
        if (!Raylib.CheckCollisionPointRec(input.MouseToWorld, bounds))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                panel.Active = false;
            }
        }

        if (Raylib.CheckCollisionPointRec(input.MouseToWorld, bounds))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                panel.Active = true;
            }
        }
    }

    /// <summary>
    /// Checks the focus of a UI node based on the mouse position.
    /// If the mouse is outside the bounds of the node and the left mouse button is pressed,
    /// the node's Active property is set to false.
    /// If the mouse is inside the bounds of the node and the left mouse button is pressed,
    /// the node's Active property is set to true.
    /// </summary>
    /// <param name="input">The InputManager instance used to get the mouse position.</param>
    /// <param name="node">The UI component to check the focus of.</param>
    public void CheckNodeFocus(InputManager input, IUIComponent node, Rectangle bounds)
    {
        if (!Raylib.CheckCollisionPointRec(input.MouseToWorld, bounds))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                node.Active = false;
            }
        }

        if (Raylib.CheckCollisionPointRec(input.MouseToWorld, bounds))
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
            {
                node.Active = true;
            }
        }
    }
    
    #region Draw/Update/Pending

    public void Draw(float frameTime)
    {
        foreach (var ui in _interfaces)
        {
            if (!ui.Visible) continue;
            ui.Draw(frameTime);
        }
    }

    public void Update(float frameTime)
    {
        foreach (var ui in _interfaces)
        {
            if (!ui.Visible) continue;
            ui.Update(frameTime);
        }
        ProcessPending();
    }

    private void ProcessPending()
    {
        lock (_threadLock)
        {
            while (_addQueue.Count > 0)
            {
                var ui = _addQueue.Dequeue();
                _interfaces.Add(ui);
                _interfaces.Sort((a, b) => Convert.ToInt32(a.Priority > b.Priority));
            }
            while (_removeQueue.Count > 0)
            {
                var ui = _removeQueue.Dequeue();
                _interfaces.Remove(ui);
                ui.Clear();
            }

            while (_changeQueue.Count > 0)
            {
                var ui = _changeQueue.Dequeue();
                _interfaces.Remove(ui);
                _interfaces.Add(ui);
                _interfaces.Sort((a, b) => Convert.ToInt32(a.Priority > b.Priority));
            }
        }
    }
    
    #endregion
}
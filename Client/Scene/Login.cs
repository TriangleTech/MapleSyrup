using System.Collections.Concurrent;
using System.Numerics;
using Client.Actors;
using Client.Avatar;
using Client.Gui.Components;
using Client.Gui.Components.Buttons;
using Client.Gui.Enums;
using Client.Gui.Panels;
using Client.Managers;
using Client.Net;
using Client.NX;
using Raylib_CsLo;

namespace Client.Scene;

/// <summary>
/// The Login class represents the login screen of the application.
/// </summary>
public class Login : IWorld
{
    /// <summary>
    /// Represents the collection of actors in the game world.
    /// </summary>
    public List<IActor> Actors { get; init; }

    /// <summary>
    /// Queue of actors to be added to the actor manager.
    /// </summary>
    public ConcurrentQueue<IActor> ToAdd { get; init; } = new();

    /// <summary>
    /// Represents the collection of actors that need to be removed from the game world.
    /// </summary>
    public ConcurrentQueue<IActor> ToRemove { get; init; } = new();

    /// <summary>
    /// Queue of actors and corresponding layers that need to be shifted.
    /// </summary>
    /// <remarks>
    /// When an actor needs to be moved to a different layer, it is added to this queue along with the target layer.
    /// The actor's current layer is updated to the target layer, and the list is resorted.
    /// </remarks>
    public ConcurrentQueue<(IActor, ActorLayer)> ToShift { get; init; } = new();

    /// <summary>
    /// Gets or sets the number of actors in the login scene.
    /// </summary>
    /// <value>
    /// The number of actors in the login scene.
    /// </value>
    public int ActorCount { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the world has been cleared.
    /// </summary>
    /// <value><c>true</c> if the world has been cleared; otherwise, <c>false</c>.</value>
    /// <remarks>
    /// When the world is cleared, all actors are cleared and the actor count is reset to zero.
    /// </remarks>
    public bool Cleared { get; set; }

    /// <summary>
    /// The camera used to view the world.
    /// </summary>
    private Camera2D _camera;

    /// <summary>
    /// Represents the camera in the game world.
    /// </summary>
    public Camera2D Camera => _camera;

    /// <summary>
    /// Object used for thread synchronization to ensure thread-safe access to shared resources.
    /// </summary>
    private object _threadLock;

    /// <summary>
    /// The current step in the login process.
    /// </summary>
    private LoginStep _step;

    private int _selectedPlayer = -1;
    private Dictionary<LoginStep, Vector2> _stepLocation;
    private byte _selectedWorld = 0, _selectedChannel = 0;

    /// <summary>
    /// Represents the login scene of the game.
    /// </summary>
    public Login()
    {
        Actors = new();
        _camera = new Camera2D
        {
            target = Vector2.Zero,
            zoom = AppConfig.BaseAspectRatio / AppConfig.CurrentAspectRatio
        };
        if (AppConfig.OriginalWidth != AppConfig.ScreenWidth || AppConfig.ScreenHeight != AppConfig.OriginalHeight)
        {
            var widthDiff = (float)AppConfig.ScreenWidth / AppConfig.OriginalWidth;
            var heightDiff = (float)AppConfig.ScreenHeight / AppConfig.OriginalHeight;
            _camera.zoom *= Math.Min(widthDiff, heightDiff);
        }
        _threadLock = new();
        _step = LoginStep.Start;
        _stepLocation = new();
    }

    /// <summary>
    /// Loads the content for the Login world.
    /// </summary>
    public void Load()
    {
        LoadUI();
        ((IWorld)this).SortLayers();
        ((IWorld)this).ProcessPending();
    }

    #region Login Specific Functions

    /// <summary>
    /// Loads the UI components for the Login scene.
    /// </summary>
    private void LoadUI()
    {
        var ui = ServiceLocator.Get<UIManager>();
        var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
        var common = uiNode["Common"];
        var title = uiNode["Title"];

        CreateFrame(ui, common);
        if (AppConfig.VersionMajor >= 92) // I think that's when it changed.
        {
            CreateNewSignboard(ui, title);
        } 
        if (AppConfig.VersionMajor < 83)
        {
            _stepLocation[LoginStep.Start] = new Vector2(-370, -310);
            _stepLocation[LoginStep.PickWorld] = new Vector2(-370, -910);
            _stepLocation[LoginStep.PickCharacter] = new Vector2(-370, -1530);
            _stepLocation[LoginStep.CreateCharacter] = new Vector2(-370, -2130);
            CreateLegacySignboard(ui, title);
            CreateLegacyCharBtns(ui, uiNode["CharSelect"]);
        }
    }

    /// <summary>
    /// Creates a frame UI component and adds it to the UIManager.
    /// </summary>
    /// <param name="ui">The UIManager instance.</param>
    /// <param name="common">The NxNode containing the frame UI configuration.</param>
    private void CreateFrame(UIManager ui, NxNode common)
    {
        var origin = common["frame"].GetVector("origin");
        var frame = ui.CreateScreenFrame("frame", common.GetTexture("frame"), common["frame"].FullPath,
            new Vector2(30, -10) - origin);
        frame.Add(ui.CreateLabel(frame, 
            new Vector2(500, 10), 
            $"[Version: {AppConfig.VersionMajor} | Build Type: Dev]", 
            16,
            Raylib.BLACK));
        ui.AddInterface(frame, true);
    }

    #region Post BB UI
    
    /// <summary>
    /// Creates a new (Post-BB) signboard UI panel.
    /// </summary>
    /// <param name="ui">The UIManager instance.</param>
    /// <param name="title">The NxNode of the title.</param>
    private void CreateNewSignboard(UIManager ui, NxNode title)
    {
        IUIPanel signboard;
        // signboard
        var texture = title.GetTexture("signboard");
        var origin = title["signboard"].GetVector("origin");
        signboard = ui.CreateStaticPanel("signboard", texture, title["signboard"].FullPath, -origin);

        // Load Login Buttons
        var btnLogin = title["BtLogin"];
        var btnQuit = title["BtQuit"];

        signboard.Add(ui.CreateButton(signboard, btnLogin, new Vector2(180, 15), null, () =>
        {
            var signboard = ui.GetUI("signboard");
            var loginBtn = signboard?.GetNode("BtLogin") as TextureButton;
            var userbox = signboard?.GetNode("userbox") as TextBox;
            var passbox = signboard?.GetNode("passbox") as TextBox;
        }));
        signboard.Add(ui.CreateButton(signboard, btnQuit, new Vector2(160, 90), null, () => AppConfig.CloseWindow = true));
        signboard.Add(ui.CreateTextbox(signboard, "userbox", new Vector2(160, 25), "Username", 12, false,
            new Vector2(15, 15)));
        signboard.Add(ui.CreateTextbox(signboard, "passbox", new Vector2(160, 25), "Password", 12, true,
            new Vector2(15, 40)));
        ui.AddInterface(signboard);
    }

    /// <summary>
    /// Creates a new (Post-BB) notice with the specified parameters and adds it to the UI.
    /// </summary>
    /// <param name="name">The name of the notice.</param>
    /// <param name="backgroundType">The type of background for the notice.</param>
    /// <param name="noticeType">The type of notice.</param>
    /// <param name="position">The position of the notice.</param>
    private void CreateNewNotice(string name, int backgroundType, int noticeType, Vector2 position)
    {
        var ui = ServiceLocator.Get<UIManager>();
        var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
        var notice = uiNode["Notice"];
        var text = notice["text"].GetTexture($"{noticeType}");
        var modal = ui.CreateModal(name, notice["backgrnd"].GetTexture($"{backgroundType}"), notice["backgrnd"][$"{backgroundType}"].FullPath,
            position, true);

        var okBtn = ui.CreateButton(modal, notice["BtYes"], new Vector2(85, 95), null, () =>
        {
            var signboard = ui.GetUI("signboard");
            var noticeBoard = ui.GetUI(name);
            var loginBtn = signboard.GetNode("BtLogin") as TextureButton;
            loginBtn.State = ButtonState.Normal;
            noticeBoard.Visible = false;
            ui.RemoveInterface(noticeBoard);
        });

        modal.Add(ui.CreateDecal(modal, text, notice["text"][$"{noticeType}"].FullPath, "noticeReason", new Vector2(17, 13)));
        modal.Add(okBtn);
        ui.AddInterface(modal);
    }

    /// <summary>
    /// Spawns new (Post-BB) worlds in the game.
    /// </summary>
    /// <param name="worldId">The ID of the world.</param>
    /// <param name="serverName">The name of the server.</param>
    /// <param name="flag">The flag.</param>
    /// <param name="eventMessage">The event message.</param>
    /// <param name="channelCount">The number of channels.</param>
    private void SpawnNewWorlds(byte worldId, string serverName, byte flag, string eventMessage, int channelCount)
    {
        lock (_threadLock)
        {
            var ui = ServiceLocator.Get<UIManager>();
            var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
            var worldSelect = uiNode["WorldSelect"];

            var worldPanelExists = ui.GetUI("worldPanel") != null;
            var worldPanel = worldPanelExists
                ? ui.GetUI("worldPanel") as ButtonPanel
                : ui.CreateButtonPanel("worldPanel", new Texture() { width = 300, height = 200}, "", new Vector2(-250, -800), GridLayout.RightStack, 15,
                    25,
                    GuiPriority.Normal, 2, 6, true);
            worldPanel?.Add(ui.CreateButton(worldPanel, worldSelect["BtWorld"][$"{worldId}"], Vector2.Zero,null, () =>
            {
                var cb = ui.GetUI($"channelBoard_{worldId}");
                var bb = ui.GetUI($"buttonBoard_{worldId}");
                cb.Visible = !cb.Visible;
                bb.Visible = !bb.Visible;
            }));
            
            if (!worldPanelExists)
            {
                ui.AddInterface(worldPanel, true); // This has to be instant...
            }

            // This is simply the background to the channels.
            var channelBoard = ui.CreateStaticPanel($"channelBoard_{worldId}", worldSelect.GetTexture("chBackgrn"), worldSelect["chBackgrn"].FullPath,
                new Vector2(-150, -675), false);
            channelBoard.Add(ui.CreateButton(channelBoard, worldSelect["BtGoworld"], new Vector2(225, 50), null, null));

            // so they way button panel is designed, it has to be separate from the other board, I may change it later, but it does work.
            var buttonBoard = ui.CreateButtonPanel($"buttonBoard_{worldId}", new Texture() {width = 300, height = 200}, "", new Vector2(-127, -580),
                GridLayout.RightStack, 6, 7, GuiPriority.Normal, 4, 5);

            for (var i = 0; i < channelCount; i++)
            {
                buttonBoard.Add(ui.CreateButton(buttonBoard, worldSelect["channel"][$"{i}"], Vector2.Zero, null, () =>
                {
                    var frame = ui.GetUI("frame") as FramePanel;
                    frame.Position = new Vector2(-370, -1510);
                    _camera.target = frame.Position;
                    _step = LoginStep.PickCharacter;
                }));
            }

            ui.AddInterface(channelBoard);
            ui.AddInterface(buttonBoard);
        }
    }
    
    #endregion
    
    #region Legacy UI

    /// <summary>
    /// Create a legacy (v55 to v82) signboard UI in the login scene.
    /// </summary>
    /// <param name="ui">The UIManager instance used to create UI elements.</param>
    /// <param name="title">The NX node representing the title of the signboard.</param>
    private void CreateLegacySignboard(UIManager ui, NxNode title)
    {
        var signboard = ui.CreateStaticPanel("signboard", new Texture() { width = 300, height = 200 }, "", new Vector2(0, -100));
        signboard.Add(ui.CreateButton(signboard, title["BtLogin"], new Vector2(222, 15), null, () =>
        {
            SendPacket(RequestOps.LoginPassword);
        }));
        signboard.Add(ui.CreateButton(signboard, title["BtQuit"], new Vector2(221, 145), null, () => AppConfig.CloseWindow = true));
        signboard.Add(ui.CreateTextbox(signboard, "userbox", new Vector2(155, 25), "Username", 12, false,
            new Vector2(65, 25)));
        signboard.Add(ui.CreateTextbox(signboard, "passbox", new Vector2(155, 25), "Password", 12, true,
            new Vector2(65, 55)));
        ui.AddInterface(signboard);
    }

    /// <summary>
    /// Spawn legacy (v55 to v82) worlds in the game.
    /// </summary>
    /// <param name="worldId">The ID of the world.</param>
    /// <param name="serverName">The name of the server.</param>
    /// <param name="flag">The flag.</param>
    /// <param name="eventMessage">The event message.</param>
    /// <param name="channelCount">The number of channels.</param>
    private void CreateWorldBoard(byte worldId, string serverName, byte flag, string eventMessage)
    {
        // TODO: Create animation of scroll upon clicking world, don't care for this right now.
        lock (_threadLock)
        {
            var ui = ServiceLocator.Get<UIManager>();
            var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
            var worldSelect = uiNode["WorldSelect"];

            var worldPanelExists = ui.GetUI("worldPanel") != null;
            var worldPanel = worldPanelExists
                ? ui.GetUI("worldPanel") as ButtonPanel
                : ui.CreateButtonPanel("worldPanel", 
                    new Texture() { width = 300, height = 200}, 
                    "", 
                    new Vector2(-212, -800), 
                    GridLayout.RightStack, 
                    2,
                    25,
                    GuiPriority.Normal, 
                    2, 6, 
                    true);
            worldPanel?.Add(ui.CreateButton(worldPanel, worldSelect["BtWorld"][$"{worldId}"], Vector2.Zero, null, () =>
            {
                // TODO: Find out when WorldStatus gets sent
                var cb = ui.GetUI($"channelBoard_{worldId}");
                var bb = ui.GetUI($"buttonBoard_{worldId}");
                cb.Visible = !cb.Visible;
                bb.Visible = !bb.Visible;
            }));
            
            if (!worldPanelExists)
            {
                ui.AddInterface(worldPanel, true); // This has to be instant...
            }

            // This is simply the background to the channels.
            var channelBoard = ui.CreateStaticPanel($"channelBoard_{worldId}", worldSelect.GetTexture("chBackgrn"), worldSelect["chBackgrn"].FullPath,
                new Vector2(-150, -675), false);
            channelBoard.Add(ui.CreateButton(channelBoard, worldSelect["BtGoworld"], new Vector2(225, 50), null, null));

            // so they way button panel is designed, it has to be separate from the other board, I may change it later, but it does work.
            var buttonBoard = ui.CreateButtonPanel($"buttonBoard_{worldId}", new Texture() {width = 300, height = 200}, "", new Vector2(-127, -580),
                GridLayout.RightStack, 6, 7, GuiPriority.Normal, 4, 5);

            ui.AddInterface(channelBoard, true);
            ui.AddInterface(buttonBoard, true);
        }
    }
    
    /// <summary>
    /// Creates a legacy (v55 to v82) notice with the given parameters.
    /// </summary>
    /// <param name="name">The name of the notice.</param>
    /// <param name="backgroundType">The background type of the notice.</param>
    /// <param name="noticeType">The type of the notice.</param>
    /// <param name="position">The position of the notice.</param>
    private void CreateLegacyNotice(string name, int backgroundType, int noticeType, Vector2 position)
    {
        var ui = ServiceLocator.Get<UIManager>();
        var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
        var notice = uiNode["Notice"];
        var text = notice["text"].GetTexture($"{noticeType}");
        var modal = ui.CreateModal(name, notice["backgrnd"].GetTexture($"{backgroundType}"), notice["backgrnd"][$"{backgroundType}"].FullPath,
            position, true);

        var okBtn = ui.CreateButton(modal, notice["BtYes"], new Vector2(85, 95), null, () =>
        {
            var signboard = ui.GetUI("signboard");
            var noticeBoard = ui.GetUI(name);
            var loginBtn = signboard.GetNode("BtLogin") as TextureButton;
            loginBtn.State = ButtonState.Normal;
            noticeBoard.Visible = false;
            ui.RemoveInterface(noticeBoard);
        });

        modal.Add(ui.CreateDecal(modal, text, notice["text"][$"{noticeType}"].FullPath, "noticeReason", new Vector2(17, 13)));
        modal.Add(okBtn);
        ui.AddInterface(modal);
    }

    private void CreateLegacyCharBtns(UIManager ui, NxNode charSelect)
    {
        var panel = ui.CreateStackPanel("charSelect", new Texture() { width = 75, height = 250 }, "",
            new Vector2(206, -1361), GridLayout.VerticalDown, 0, 1f);
        var selectBtn = ui.CreateButton(panel, charSelect["BtSelect"], Vector2.Zero, null, null);
        var createBtn = ui.CreateButton(panel, charSelect["BtNew"], new Vector2(0, 2), null, () =>
        {
            lock (_threadLock)
            {
                var look = new AvatarLook()
                {
                    Equipment =
                    {
                        { PartType.Coat, 1040036 },
                        { PartType.Pants, 1060026 },
                        { PartType.Cape, 1102000 },
                        { PartType.Weapon, 1302000 }
                    }
                };

                var stats = new AvatarStats
                {
                    Id = 0,
                    Name = null,
                    Gender = 0,
                    SkinId = 0,
                    Face = 0,
                    Hair = 30000,
                    PetId = null,
                    Level = 1,
                    JobId = 0,
                    Strength = 4,
                    Dexterity = 4,
                    Intelligence = 4,
                    Luck = 4,
                    Hp = 100,
                    MaxHp = 100,
                    Mp = 50,
                    MaxMp = 50,
                    RemainingAp = 0,
                    Exp = 0,
                    Fame = 0,
                    GachaExp = 0,
                    MapId = 10000,
                    InitialSpawnPoint = 0
                };
                var createAdventurer = new Player(stats, look)
                {
                    ID = 9999999,
                    Name = "ACP",
                    ControlsLocked = true,
                    IsLeft = false,
                    Moving = false,
                    OnGround = false,
                    Layer = ActorLayer.TileLayer1,
                    ActorType = ActorType.Player,
                    Node = null,
                    Stats = stats,
                    Look = look,
                    Z = 0,
                    Visible = true,
                    Position = new Vector2(0, -1700),
                    Origin = default,
                    ScreenOffset = default,
                };
                _step = LoginStep.CreateCharacter;
                var frame = ui.GetUI("frame") as FramePanel;
                frame!.Position = _stepLocation[_step];
                UpdateCamera(frame.Position);
                Thread.Sleep(1);
                ((IWorld)this).AddActor(createAdventurer);
            }
        }); // it needed to be shifted individually.
        var deleteBtn = ui.CreateButton(panel, charSelect["BtDelete"]!, Vector2.Zero, null, null);
        panel.Add(selectBtn);
        panel.Add(createBtn);
        panel.Add(deleteBtn);
        panel.Visible = true;
        ui.AddInterface(panel);
    }
    
    #endregion

    #region Univeral Items (Not version dependent)
    
    private void AddChannelButton(byte worldId, string worldName, int population, byte channelId)
    {
        var ui = ServiceLocator.Get<UIManager>();
        var uiNode = ServiceLocator.Get<NxManager>().GetNode("Login.img");
        var worldSelect = uiNode["WorldSelect"];
        var buttonBoard = ui.GetUI($"buttonBoard_{worldId}") as ButtonPanel;
        if (buttonBoard == null) return;
        buttonBoard.Add(ui.CreateButton(buttonBoard, worldSelect["channel"][$"{channelId}"], Vector2.Zero, null, () =>
        {
            _selectedWorld = worldId;
            _selectedChannel = channelId;
            SendPacket(RequestOps.CharlistRequest);
        }));
    }
    
    #endregion
    
    #endregion

    #region Clear/Update/Draw

    /// <summary>
    /// Clears all assets used within the world.
    /// </summary>
    public void Clear()
    {
        if (Cleared) return;
        foreach (var actor in Actors)
            actor.Clear();

        ActorCount = 0;
        Cleared = true;
    }

    /// <summary>
    /// Draw method that renders the actors in the Login scene.
    /// </summary>
    /// <param name="frameTime">The elapsed time in milliseconds since the last frame.</param>
    public void Draw(float frameTime)
    {
        foreach(var actor in Actors)
            actor.Draw(frameTime);
    }

    /// <summary>
    /// Updates the Login scene by updating the actors in different layers, processing pending actions, and handling camera zoom based on mouse wheel input.
    /// </summary>
    /// <param name="frameTime">The time in milliseconds since
    public void Update(float frameTime)
    {
        var ui = ServiceLocator.Get<UIManager>();
        var input = ServiceLocator.Get<InputManager>();
        
        foreach(var actor in Actors)
        {
            actor.Update(frameTime);
            /*if (actor is not Player) continue;
            var player = actor as Player;
            player.Bounds = new Rectangle(
                (player.Position.X - 15f)  * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
                (player.Position.Y - 50f) * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY,
                25 * AppConfig.ScaleFactor,
                50 * AppConfig.ScaleFactor);
            if (Raylib.CheckCollisionRecs(input.MouseRec, player.Bounds))
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    if (player.ID == _selectedPlayer) continue;
                    var previousPlayer = GetActorById(_selectedPlayer) as Player;
                    previousPlayer?.ChangeState(AvatarState.Stand1);
                    _selectedPlayer = player.ID;
                    player.ChangeState(AvatarState.Walk1);
                }
            }*/
        }

        ((IWorld)this).ProcessPending();
        var frame = ui.GetUI("frame") as FramePanel;
        _camera.target = new Vector2(frame.Position.X * AppConfig.ScaleFactor + AppConfig.ScreenOffsetX,
            frame.Position.Y * AppConfig.ScaleFactor + AppConfig.ScreenOffsetY);
    }

    #endregion

    #region Packet Processing
    
    /// <summary>
    /// Process the incoming packet and perform necessary operations based on the opcode.
    /// </summary>
    /// <param name="packet">The incoming packet to be processed.</param
    public void ProcessPacket(InPacket packet)
    {
        lock (_threadLock)
        {
            var ui = ServiceLocator.Get<UIManager>();
            switch (packet.Opcode)
            {
                case ResponseOps.LoginStatus:
                {
                    var status = packet.ReadByte();
                    Console.WriteLine($"[Login Server] User/Pass Status: {status}");
                    switch (status)
                    {
                        case 0: // success
                            var accountId = packet.ReadInt();
                            var gender = packet.ReadByte();
                            var isAdmin = packet.ReadBoolean();
                            var accountUsername = packet.ReadMapleString();
                            Console.WriteLine($"[Acc ID: {accountId} Gender: {gender} Admin: {isAdmin} Acc Name: {accountUsername}]");
                            SendPacket(RequestOps.ServerlistRequest);
                            
                            break;
                        case 1: // Wrong user
                            if (AppConfig.VersionMajor > 92)
                                CreateNewNotice("wrong_user", (int)NoticeBackground.Error,
                                    (int)NoticeType.IncorrectUser, new Vector2(-115, -66)); 
                            if (AppConfig.VersionMajor < 83)
                                CreateLegacyNotice("wrong_user", (int)NoticeBackground.Error,
                                    (int)NoticeType.IncorrectUser, new Vector2(-25, -100));
                            break;
                        case 2: // Wrong pass
                            if (AppConfig.VersionMajor > 92)
                                CreateNewNotice("wrong_pass", (int)NoticeBackground.Error,
                                    (int)NoticeType.IncorrectPass, new Vector2(-115, -66)); 
                            if (AppConfig.VersionMajor < 83)
                                CreateLegacyNotice("wrong_pass", (int)NoticeBackground.Error,
                                    (int)NoticeType.IncorrectPass, new Vector2(-25, -100));
                            break;
                        case 3: // banned
                            break;
                        case 5: // not reg.
                            if (AppConfig.VersionMajor > 92)
                                CreateNewNotice("not_reg", (int)NoticeBackground.Error, (int)NoticeType.NotRegistered,
                                    new Vector2(-115, -66));
                            else if (AppConfig.VersionMajor < 83)
                                CreateLegacyNotice("not_reg", (int)NoticeBackground.Error,
                                    (int)NoticeType.NotRegistered, new Vector2(-25, -100));

                            break;
                        case 7: // already logged in.
                        case 23: // tos
                            var signboard = ui.GetUI("signboard");
                            var loginBtn = signboard.GetNode("BtLogin") as TextureButton;
                            loginBtn.State = ButtonState.Normal;
                            break;
                    }
                }
                    break;
                case ResponseOps.Serverlist:
                {
                    var numberOfWorlds = packet.ReadByte();
                    if (numberOfWorlds != 0xFF)
                    {
                        for (var w = 0; w < numberOfWorlds; w++)
                        {
                            var worldId = packet.ReadByte();
                            var worldName = packet.ReadMapleString();
                            var worldFlag = packet.ReadByte();
                            var eventMessage = packet.ReadMapleString();
                            var channelCount = packet.ReadByte();

                            CreateWorldBoard(worldId, worldName, 0, "");

                            for (var i = 0; i < channelCount; i++)
                            {
                                var worldMessage = packet.ReadMapleString();
                                var channelPopulation = packet.ReadInt();
                                var channelWorld = packet.ReadByte(); // why??
                                var channelId = packet.ReadByte();
                                var isAdultChannel = packet.ReadBoolean();
                                AddChannelButton(worldId, worldMessage, channelPopulation, channelId);
                            }
                        }
                    }
                    else
                    {

                        if (AppConfig.VersionMajor > 92)
                        {
                            var frame = ui.GetUI("frame") as FramePanel;
                            frame.Position = new Vector2(-370, -910);
                            _camera.target = frame.Position;
                            _step = LoginStep.PickWorld;
                        }
                        else
                        {
                            _step = LoginStep.PickWorld;
                            var frame = ui.GetUI("frame") as FramePanel;
                            frame.Position = _stepLocation[_step];
                            UpdateCamera(frame.Position);
                        }
                    }
                }
                    break;
                case ResponseOps.Charlist:
                {
                    var numberOfCharacters = packet.ReadByte();
                    for (var i = 0; i < numberOfCharacters; i++)
                    {
                        var playerId = packet.ReadInt();
                        var playerName = packet.ReadMapleString();
                        var playerGender = packet.ReadByte();
                        var playerSkin = packet.ReadByte();
                        var playerFace = packet.ReadInt();
                        var playerHair = packet.ReadInt();
                        var playerLevel = packet.ReadShort();
                        var playerJob = packet.ReadShort();
                        var playerStr = packet.ReadShort();
                        var playerDex = packet.ReadShort();
                        var playerInt = packet.ReadShort();
                        var playerLuk = packet.ReadShort();
                        var playerHp = packet.ReadShort();
                        var playerMaxHp = packet.ReadShort();
                        var playerMp = packet.ReadShort();
                        var playerMaxMp = packet.ReadShort();
                        var remainingAp = packet.ReadShort();
                        var remainingSp = packet.ReadShort();
                        var playerExp = packet.ReadInt();
                        var playerFame = packet.ReadShort();
                        var gachaExp = packet.ReadInt();
                        var playerMapId = packet.ReadInt();
                        var spawnPoint = packet.ReadByte();
                        var stats = new AvatarStats()
                        {
                            Id = playerId,
                            Name = playerName,
                            Gender = playerGender,
                            SkinId = playerSkin,
                            Face = playerFace,
                            Hair = playerHair,
                            Level = playerLevel,
                            JobId = playerJob,
                            Strength = playerStr,
                            Dexterity = playerDex,
                            Intelligence = playerInt,
                            Luck = playerLuk,
                            Hp = playerHp,
                            MaxHp = playerMaxHp,
                            Mp = playerMp,
                            MaxMp = playerMaxMp,
                            RemainingAp = remainingAp,
                            Exp = playerExp,
                            Fame = playerFame,
                            GachaExp = gachaExp,
                            MapId = playerMapId,
                            InitialSpawnPoint = spawnPoint
                        };
                        var look = new AvatarLook()
                        {
                            Gender = packet.ReadByte(),
                            SkinId = packet.ReadByte(),
                            Face = packet.ReadInt(),
                            Hair = packet.ReadInt(),
                            Equipment =
                            {
                                { PartType.Coat, 1040036 },
                                { PartType.Pants, 1060026 },
                                { PartType.Cape, 1102000 },
                                { PartType.Weapon, 1302000 }
                            }
                        };
                        var player = new Player(stats, look)
                        {
                            ID = ActorCount++,
                            Name = playerName,
                            Position = new Vector2(-120 + 125 * i, -1140),
                            Layer = ActorLayer.TileLayer1,
                            Visible = true,
                            IsLeft = false,
                            Moving = false,
                            ControlsLocked = true,
                            Z = 0,
                            Stats = stats,
                            Look = look
                        };
                        ((IWorld)this).AddActor(player);
                    }

                    // todo: character slots.
                    
                    _step = LoginStep.PickCharacter;
                    var frame = ui.GetUI("frame") as FramePanel;
                    frame.Position = _stepLocation[_step];
                    UpdateCamera(frame.Position);
                    Console.WriteLine($"Login Step: {LoginStep.PickCharacter} Location: {frame.Position}");
                }
                    break;
            }
        }
    }
    
    private void SendPacket(RequestOps request)
    {
        var packet = new OutPacket(request);
        switch (request)
        {
            case RequestOps.LoginPassword:
                var ui = ServiceLocator.Get<UIManager>();
                var signboard = ui.GetUI("signboard");
                var loginBtn = signboard?.GetNode("BtLogin") as TextureButton;
                var userbox = signboard?.GetNode("userbox") as TextBox;
                var passbox = signboard?.GetNode("passbox") as TextBox;
                loginBtn.State = ButtonState.Disabled;
                
                packet.WriteMapleString(userbox?.Text ?? "");
                packet.WriteMapleString(passbox?.Text ?? "");
                break;
            case RequestOps.ServerlistRequest:
                packet.WriteByte(0); // useless byte, kind of needed unforuntately
                break;
            case RequestOps.CharlistRequest:
                packet.WriteByte(_selectedWorld);
                packet.WriteByte(_selectedChannel);
                break;
        }
        
        packet.Send();
    }
    
    #endregion

    /// <summary>
    /// Updates the position of the camera based on a specified position.
    /// </summary>
    /// <param name="position">The position the camera needs to reference.</param>
    public void UpdateCamera(Vector2 position)
    {
        _camera.target = Vector2.Lerp(_camera.target, position, 1f);
    }

    public void UpdateZoom(float zoom)
    {
        _camera.zoom = zoom;
    }

    public IActor? GetActorById(int id)
    {
        return Actors.Find(x => x.ID == id);
    }
}

/// <summary>
/// Represents the different steps of the login process.
/// </summary>
public enum LoginStep
{
    /// <summary>
    /// Represents the starting step in the login process.
    /// </summary>
    Start,

    /// <summary>
    /// Represents the step in the login process where the player picks the world.
    /// </summary>
    PickWorld,

    /// <summary>
    /// Represents the login step where the player picks a character.
    /// </summary>
    PickCharacter,

    /// <summary>
    /// Represents the step in the login process where the user chooses their character class.
    /// </summary>
    ChooseClass,

    /// <summary>
    /// Represents the step in the login process where the player creates a new character.
    /// </summary>
    CreateCharacter
}
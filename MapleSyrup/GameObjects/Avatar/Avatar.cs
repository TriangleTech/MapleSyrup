using MapleSyrup.GameObjects.Components;
using MapleSyrup.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapleSyrup.GameObjects.Avatar;

public class Avatar : Actor
{
    private List<string> _possibleStates;
    private Dictionary<string, Texture2D> _bodyParts;
    private Dictionary<string, Texture2D> _equipments;

    public Avatar()
    {
        _possibleStates = new();
        _bodyParts = new();
        _equipments = new();
        LoadStates();
    }

    private void LoadStates()
    {
        var skinId = "00002000.img";
        var headId = "00012000.img";
        using var nx = ResourceManager.Instance["Character"].GetNode(skinId);

        foreach (var stateNode in nx.Children.Where(state => state.Name != "info"))
        {
            _possibleStates.Add(stateNode.Name);
        }
    }

    public override void Clear()
    {
        _possibleStates.Clear();
        _bodyParts.Clear();
        _equipments.Clear();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
    }

    public override void Update(GameTime gameTime)
    {
    }
}
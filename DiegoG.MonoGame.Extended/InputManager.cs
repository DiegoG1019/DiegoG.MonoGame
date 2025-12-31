using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;

namespace DiegoG.MonoGame.Extended;

public class InputManager(Game game) : IGameComponent, IUpdateable
{
    public MouseStateMemory MouseState { get; protected set; }
    public KeyboardStateExtended KeyboardState { get; protected set; }

    public void Initialize()
    {
    }

    public void Update(GameTime gameTime)
    {
        MouseState = new MouseStateMemory(MouseState, Mouse.GetState());
        KeyboardExtended.Update();
        KeyboardState = KeyboardExtended.GetState();
    }

    public bool Enabled
    {
        get;
        set
        {
            if (value == field) return;
            field = value;
            EnabledChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public int UpdateOrder => int.MaxValue;
    public event EventHandler<EventArgs>? EnabledChanged;
    public event EventHandler<EventArgs>? UpdateOrderChanged 
    {
        add { }
        remove { }
    }
}
/*using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace DiegoG.MonoGame.UI;

public abstract class UiObject : IUiObject
{
    public RectangleF Area { get; set; }
    public RectangleF? DraggableArea { get; set; }
    public bool Enabled { get; set; }
    public bool Visible { get; set; }
    
    public event StandaloneUiObjectEvent? LayerChanged;

    public int Layer
    {
        get;
        set
        {
            if (value == field) return;
            
            field = value;
            LayerChanged?.Invoke(this);
        }
    }
    
    public abstract void OnFocused(UiManager manager);
    public abstract void OnUnfocused(UiManager manager);
    public abstract void AddedTo(UiManager manager);
    public abstract void RemovedFrom(UiManager manager);
    public abstract void Draw(UiManager manager, GameTime time);
    public abstract bool OnMouseEnter(UiManager manager, in MouseStateExtended state, bool isHandled);
    public abstract bool OnMouseLeave(UiManager manager, in MouseStateExtended state, bool isHandled);
    public abstract bool OnMouseButtonPressed(UiManager manager, in MouseStateExtended state, bool isHandled);
    public abstract bool OnMouseButtonReleased(UiManager manager, in MouseStateExtended state, bool isHandled);
    public abstract bool OnKeyPressed(UiManager manager, in KeyboardEventArgs state, bool isHandled);
    public abstract bool OnKeyTyped(UiManager manager, in KeyboardEventArgs state, bool isHandled, char typedChar);
    public abstract bool OnKeyReleased(UiManager manager, in KeyboardEventArgs state, bool isHandled);
}*/
/*using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace DiegoG.MonoGame.UI;

public interface IUiObject
{
    public int Layer { get; set; }
    public RectangleF Area { get; set; }
    public RectangleF? DraggableArea { get; set; }
    
    /// <summary>
    /// <see langword="true"/> if the object should receive input events
    /// </summary>
    public bool Enabled { get; set; }
    
    /// <summary>
    /// <see langword="true"/> if the object should be visible; if <see langword="false"/>, <see cref="Enabled"/> is considered <see langword="false"/> by <see cref="UiManager"/>
    /// </summary>
    public bool Visible { get; set; }

    public event StandaloneUiObjectEvent? LayerChanged;

    /// <summary>
    /// Called when the object is focused by the given manager
    /// </summary>
    public void OnFocused(UiManager manager);
    
    /// <summary>
    /// Called when the object is unfocused by the given manager
    /// </summary>
    /// <param name="manager"></param>
    public void OnUnfocused(UiManager manager);

    /// <summary>
    /// Called when the object is added to the given manager
    /// </summary>
    /// <param name="manager"></param>
    public void AddedTo(UiManager manager);
    
    /// <summary>
    /// Called when the object is removed from the given manager
    /// </summary>
    /// <param name="manager"></param>
    public void RemovedFrom(UiManager manager);
    
    /// <summary>
    /// Draws the UIObject onto the Screen
    /// </summary>
    public void Draw(UiManager manager, GameTime time);
    
    /// <summary>
    /// Called when the mouse enters the object
    /// </summary>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="state">The state of the mouse when the method was called</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnMouseEnter(UiManager manager, in MouseStateExtended state, bool isHandled);
    
    /// <summary>
    /// Called when the mouse leaves the object
    /// </summary>
    /// <param name="state">The state of the mouse when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnMouseLeave(UiManager manager, in MouseStateExtended state, bool isHandled);
    
    /// <summary>
    /// Called when a mouse button is pressed on the object
    /// </summary>
    /// <param name="state">The state of the mouse when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnMouseButtonPressed(UiManager manager, in MouseStateExtended state, bool isHandled);
    
    /// <summary>
    /// Called when a mouse button is released on the object
    /// </summary>
    /// <param name="state">The state of the mouse when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnMouseButtonReleased(UiManager manager, in MouseStateExtended state, bool isHandled);
    
    /// <summary>
    /// Called when a key is pressed on the object
    /// </summary>
    /// <param name="state">The state of the keyboard when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnKeyPressed(UiManager manager, in KeyboardEventArgs state, bool isHandled);
    
    /// <summary>
    /// Called when a key is held on the object
    /// </summary>
    /// <param name="state">The state of the keyboard when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <param name="typedChar"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnKeyTyped(UiManager manager, in KeyboardEventArgs state, bool isHandled, char typedChar);
    
    /// <summary>
    /// Called when a key is released on the object
    /// </summary>
    /// <param name="state">The state of the keyboard when the method was called</param>
    /// <param name="isHandled"><see langword="true"/> when another UIObject in a higher layer has already handled the event, <see langword="false"/> otherwise</param>
    /// <param name="manager"></param>
    /// <returns><see langword="true"/> if this object has handled the event, <see langword="false"/> otherwise.</returns>
    public bool OnKeyReleased(UiManager manager, in KeyboardEventArgs state, bool isHandled);
}
*/
/*
using System.Diagnostics;
using DiegoG.MonoGame.Extended;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;

namespace DiegoG.MonoGame.UI; 

public abstract class UiManager : DrawableGameComponent
{
    private readonly List<IUiObject> uiObjects = [];
    private readonly KeyboardListener kbListener;
    private readonly MouseListener msListener;
    // TODO: Do collision checks

    protected UiManager(Game game, KeyboardListener kbListener, MouseListener msListener) : base(game)
    {
        this.kbListener = kbListener;
        this.msListener = msListener;
        
        this.kbListener.KeyPressed += KbListenerOnKeyPressed; 
        this.kbListener.KeyReleased += KbListenerOnKeyReleased;
        this.kbListener.KeyTyped += KbListenerOnKeyTyped;
    }

    // ReSharper disable once InconsistentlySynchronizedField // Since this collection is read only, there's no need to lock. If it's cast into a modifiable type, that's an user problem 
    public IReadOnlyCollection<IUiObject> UiObjects => uiObjects;

    public IUiObject? FocusedObject
    {
        get;
        set
        {
            if (value == field) return;
            
            if (field is not null)
                field.OnUnfocused(this);
            
            if (value is not null) 
                value.OnFocused(this);
            
            field = value;
            FocusedChanged?.Invoke(this, field);
        }
    }

    public event UiManagerObjectEvent? FocusedChanged;

    private bool HasMouseMoved => (pstate.X != cstate.X) || (pstate.Y != cstate.Y);
    private MouseState cstate;
    private MouseState pstate;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        cstate = Mouse.GetState();

        CheckButtonPressed(s => s.LeftButton, MouseButton.Left);
        CheckButtonPressed(s => s.MiddleButton, MouseButton.Middle);
        CheckButtonPressed(s => s.RightButton, MouseButton.Right);
        CheckButtonPressed(s => s.XButton1, MouseButton.XButton1);
        CheckButtonPressed(s => s.XButton2, MouseButton.XButton2);

        CheckButtonReleased(s => s.LeftButton, MouseButton.Left);
        CheckButtonReleased(s => s.MiddleButton, MouseButton.Middle);
        CheckButtonReleased(s => s.RightButton, MouseButton.Right);
        CheckButtonReleased(s => s.XButton1, MouseButton.XButton1);
        CheckButtonReleased(s => s.XButton2, MouseButton.XButton2);

        // Check for any sort of mouse movement.
        if (HasMouseMoved)
        {
            // TODO: This was taken from MouseListener, replace event calls with checking each bicho
            MouseMoved?.Invoke(this,
                new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, pstate, cstate));

            CheckMouseDragged(s => s.LeftButton, MouseButton.Left);
            CheckMouseDragged(s => s.MiddleButton, MouseButton.Middle);
            CheckMouseDragged(s => s.RightButton, MouseButton.Right);
            CheckMouseDragged(s => s.XButton1, MouseButton.XButton1);
            CheckMouseDragged(s => s.XButton2, MouseButton.XButton2);
        }

        // Handle mouse wheel events.
        if (pstate.ScrollWheelValue != cstate.ScrollWheelValue)
        {
            MouseWheelMoved?.Invoke(this,
                new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, pstate, cstate));
        }

        pstate = cstate;
        
        lock (uiObjects)
        {
            foreach (var obj in uiObjects)
            {
                if (obj.Visible && obj.Enabled)
                {
                    // Handle input
                }
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);

        lock (uiObjects)
        {
            foreach (var dr in uiObjects)
            {
                if (dr.Visible)
                    dr.Draw(this, gameTime);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        lock (uiObjects)
        {
            foreach (var obj in uiObjects) 
                obj.RemovedFrom(this);
            uiObjects.Clear();
        }
        
        kbListener.KeyPressed -= KbListenerOnKeyPressed; 
        kbListener.KeyReleased -= KbListenerOnKeyReleased;
        kbListener.KeyTyped -= KbListenerOnKeyTyped;

        base.Dispose(disposing);
    }
    
    public string? DebugName { get; set; }

    public bool RemoveObject(IUiObject uiObj)
        => RemoveObjectCore(uiObj);

    public void AddObject(IUiObject uiObject)
        => AddObjectCore(uiObject);
    
    #region Input Event Handlers
    
    private void CheckButtonPressed(Func<MouseState, ButtonState> getButtonState, MouseButton button)
    {
        if ((getButtonState(cstate) == ButtonState.Pressed) &&
            (getButtonState(pstate) == ButtonState.Released))
        {
            var args = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, pstate, cstate, button);

            MouseDown?.Invoke(this, args);
            _mouseDownArgs = args;

            if (_previousClickArgs != null)
            {
                // If the last click was recent
                var clickMilliseconds = (args.Time - _previousClickArgs.Value.Time).TotalMilliseconds;

                if (clickMilliseconds <= DoubleClickMilliseconds)
                {
                    MouseDoubleClicked?.Invoke(this, args);
                    _hasDoubleClicked = true;
                }

                _previousClickArgs = null;
            }
        }
    }

    private void CheckButtonReleased(Func<MouseState, ButtonState> getButtonState, MouseButton button)
    {
        if ((getButtonState(cstate) == ButtonState.Released) &&
            (getButtonState(pstate) == ButtonState.Pressed))
        {
            var args = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, pstate, cstate, button);

            if (_mouseDownArgs.Button == args.Button)
            {
                var clickMovement = DistanceBetween(args.Position, _mouseDownArgs.Position);

                // If the mouse hasn't moved much between mouse down and mouse up
                if (clickMovement < DragThreshold)
                {
                    if (!_hasDoubleClicked)
                        MouseClicked?.Invoke(this, args);
                }
                else // If the mouse has moved between mouse down and mouse up
                {
                    MouseDragEnd?.Invoke(this, args);
                    _dragging = false;
                }
            }

            MouseUp?.Invoke(this, args);

            _hasDoubleClicked = false;
            _previousClickArgs = args;
        }
    }

    private void CheckMouseDragged(Func<MouseState, ButtonState> getButtonState, MouseButton button)
    {
        if ((getButtonState(cstate) == ButtonState.Pressed) &&
            (getButtonState(pstate) == ButtonState.Pressed))
        {
            var args = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, pstate, cstate, button);

            if (_mouseDownArgs.Button == args.Button)
            {
                if (_dragging)
                    MouseDrag?.Invoke(this, args);
                else
                {
                    // Only start to drag based on DragThreshold
                    var clickMovement = DistanceBetween(args.Position, _mouseDownArgs.Position);

                    if (clickMovement > DragThreshold)
                    {
                        _dragging = true;
                        MouseDragStart?.Invoke(this, args);
                    }
                }
            }
        }
    }

    private static int DistanceBetween(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }
    
    private void KbListenerOnKeyTyped(KeyboardListener sender, KeyboardEventArgs e, char typedChar)
    {
        if (FocusedObject is { Enabled: true } focused)
            focused.OnKeyTyped(this, e, false, typedChar);
    }

    private void KbListenerOnKeyReleased(KeyboardListener sender, KeyboardEventArgs e)
    {
        if (FocusedObject is { Enabled: true } focused)
            focused.OnKeyReleased(this, e, false);
    }

    private void KbListenerOnKeyPressed(KeyboardListener sender, KeyboardEventArgs e)
    {
        if (FocusedObject is { Enabled: true } focused)
            focused.OnKeyPressed(this, e, false);
    }
    
    #endregion

    #region Sorting and event handling
    
    private bool RemoveObjectCore(IUiObject uiObj)
    {
        lock (uiObjects)
        {
            uiObj.LayerChanged -= OnLayerChanged;
            uiObj.RemovedFrom(this);
            return uiObjects.Remove(uiObj);
        }
    }
    
    private void AddObjectCore(IUiObject uiObj)
    {
        lock (uiObjects)
        {
            AddSorted(uiObj);
            uiObj.LayerChanged += OnLayerChanged;
            uiObj.AddedTo(this);
        }
    }

    private void OnLayerChanged(IUiObject uiObj)
    {
        MoveSorted(uiObj);
    }

    protected void MoveSorted(IUiObject item)
    {
        lock (uiObjects)
        {
            uiObjects.Remove(item);
            AddSorted(item);
        }
    }
    
    private void AddSorted(IUiObject item)
    {
        lock (uiObjects)
        {
            if (uiObjects.Count == 0 || uiObjects[^1].Layer.CompareTo(item.Layer) <= 0)
            {
                uiObjects.Add(item);
                return;
            }

            if (uiObjects[0].Layer.CompareTo(item.Layer) >= 0)
            {
                uiObjects.Insert(0, item);
                return;
            }
            
            var index = uiObjects.BinarySearch(item, UiObjectOrderComparer.Instance);
            if (index < 0) 
                index = ~index;
            
            uiObjects.Insert(index, item);
        }
    }
    
    #endregion
}
*/
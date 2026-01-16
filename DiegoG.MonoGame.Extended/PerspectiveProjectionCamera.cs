using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

// Based on https://community.monogame.net/t/fixed-and-free-3d-camera-code-example/11476

public enum CameraType
{
    Fixed,
    Free
}

public class PerspectiveProjectionCamera(Game game) : GameComponent(game), IDebugExplorable
{
    public float MovementUnitsPerSecond { get; set; } = 30f;
    public float RotationRadiansPerSecond { get; set; } = 60f;

    public float FieldOfView { get; set; } = float.DegreesToRadians(45);

    public float FieldOfViewDegrees
    {
        get => float.RadiansToDegrees(FieldOfView);
        set => FieldOfView = float.DegreesToRadians(value);
    }

    public float NearPlane { get; set; } = .05f;
    public float FarPlane { get; set; } = 1000f;

    public CameraType CameraType { get; set; } = CameraType.Fixed;

    public Vector3 TargetPosition { get; set; }
    
    public FacingDirection HorizontalFacingDirection { get; private set; }
    
    public FacingDirection VerticalFacingDirection { get; private set; }
    
    /// <summary>
    /// Gets or sets the camera's position in the world.
    /// </summary>
    public Vector3 Position
    {
        set
        {
            if (field == value) return;
            field = value;
            InvalidateWorldAndView();
        }
        get;
    }
    
    /// <summary>
    /// Gets or Sets the direction the camera is looking at in the world.
    /// The forward is the same as the look at direction it is a directional vector not a position.
    /// </summary>
    public Vector3 Forward
    {
        set
        {
            if (field == value) return;
            field = value;
            InvalidateWorldAndView();
        }
        get;
    } = Vector3.Forward;

    public Vector3 Backwards => -Forward;

    /// <summary>
    /// This serves as the cameras up. For fixed cameras this might not change at all ever. For free cameras it changes constantly.
    /// A fixed camera keeps a fixed horizon but can gimble lock under normal rotation when looking straight up or down.
    /// A free camera has no fixed horizon but can't gimble lock under normal rotation as the up changes as the camera moves.
    /// Most hybrid cameras are a blend of the two but all are based on one or both of the above.
    /// </summary>
    public Vector3 Up
    {
        set
        {
            if (field == value) return;
            field = value;
            InvalidateWorldAndView();
        }
        get;
    } = Vector3.Up;

    public Vector3 Down => -Up;

    public Vector3 Right
    {
        set
        {
            if (field == value) return;
            field = value;
            InvalidateWorldAndView();
        }
        get;
    } = Vector3.Right;

    public Vector3 Left => -Right;
    
    /// <summary>
    /// Directly set or get the world matrix this also updates the view matrix
    /// </summary>
    public Matrix World { get; private set; } = Matrix.Identity;

    /// <summary>
    /// Gets the view matrix we never really set the view matrix ourselves outside this method just get it.
    /// The view matrix is remade internally when we know the world matrix forward or position is altered.
    /// </summary>
    public Matrix View { get; private set; } = Matrix.Identity;

    /// <summary>
    /// Gets the projection matrix.
    /// </summary>
    public Matrix Projection { get; private set; } = Matrix.Identity;
    
    /// <summary>
    /// Gets a world matrix for 2D objects that shouldn't be skewed
    /// </summary>
    public Matrix World2D { get; private set; } = Matrix.Identity;

    public Vector2 ViewportCenter
    {
        get
        {
            var vp = Game.GraphicsDevice.Viewport;
            return new(vp.Width / 2f, vp.Height / 2f);
        }
    }

    private bool worldAndViewAreValid;
    private bool projectionIsValid;

    private void InvalidateWorldAndView() => worldAndViewAreValid = false;

    private void InvalidateProjection() => projectionIsValid = false;

    public event Action<PerspectiveProjectionCamera>? TransformationChanged;

    /// <summary>
    /// update the camera.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        if (TargetPosition != Position)
        {
            Position += ((TargetPosition - Position) * MovementUnitsPerSecond) *
                        (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        bool changed = false;
        if (worldAndViewAreValid is false)
        {
            Up = CameraType switch
            {
                CameraType.Fixed => Vector3.Up,
                CameraType.Free => World.Up,
                _ => Up
            };

            World2D = Matrix.CreateTranslation(new(-Position.X, -Position.Z, 0));
            World = Matrix.CreateWorld(default, Forward, Up);
            View = Matrix.CreateLookAt(Position, Forward + Position, World.Up);
            worldAndViewAreValid = true;
            changed = true;
        }

        if (projectionIsValid is false)
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
                Game.GraphicsDevice.Viewport.AspectRatio,
                NearPlane, FarPlane);
            
            projectionIsValid = true;
            changed = true;
        }

        if (changed) 
            TransformationChanged?.Invoke(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        Game.Window.ClientSizeChanged += WindowOnClientSizeChanged;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Game.Window.ClientSizeChanged -= WindowOnClientSizeChanged;
    }

    private void WindowOnClientSizeChanged(object? sender, EventArgs e)
    {
        InvalidateProjection();
    }

    /*
    /// <summary>
    /// like a fps games camera right clicking turns mouse look on or off same for the edit mode.
    /// </summary>
    private void FpsUiControlsLayout()
    {
        MouseState state = Mouse.GetState(Game.Window);
        KeyboardState kstate = Keyboard.GetState();
        if (kstate.IsKeyDown(Keys.W))
        {
            MoveForward();
        }
        else if (kstate.IsKeyDown(Keys.S) == true)
        {
            MoveBackward();
        }
        // strafe. 
        if (kstate.IsKeyDown(Keys.A) == true)
        {
            MoveLeft();
        }
        else if (kstate.IsKeyDown(Keys.D) == true)
        {
            MoveRight();
        }

        // rotate 
        if (kstate.IsKeyDown(Keys.Left) == true)
        {
            RotateLeft();
        }
        else if (kstate.IsKeyDown(Keys.Right) == true)
        {
            RotateRight();
        }
        // rotate 
        if (kstate.IsKeyDown(Keys.Up) == true)
        {
            RotateUp();
        }
        else if (kstate.IsKeyDown(Keys.Down) == true)
        {
            RotateDown();
        }

        if (kstate.IsKeyDown(Keys.Q) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FIXED)
                MoveUpInNonLocalSystemCoordinates();
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                MoveUp();
        }
        else if (kstate.IsKeyDown(Keys.E) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FIXED)
                MoveDownInNonLocalSystemCoordinates();
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                MoveDown();
        }


        if (state.LeftButton == ButtonState.Pressed)
        {
            if (mouseLookIsUsed == false)
                mouseLookIsUsed = true;
            else
                mouseLookIsUsed = false;
        }
        if (mouseLookIsUsed)
        {
            Vector2 diff = state.Position.ToVector2() - mState.Position.ToVector2();
            if (diff.X != 0f)
                RotateLeftOrRight(, diff.X);
            if (diff.Y != 0f)
                RotateUpOrDown(, diff.Y);
        }
        mState = state;
        kbState = kstate;
    }

    /// <summary>
    /// when working like programing editing and stuff.
    /// </summary>
    /// <param name="gameTime"></param>
    private void EditingUiControlsLayout(GameTime gameTime)
    {
        MouseState state = Mouse.GetState(Game.Window);
        KeyboardState kstate = Keyboard.GetState();
        if (kstate.IsKeyDown(Keys.E))
        {
            MoveForward(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Q) == true)
        {
            MoveBackward(gameTime);
        }
        if (kstate.IsKeyDown(Keys.W))
        {
            RotateUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.S) == true)
        {
            RotateDown(gameTime);
        }
        if (kstate.IsKeyDown(Keys.A) == true)
        {
            RotateLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.D) == true)
        {
            RotateRight(gameTime);
        }

        if (kstate.IsKeyDown(Keys.Left) == true)
        {
            MoveLeft(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Right) == true)
        {
            MoveRight(gameTime);
        }
        // rotate 
        if (kstate.IsKeyDown(Keys.Up) == true)
        {
            MoveUp(gameTime);
        }
        else if (kstate.IsKeyDown(Keys.Down) == true)
        {
            MoveDown(gameTime);
        }

        // roll counter clockwise
        if (kstate.IsKeyDown(Keys.Z) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                RotateRollCounterClockwise(gameTime);
        }
        
        // roll clockwise
        else if (kstate.IsKeyDown(Keys.C) == true)
        {
            if (cameraTypeOption == CAM_TYPE_OPTION_FREE)
                RotateRollClockwise(gameTime);
        }

        if (state.RightButton == ButtonState.Pressed)
            mouseLookIsUsed = true;
        else
            mouseLookIsUsed = false;
        if (mouseLookIsUsed)
        {
            Vector2 diff = state.Position.ToVector2() - mState.Position.ToVector2();
            if (diff.X != 0f)
                RotateLeftOrRight(gameTime, diff.X);
            if (diff.Y != 0f)
                RotateUpOrDown(gameTime, diff.Y);
        }
        
        mState = state;
        kbState = kstate;
    }

    */

    public void LookAt(Vector3 location)
    {
        Forward = Vector3.Normalize(location - Position);
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
        HorizontalFacingDirection = new FacingDirection(new Vector2(Forward.X, Forward.Z).AngleBetweenUp());
        VerticalFacingDirection = new FacingDirection(new Vector2(Forward.X, Forward.Y).AngleBetweenUp());
    }

    public void ProjectTo2DSpace(Vector3 position3d, out Vector2 position2d, out float scale, out float rotation)
    {
        float f = 1.0f / float.Tan(FieldOfView / 2);
        float rangeInv = 1.0f / (NearPlane - FarPlane);
        float aspect = Game.GraphicsDevice.Viewport.AspectRatio;

        position2d = new Vector2((position3d.X * f) / aspect, position3d.Y * f);
        scale = (NearPlane + FarPlane) * rangeInv * position3d.Z;
        rotation = 1;
    }

    /// <summary>
    /// This function can be used to check if gimble is about to occur in a fixed camera.
    /// If this value returns 1.0f you are in a state of gimble lock, However even as it gets near to 1.0f you are in danger of problems.
    /// In this case you should interpolate towards a free camera. Or begin to handle it.
    /// Earlier then .9 in some manner you deem to appear fitting otherwise you will get a hard spin effect. Though you may want that.
    /// </summary>
    public float GetGimbleLockDangerValue()
    {
        var c0 = Vector3.Dot(World.Forward, World.Up);
        if (c0 < 0f) c0 = -c0;
        return c0;
    }
    
    #region Local Translations and Rotations.
    
    public void MoveTo(Vector3 position)
    {
        TargetPosition = position;
    }
    
    public void Move(Vector3 movementVector)
    {
        TargetPosition += movementVector;
    }

    /*
    public void MoveForward()
    {
        TargetPosition += Forward;
    }
    
    public void MoveBackward()
    {
        TargetPosition += World.Backward;
    }
    
    public void MoveLeft()
    {
        TargetPosition += World.Left;
    }
    
    public void MoveRight()
    {
        TargetPosition += World.Right;
    }
    
    public void MoveUp()
    {
        TargetPosition += World.Up;
    }
    
    public void MoveDown()
    {
        TargetPosition += World.Down;
    }

    public void RotateUp()
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Right, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    
    public void RotateDown()
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Right, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    
    public void RotateLeft()
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Up, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    
    public void RotateRight()
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Up, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    
    public void RotateRollClockwise()
    {
        var radians = RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var pos = Position;
        World *= Matrix.CreateFromAxisAngle(Forward, MathHelper.ToRadians(radians));
        Position = pos;
        InvalidateWorldAndView();
    }
    
    public void RotateRollCounterClockwise()
    {
        var radians = -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var pos = Position;
        World *= Matrix.CreateFromAxisAngle(Forward, MathHelper.ToRadians(radians));
        Position = pos;
        InvalidateWorldAndView();
    }

    // just for example this is the same as the above rotate left or right.
    public void RotateLeftOrRight(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Up, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    public void RotateUpOrDown(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(World.Right, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    } 

    #endregion

    #region Non Local System Translations and Rotations.

    public void MoveForwardInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Forward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public void MoveBackwardsInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Backward * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public void MoveUpInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Up * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public void MoveDownInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Down * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public void MoveLeftInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Left * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public void MoveRightInNonLocalSystemCoordinates()
    {
        TargetPosition += (Vector3.Right * MovementUnitsPerSecond) * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    /// <summary>
    /// These aren't typically useful and you would just use create world for a camera snap to a new view. I leave them for completeness.
    /// </summary>
    public void NonLocalRotateLeftOrRight(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }
    /// <summary>
    /// These aren't typically useful and you would just use create world for a camera snap to a new view.  I leave them for completeness.
    /// </summary>
    public void NonLocalRotateUpOrDown(GameTime gameTime, float amount)
    {
        var radians = amount * -RotationRadiansPerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Matrix matrix = Matrix.CreateFromAxisAngle(Vector3.Right, MathHelper.ToRadians(radians));
        Forward = Vector3.TransformNormal(Forward, matrix);
        InvalidateWorldAndView();
    }

    */
    #endregion

    public void RenderImGuiDebug()
    {
        void PrintVectors()
        {
            Span<char> bf = stackalloc char[38];
        
            ImGui.LabelText("Up Vector", Up.ToStringSpan(bf, "0.0000"));
            ImGui.LabelText("Right Vector", World.Right.ToStringSpan(bf, "0.0000"));
            ImGui.LabelText("Forward Vector", Forward.ToStringSpan(bf, "0.0000"));
            ImGui.LabelText("Position Vector", Position.ToStringSpan(bf, "0.0000"));
        }
        
        PrintVectors();
        
        var __Far = FarPlane;
        if (ImGui.InputFloat("Far Plane", ref __Far) && __Far > 0.0 && __Far > NearPlane)
        {
            FarPlane = __Far;
            InvalidateProjection();
            InvalidateWorldAndView();
        }
        
        var __Near = NearPlane;
        if (ImGui.InputFloat("Near Plane", ref __Near) && __Near > 0.0 && __Near < FarPlane)
        {
            NearPlane = __Near;
            InvalidateProjection();
            InvalidateWorldAndView();
        }

        var __fovdeg = FieldOfViewDegrees;
        if (ImGui.InputFloat("Field of View (Degrees)", ref __fovdeg) && __fovdeg > 0.0 && __fovdeg < 360)
        {
            FieldOfViewDegrees = __fovdeg;
            InvalidateProjection();
            InvalidateWorldAndView();
        }

        var __Fov = FieldOfView;
        if (ImGui.InputFloat("Field of View (Radians)", ref __Fov) && __Fov > 0.0 && __Fov < 3.1415929794311523)
        {
            FieldOfView = __Fov;
            InvalidateProjection();
            InvalidateWorldAndView();
        }

        var __pos = TargetPosition.ToNumerics();
        if (ImGui.InputFloat3("Target Position", ref __pos))
        {
            TargetPosition = __pos;
            InvalidateProjection();
            InvalidateWorldAndView();
        }
    }
}
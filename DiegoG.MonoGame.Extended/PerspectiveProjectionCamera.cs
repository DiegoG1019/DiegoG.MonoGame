// Code imported and modified from https://github.com/Atlanticity91/MonoVoxel/blob/master/MonoVoxel/Engine/Utils/MonoVoxelCamera.cs

using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace DiegoG.MonoGame.Extended;

public class PerspectiveProjectionCamera : IDebugExplorable
{
    private Matrix m_projection = Matrix.Identity;
    private Matrix m_view = Matrix.Identity;
    private Matrix m_world = Matrix.Identity;
    private Matrix m_transform;

    private Vector3 m_up = new(0.0f, 1.0f, 0.0f);
    private Vector3 m_right = new(1.0f, 0.0f, 0.0f);
    private Vector3 m_forward = new(0.0f, 0.0f, -1.0f);

    private readonly float m_pitch_min = -MathHelper.ToRadians(89);
    private readonly float m_pitch_max = MathHelper.ToRadians(89);

    public Vector3 Up => m_up;
    public Vector3 Right => m_right;
    public Vector3 Forward => m_forward;

    public Matrix Projection => m_projection;
    public Matrix View => m_view;
    public Matrix World => m_world;
    public Matrix Transform => m_transform;

    public RectangleF CameraArea { get; private set; }
    public Viewport CameraViewport { get; private set; }
    public float FieldOfView { get; set; } = MathHelper.ToRadians(45);
    public float Yaw { get; set; } = 0.0f;
    public float Pitch { get; set; } = 0.0f;
    public Vector3 Position { get; set; }
    public float Far { get; set; } = 100.0f;
    public float Near { get; set; } = 0.1f;
    
    /// <summary>
    /// Rotate pitch.
    /// </summary>
    /// <param name="delta_y" >Delta pitch in radians</param>
    public void RotatePitch(float delta_y)
    {
        Pitch -= delta_y;
        Pitch = Math.Clamp( Pitch, m_pitch_min, m_pitch_max );
    }

    /// <summary>
    /// Rotate yaw.
    /// </summary>
    /// <param name="delta_x" >Delta yaw in radians</param>
    public void RotateYaw(float delta_x)
        => Yaw += delta_x;

    /// <summary>
    /// Rotate pitch and yaw.
    /// </summary>
    /// <param name="rotation" >Rotation for pitch and yaw, vector in radians</param>
    public void Rotate(Vector2 rotation)
    {
        RotateYaw(rotation.X);
        RotatePitch(rotation.Y);
    }

    /// <summary>
    /// Rotate pitch and yaw.
    /// </summary>
    /// <param name="yaw" >Delta yaw in radians</param>
    /// <param name="pitch" >Delta pitch in radians</param>
    public void Rotate(float yaw, float pitch)
    {
        RotateYaw(yaw);
        RotatePitch(pitch);
    }

    /// <summary>
    /// Move to the left.
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveLeft(float velocity)
        => Position -= m_right * velocity;

    /// <summary>
    /// Move to the right.
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveRight(float velocity)
        => Position += m_right * velocity;

    /// <summary>
    /// Move up.
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveUp(float velocity)
        => Position += m_up * velocity;

    /// <summary>
    /// Move down
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveDown(float velocity)
        => Position -= m_up * velocity;

    /// <summary>
    /// Move forward.
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveForward(float velocity)
        => Position += m_forward * velocity;

    /// <summary>
    /// Move backward.
    /// </summary>
    /// <param name="velocity" >Movement velocity</param>
    public void MoveBackward(float velocity)
        => Position -= m_forward * velocity;

    public void Move(Vector3 moveVector)
    {
        Position += m_forward * moveVector.X;
        Position += m_right * moveVector.Y;
        Position += m_up * moveVector.Z;
    }

    public void MoveAbsolute(Vector2 moveVector)
    {
        Position += new Vector3(moveVector, 0);
    }

    public void MoveAbsolute(Vector3 moveVector)
    {
        Position += moveVector;
    }

    public void MoveTo(Vector2 position)
    {
        MoveAbsolute(position - new Vector2(Position.X, Position.Y));
    }

    public void MoveTo(Vector3 position)
    {
        MoveAbsolute(position - Position);
    }

    /// <summary>
    /// Tick camera, update matrices.
    /// </summary>
    /// <param name="device">Current graphics device instance</param>
    public void Tick(GraphicsDevice device)
    {
        CameraViewport = device.Viewport;
        CameraArea = new(Position.X, Position.Y, CameraViewport.Width, CameraViewport.Height); 

        // Update Forward Vector
        m_forward.X = MathF.Cos(Yaw) * MathF.Cos(Pitch);
        m_forward.Y = MathF.Sin(Pitch);
        m_forward.Z = MathF.Sin(Yaw) * MathF.Cos(Pitch);
        m_forward.Normalize();

        // Update Right Vector
        m_right = Vector3.Cross(m_forward, new Vector3(0.0f, 1.0f, 0.0f));
        m_right.Normalize();

        // Update North
        m_up = Vector3.Cross(m_right, m_forward);
        m_up.Normalize();

        // Update matrices
        m_projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView, CameraViewport.AspectRatio, Near, Far);
        m_view = Matrix.CreateLookAt(Position, default, m_up);
        m_world = Matrix.CreateTranslation(-Position);// * Matrix.CreateScale();

        Matrix.Multiply(ref m_world, ref m_view, out m_transform);
        Matrix.Multiply(ref m_transform, ref m_projection, out m_transform);
    }

    public Vector2 GetAspect()
        => new(
            2.0f * MathF.Atan(MathF.Tan(FieldOfView * 0.5f) * CameraViewport.AspectRatio),
            FieldOfView
        );

    public Vector2 GetHalfAspect()
        => GetAspect() * 0.5f;

    public void RenderImGuiDebug()
    {
        void PrintVectors()
        {
            Span<char> bf = stackalloc char[38];
        
            ImGui.LabelText("North Vector", Up.ToStringSpan(bf, "0.0000"));
            ImGui.LabelText("Right Vector", Right.ToStringSpan(bf, "0.0000"));
            ImGui.LabelText("Forward Vector", Forward.ToStringSpan(bf, "0.0000"));

        }
        
        var __Yaw = Yaw;
        if (ImGui.InputFloat("Yaw", ref __Yaw))
            Yaw = __Yaw;
        
        var __Pitch = Pitch;
        if (ImGui.InputFloat("Pitch", ref __Pitch))
            Pitch = __Pitch;
        
        var __Far = Far;
        if (ImGui.InputFloat("Far Plane", ref __Far) && __Far > 0.0 && __Far > Near)
            Far = __Far;
        
        var __Near = Near;
        if (ImGui.InputFloat("Near Plane", ref __Near) && __Near > 0.0 && __Near < Far)
            Near = __Near;

        var __Fov = FieldOfView;
        if (ImGui.InputFloat("Field of View", ref __Fov) && __Fov > 0.0 && __Fov < 3.1415929794311523)
            FieldOfView = __Fov;

        var __pos = Position.ToNumerics();
        if (ImGui.InputFloat3("Position", ref __pos))
            Position = __pos;
    }
}

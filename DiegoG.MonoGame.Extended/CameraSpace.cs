using GLV.Shared.Common;
using GLV.Shared.Common.Text;
using ImGuiNET;
using MonoGame.Extended;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DiegoG.MonoGame.Extended;

public static class CameraExtensions
{
    public static void RenderImGuiDebug(this Camera<Vector2> cam)
    {
        var _Position = cam.Position.ToNumerics();
        if (ImGui.InputFloat2("Position", ref _Position))
            cam.Position = _Position;
        
        float _Rotation = cam.Rotation;
        if (ImGui.InputFloat("Rotation", ref _Rotation))
            cam.Rotation = _Rotation;
        
        float _Zoom = cam.Zoom;
        if (ImGui.InputFloat("Zoom", ref _Zoom))
            cam.Zoom = _Zoom;
        
        float _MinimumZoom = cam.MinimumZoom;
        if (ImGui.InputFloat("Minimum Zoom", ref _MinimumZoom))
            cam.MinimumZoom = _MinimumZoom;
        
        float _MaximumZoom = cam.MaximumZoom;
        if (ImGui.InputFloat("Maximum Zoom", ref _MaximumZoom))
            cam.MaximumZoom = _MaximumZoom;
        
        float _Pitch = cam.Pitch;
        if (ImGui.InputFloat("Pitch", ref _Pitch))
            cam.Pitch = _Pitch;
        
        float _MinimumPitch = cam.MinimumPitch;
        if (ImGui.InputFloat("Minimum Pitch", ref _MinimumPitch))
            cam.MinimumPitch = _MinimumPitch;
        
        float _MaximumPitch = cam.MaximumPitch;
        if (ImGui.InputFloat("Maximum Pitch", ref _MaximumPitch))
            cam.MaximumPitch = _MaximumPitch;

        var _Origin = cam.Origin.ToNumerics();
        if (ImGui.InputFloat2("Origin", ref _Origin))
            cam.Origin = _Origin;
        
        RenderImGuiOtherLabels(cam);
        
        return;

        static void RenderImGuiOtherLabels(Camera<Vector2> cam)
        {
            Span<char> sbb = stackalloc char[200];
            Span<char> bf = stackalloc char[20];
            var sb = new ValueStringBuilder(sbb);
            
            sb.Append("X: "); sb.Append(cam.BoundingRectangle.X.ToStringSpan(bf)); sb.Append(' ');
            sb.Append("Y: "); sb.Append(cam.BoundingRectangle.Y.ToStringSpan(bf)); sb.Append(' ');
            sb.Append("W: "); sb.Append(cam.BoundingRectangle.Width.ToStringSpan(bf)); sb.Append(' ');
            sb.Append("H: "); sb.Append(cam.BoundingRectangle.Height.ToStringSpan(bf));
            ImGui.LabelText("Bounding Rectangle", sb.AsSpan());
            
            sb.Clear();
            sb.Append("X: "); sb.Append(cam.BoundingRectangle.X.ToStringSpan(bf));
            sb.Append("Y: "); sb.Append(cam.BoundingRectangle.Y.ToStringSpan(bf));
            ImGui.LabelText("Center", sb.AsSpan());
        }
    }
}

//public readonly record struct SpriteDescription(string TexturePath, Rectangle? Region)
//{
//    public Sprite CreateSprite(ContentManager content)
//    {
//        var tex = content.Load<Texture2DRegion>(TexturePath);
//        return new Sprite();
//    }
//}

public class CameraSpace(Camera<Vector2> camera) : ISpace
{
    public Camera<Vector2> Camera { get; } = camera;
    public Matrix Transform => Camera.GetViewMatrix();
    public Matrix InverseTransform => Camera.GetInverseViewMatrix();
}

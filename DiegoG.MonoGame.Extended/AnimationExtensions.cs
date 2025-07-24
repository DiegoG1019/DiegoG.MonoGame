using System.Text;
using GLV.Shared.Common;
using ImGuiNET;
using MonoGame.Extended.Animations;

namespace DiegoG.MonoGame.Extended;

public static class AnimationExtensions
{
    public static void Restart(this IAnimationController controller)
    {
        controller.Play(0);
        controller.Unpause(true);
    }

    public static void RenderImGuiDebug(this IAnimationController controller)
    {
        Span<char> buffer = stackalloc char[100];

        ImGui.LabelText("IsPaused", controller.IsPaused.ToString());
        ImGui.LabelText("IsAnimating", controller.IsAnimating.ToString());
        ImGui.LabelText("IsLooping", controller.IsLooping.ToString());
        ImGui.LabelText("IsReversed", controller.IsReversed.ToString());
        ImGui.LabelText("IsPingPong", controller.IsPingPong.ToString());
        ImGui.LabelText("Speed", controller.Speed.ToStringSpan(buffer));
        ImGui.LabelText("CurrentFrameTimeRemaining", controller.CurrentFrameTimeRemaining.ToStringSpan(buffer));
        ImGui.LabelText("CurrentFrame", controller.CurrentFrame.ToStringSpan(buffer));
        ImGui.LabelText("FrameCount", controller.FrameCount.ToStringSpan(buffer));
    }
}
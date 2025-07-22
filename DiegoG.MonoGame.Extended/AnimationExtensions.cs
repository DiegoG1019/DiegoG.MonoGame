using System.Text;
using GLV.Shared.Common;
using MonoGame.Extended.Animations;

namespace DiegoG.MonoGame.Extended;

public static class AnimationExtensions
{
    public static void Restart(this IAnimationController controller)
    {
        controller.Play(0);
        controller.Unpause(true);
    }

    public static void DebugDump(this IAnimationController controller, StringBuilder sb, int tabs)
    {
        Span<char> buffer = stackalloc char[100];
        
        sb.AppendTabs(tabs).Append("IsPaused: ").Append(controller.IsPaused).AppendLine();
        sb.AppendTabs(tabs).Append("IsAnimating: ").Append(controller.IsAnimating).AppendLine();
        sb.AppendTabs(tabs).Append("IsLooping: ").Append(controller.IsLooping).AppendLine();
        sb.AppendTabs(tabs).Append("IsReversed: ").Append(controller.IsReversed).AppendLine();
        sb.AppendTabs(tabs).Append("IsPingPong: ").Append(controller.IsPingPong).AppendLine();
        sb.AppendTabs(tabs).Append("Speed: ").Append(controller.Speed.ToStringSpan(buffer)).AppendLine();
        sb.AppendTabs(tabs).Append("CurrentFrameTimeRemaining: ").Append(controller.CurrentFrameTimeRemaining.ToStringSpan(buffer)).AppendLine();
        sb.AppendTabs(tabs).Append("CurrentFrame: ").Append(controller.CurrentFrame.ToStringSpan(buffer)).AppendLine();
        sb.AppendTabs(tabs).Append("FrameCount: ").Append(controller.FrameCount.ToStringSpan(buffer)).AppendLine();
    }
}
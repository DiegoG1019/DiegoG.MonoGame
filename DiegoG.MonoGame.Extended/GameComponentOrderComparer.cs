using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public sealed class GameComponentOrderComparer : IComparer<IUpdateable>, IComparer<IDrawable>
{
    private GameComponentOrderComparer() { }

    public static GameComponentOrderComparer Instance { get; } = new();

    public int Compare(IUpdateable? x, IUpdateable? y) 
        => x == y ? 0 : x is null ? -1 : y is null ? 1 : x.UpdateOrder.CompareTo(y.UpdateOrder);
        // ^ If they're the same object
        //               ^ We already know both aren't null (null == null -> true)

    public int Compare(IDrawable? x, IDrawable? y)
        => x == y ? 0 : x is null ? -1 : y is null ? 1 : x.DrawOrder.CompareTo(y.DrawOrder);
}
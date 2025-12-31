/*namespace DiegoG.MonoGame.UI;

public sealed class UiObjectOrderComparer : IComparer<IUiObject>
{
    private UiObjectOrderComparer() { }

    public static UiObjectOrderComparer Instance { get; } = new();

    public int Compare(IUiObject? x, IUiObject? y) 
        => x == y ? 0 : x is null ? -1 : y is null ? 1 : x.Layer.CompareTo(y.Layer);
    // ^ If they're the same object
    //               ^ We already know both aren't null (null == null -> true)
}*/
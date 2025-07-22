using System.Diagnostics;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace DiegoG.MonoGame.Extended;

public sealed class ChainAssetLoadTracker
{
    private readonly ContentManager manager;
    private readonly HashSet<string> loadedSet;
    
    public ChainAssetLoadTracker LoadLocalized<T>(string assetName, out T asset)
    {
        loadedSet.Add(assetName);
        asset = manager.LoadLocalized<T>(assetName);
        return this;
    }
    
    public ChainAssetLoadTracker Load<T>(string assetName, out T asset)
    {
        loadedSet.Add(assetName);
        asset = manager.Load<T>(assetName);
        return this;   
    }

    public void UnloadTheRest()
    {
        manager.UnloadExcept(loadedSet);
    }

    internal ChainAssetLoadTracker(ContentManager manager)
    {
        this.manager = manager;
        loadedSet = [];
    }
}

public static class ContentManagerExtensions
{
    private static readonly FieldInfo ContentManagerLoadedAssetsProp;
    
    static ContentManagerExtensions()
    {
        ContentManagerLoadedAssetsProp = typeof(ContentManager).GetField("loadedAssets", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance)!;
        Debug.Assert(ContentManagerLoadedAssetsProp is not null);
    }
    
    public static void UnloadExcept(this ContentManager manager, params ICollection<string> assetsToSpare)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(assetsToSpare);
        
        var assetCollection =
            (Dictionary<string, object>)ContentManagerLoadedAssetsProp.GetValue(manager)!;

        if (assetsToSpare is not HashSet<string> && assetsToSpare.Count > 100)
            assetsToSpare = assetsToSpare.ToHashSet();
        
        foreach (var (name, asset) in assetCollection)
            if (assetsToSpare.Contains(name) is false)
                manager.UnloadAsset(name);
    }

    public static ChainAssetLoadTracker LoadAssetsAndUnloadNotNeeded(this ContentManager manager)
    {
        ArgumentNullException.ThrowIfNull(manager);
        return new ChainAssetLoadTracker(manager);
    }
}
using Microsoft.Xna.Framework;

namespace DiegoG.MonoGame.Extended;

public static class CallDeferrerExtensions
{
    public static void DeferToUpdateStart(this Game game, CallDeferrer.DeferredGameCall action) 
        => game.Services.GetService<CallDeferrer>().DeferToUpdateStart(action);
    
    public static void DeferToUpdateEnd(this Game game, CallDeferrer.DeferredGameCall action) 
        => game.Services.GetService<CallDeferrer>().DeferToUpdateEnd(action);
    
    public static void DeferToDrawStart(this Game game, CallDeferrer.DeferredGameCall action) 
        => game.Services.GetService<CallDeferrer>().DeferToDrawStart(action);
    
    public static void DeferToDrawEnd(this Game game, CallDeferrer.DeferredGameCall action) 
        => game.Services.GetService<CallDeferrer>().DeferToDrawEnd(action);

    public static CallDeferrer AddCallDeferrerService(this GameServiceContainer services, Game game)
    {
        var deferrer = new CallDeferrer(game);
        services.AddService<CallDeferrer>(deferrer);
        return deferrer;
    }
    // I'd like to keep this slightly tucked away from normal view, so the extension is on GameServiceContainer rather than on Game
}

public class CallDeferrer(Game game)
{
    public delegate void DeferredGameCall(Game game, GameTime gameTime);
    
    private readonly Queue<DeferredGameCall> updateStartQueue = [];
    private readonly Queue<DeferredGameCall> updateEndQueue = [];
    private readonly Queue<DeferredGameCall> drawStartQueue = [];
    private readonly Queue<DeferredGameCall> drawEndQueue = [];

    private static void DeferTo(DeferredGameCall action, Queue<DeferredGameCall> q) => q.Enqueue(action);

    public void DeferToUpdateStart(DeferredGameCall action) => DeferTo(action, updateStartQueue);
    public void DeferToUpdateEnd(DeferredGameCall action) => DeferTo(action, updateEndQueue);
    public void DeferToDrawStart(DeferredGameCall action) => DeferTo(action, drawStartQueue);
    public void DeferToDrawEnd(DeferredGameCall action) => DeferTo(action, drawEndQueue);

    private void Execute(Queue<DeferredGameCall> q, GameTime gameTime)
    {
        while (q.TryDequeue(out var c)) c.Invoke(game, gameTime);
    }

    public void ExecuteUpdateStartDeferredCalls(GameTime gameTime) => Execute(updateStartQueue, gameTime);
    public void ExecuteUpdateEndDeferredCalls(GameTime gameTime) => Execute(updateEndQueue, gameTime);
    public void ExecuteDrawStartDeferredCalls(GameTime gameTime) => Execute(drawStartQueue, gameTime);
    public void ExecuteDrawEndDeferredCalls(GameTime gameTime) => Execute(drawEndQueue, gameTime);
}
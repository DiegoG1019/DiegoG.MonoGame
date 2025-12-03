using System.Diagnostics;
using System.Reflection;
using GLV.Shared.Common;
using GLV.Shared.Common.Text;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace DiegoG.MonoGame.Extended.UIComponents;

public class DebugImGuiViews(Game game) : DrawableGameComponent(game)
{
    static DebugImGuiViews()
    {
        ContentManagerLoadedAssets = typeof(ContentManager).GetField("loadedAssets", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance)!;
        Debug.Assert(ContentManagerLoadedAssets is not null);
        Debug.Assert(ContentManagerLoadedAssets.FieldType == typeof(Dictionary<string, object>));

        GameServiceContainerDict = typeof(GameServiceContainer).GetField("services",
            BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance)!;
        Debug.Assert(GameServiceContainerDict is not null);
        Debug.Assert(GameServiceContainerDict.FieldType == typeof(Dictionary<Type, object>));
    }

    private delegate void DebugFormatter<in T>(T value, ref ValueStringBuilder sb);

    private static readonly FieldInfo GameServiceContainerDict;
    private static readonly FieldInfo ContentManagerLoadedAssets; 
    
    public bool DebugViews { get; set; } = true;
    private bool ShowMetricsWindow;
    private readonly HashSet<IGameComponent> exploredComponents = [];

    public override void Draw(GameTime gameTime)
    {
        exploredComponents.Clear();
        base.Draw(gameTime);
        
        if (!DebugViews) return;
        
        ImGui.Begin("Debug Tools");

        ImGui.Checkbox("Show metrics window", ref ShowMetricsWindow);
        if (ShowMetricsWindow)
            ImGui.ShowMetricsWindow();

        if (ImGui.CollapsingHeader("All Components"))
            RenderComponentBlock();

        if (ImGui.CollapsingHeader("All Game Services"))
        {
            var dict = (Dictionary<Type, object>)GameServiceContainerDict.GetValue(Game.Services)!;
            DictionaryBulletsBlock(
                dict,
                "Service Type",
                "Implemented By",
                (Type k, ref ValueStringBuilder sb) => sb.Append(k.GetCSharpTypeExpression()),
                (object o, ref ValueStringBuilder sb) => sb.Append(o.GetType().GetCSharpTypeExpression())
            );
        }

        if (ImGui.CollapsingHeader("All Loaded Assets"))
        {
            var dict = (Dictionary<string, object>)ContentManagerLoadedAssets.GetValue(Game.Content)!;
            DictionaryBulletsBlock(
                dict,
                "Asset Name",
                "Asset",
                (string k, ref ValueStringBuilder sb) => sb.Append(k),
                (object o, ref ValueStringBuilder sb) => sb.Append(o.GetType().Name)
            );
        }
        
        ImGui.End();
    }

    private void RenderDictionaryBullets<TKey>(
        Dictionary<TKey, object> dict, 
        Span<char> buffer, 
        Span<char> smallbuffer, 
        ref ValueStringBuilder sb, 
        string keyText, 
        string valueText,
        DebugFormatter<TKey> keyFormatter,
        DebugFormatter<object> valueFormatter
    ) where TKey : notnull
    {
        foreach (var (k, v) in dict)
        {
            sb.Clear();
            sb.Append(keyText);
            sb.Append(" '");
            keyFormatter(k, ref sb);
            sb.Append("' ");
            sb.Append(valueText);
            sb.Append(" '");
            valueFormatter(v, ref sb);
            sb.Append('\'');
            ImGui.BulletText(sb.AsSpan());
        } 
    }

    private void DictionaryBulletsBlock<TKey>(
        Dictionary<TKey, object> dict, 
        string? keyText = null, 
        string? valueText = null,
        DebugFormatter<TKey>? keyFormatter = null,
        DebugFormatter<object>? valueFormatter = null
    ) where TKey : notnull
    {
        Span<char> buffer = stackalloc char[800];
        Span<char> smallbuffer = stackalloc char[16];
        var sb = new ValueStringBuilder(buffer);
        RenderDictionaryBullets(
            dict, 
            buffer, 
            smallbuffer, 
            ref sb,
            keyText ?? "Key",
            valueText ?? "Value",
            keyFormatter ?? ((TKey k, ref ValueStringBuilder sb) => sb.Append(k.ToString())),
            valueFormatter ?? ((object k, ref ValueStringBuilder sb) => sb.Append(k.ToString()))
        );
    }

    internal void RenderComponentBlock()
    {
        Span<char> buffer = stackalloc char[800];
        Span<char> smallbuffer = stackalloc char[16];
        var sb = new ValueStringBuilder(buffer);

        foreach (var comp in Game.Components)
            RenderImGuiComponent(comp, buffer, smallbuffer, ref sb);
    }

    internal void RenderImGuiComponent(IGameComponent comp, Span<char> buffer, Span<char> smallbuffer, ref ValueStringBuilder sb)
    {
        if (comp.GetType().IsAssignableTo(typeof(DebugImGuiViews)))
        {
            ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), "Debug View Component");
            return;
        }

        sb.Clear();
        if (exploredComponents.Add(comp) is false)
        {
            sb.Append("Component '");
            sb.Append(comp.GetType().Name);
            sb.Append("' (");
            sb.Append(comp.GetHashCode().ToStringSpan(smallbuffer));
            sb.Append(')');

            ImGui.TextColored(Color.Yellow.ToVector4().ToNumerics(), sb.AsSpan());
            return;
        }
        
        if (comp is Scene sc)
        {
            sb.Append("Scene '");
            sb.Append(sc.GetType().Name);
            sb.Append("' (");
            sb.Append(sc.GetHashCode().ToStringSpan(smallbuffer));
            sb.Append(')');
            
            if (ImGui.TreeNode(sc.GetHashCode().ToStringSpan(smallbuffer), sb.AsSpan()))
            {
                var en = sc.Enabled;
                if (ImGui.Checkbox("Enabled", ref en))
                    sc.Enabled = en;
                ImGui.SameLine();
                if (sc.Enabled)
                    ImGui.TextColored(Color.LightBlue.ToVector4().ToNumerics(), "\tUpdating");
                else
                    ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), "\tNot Updating");
                
                var vi = sc.Visible;
                if (ImGui.Checkbox("Visible", ref vi))
                    sc.Visible = vi;
                ImGui.SameLine();
                if (sc.Visible)
                    ImGui.TextColored(Color.LightBlue.ToVector4().ToNumerics(), "\tDrawing");
                else
                    ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), "\tNot Drawing");

                if (ImGui.TreeNode("More info"))
                {
                    if (sc is IDebugExplorable dsc) 
                        dsc.RenderImGuiDebug();
                    ImGui.TreePop();
                }
                
                sb.Clear();
                sb.Append(sc.SceneComponents.Count.ToStringSpan(smallbuffer));
                sb.Append(" Child Components");

                if (sc.SceneComponents.Count is > 0)
                {
                    if (ImGui.TreeNode(sc.SceneComponents.GetHashCode().ToStringSpan(smallbuffer), sb.AsSpan()))
                    {
                        foreach (var c in sc.SceneComponents)
                            RenderImGuiComponent(c, buffer, smallbuffer, ref sb);
                        ImGui.TreePop();
                    }
                }
                else
                    ImGui.Text("No child components");
                
                ImGui.TreePop();
            }
            
            return;
        }
        
        sb.Append("Component '");
        sb.Append(comp.GetType().Name);
        sb.Append("' (");
        sb.Append(comp.GetHashCode().ToStringSpan(smallbuffer));
        sb.Append(')');

        if (ImGui.TreeNode(sb.AsSpan()))
        {
            if (comp is IUpdateable up)
            {
                if (up is GameComponent gc)
                {
                    var en = gc.Enabled;
                    if (ImGui.Checkbox("Enabled", ref en))
                        gc.Enabled = en;
                    ImGui.SameLine();
                    if (gc.Enabled)
                        ImGui.TextColored(Color.LightBlue.ToVector4().ToNumerics(), "\tUpdating");
                    else
                        ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), "\tNot Updating");
                }
                else
                {
                    ImGui.LabelText("Updateable (Editing not supported)", up.Enabled ? "Enabled" : "Disabled");
                }
            }
                
            if (comp is IDrawable dr)
            {
                if (dr is DrawableGameComponent drgc)
                {
                    var vi = drgc.Visible;
                    if (ImGui.Checkbox("Visible", ref vi))
                        drgc.Visible = vi;
                    ImGui.SameLine();
                    if (drgc.Visible)
                        ImGui.TextColored(Color.LightBlue.ToVector4().ToNumerics(), "\tDrawing");
                    else
                        ImGui.TextColored(Color.Red.ToVector4().ToNumerics(), "\tNot Drawing");   
                }
                else
                {
                    ImGui.LabelText("Drawable (Editing not supported)", dr.Visible ? "Visible" : "Invisible");
                }
            }

            if (comp is IDebugExplorable dcomp && ImGui.TreeNode("More Info"))
            {
                dcomp.RenderImGuiDebug();
                ImGui.TreePop();                
            }
            
            ImGui.TreePop();
        }
    }
}
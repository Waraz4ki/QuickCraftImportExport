using System.Reflection;
using Brutal.Logging;
using HarmonyLib;



namespace QuickCraftImportExport.Core;


public class Patch(string name, Assembly patchAssembly)
{
    private string Id { get; } = name;
    public bool IsPatched { get; set; }
    private Harmony Owner { get; } = new Harmony(name);
    private Assembly PatchAssembly { get; } = patchAssembly;

    public bool TryPatch()
    {
        if (!IsPatched && Owner is not null && PatchAssembly is not null)
        {
            Owner.PatchAll(PatchAssembly);
            IsPatched = true;
            DefaultCategory.Log.Debug($"[CC] patched: {Id}");
            return true;
        }
        return false;
    }
    public bool Unpatch()
    {
        try
        {
            Owner.UnpatchAll(Id);
            IsPatched = false;
            return true;
        }
        catch (Exception ex)
        {
            DefaultCategory.Log.Critical($"[CC] failed to unpatch: {Id} | {ex.Message} -> {ex.StackTrace}");
            return false;
        }
    }

}


internal sealed class Patcher(string modId)
{
    private readonly List<Patch> _patches = [];

    public bool PatchAll()
    {
        foreach (Patch patch in _patches)
        {
            if (!patch.TryPatch()){return false;}
        }
        return true;
    }

    public void Add(string name, Assembly patchAssembly)
    {
        Patch patch1 = new($"{modId}.{name}", patchAssembly);
        patch1.TryPatch();
        _patches.Add(patch1);
    }

    public bool UnpatchAll()
    {
        foreach (Patch patch in _patches)
        {
            if (!patch.Unpatch()){return false;}
        }
        return true;
    }
}
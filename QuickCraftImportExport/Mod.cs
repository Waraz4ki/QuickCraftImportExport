using Brutal.Logging;
using KSA;
using QuickCraftImportExport.Core;
using QuickCraftImportExport.Ui;
using StarMap.API;



namespace QuickCraftImportExport;


[StarMapMod]
public class QuickCraftImportExport
{
    private readonly Patcher Patches = new(nameof(QuickCraftImportExport));

    [StarMapImmediateLoad]
    public void OnLoad(Mod definingMod)
    {
        DefaultCategory.Log.Debug("[CC] Loading Patches:");
        Patches.Add("Buttons", typeof(ImportExportButtons).Assembly);
    }
}
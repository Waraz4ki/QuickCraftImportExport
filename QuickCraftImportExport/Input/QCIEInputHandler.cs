using Brutal.Logging;
using HarmonyLib;
using KSA;
using QuickCraftImportExport.Core;
using RenderCore.Input;
using static KSA.Input;

namespace QuickCraftImportExport.Input;


//[HarmonyPatch(typeof(VehicleEditor), nameof(VehicleEditor.OnKey))]
public static class QCIEInputHandler
{



    [HarmonyPatch(typeof(VehicleEditor), nameof(VehicleEditor.OnKey))]
    [HarmonyPrefix]
    public static bool OnKey(VehicleEditor __instance, GlfwKeyEvent keyEvent)
    {
        GlfwKeyEvent glfwKeyEvent = keyEvent;

        try
        {
            DefaultCategory.Log.Info("[CC] Input");
            if (KSA.Input.MatchPressed(keyEvent, (KSA.InputAction)InputAction.Copy))
            {
                DefaultCategory.Log.Debug($"[CC] Craft copied");
                ClipboardManager.SaveXMLToClipBoard();
                return true;

            }
            if (KSA.Input.MatchPressed(keyEvent, (KSA.InputAction)InputAction.Paste))
            {
                DefaultCategory.Log.Debug($"[CC] Craft pasted");
                ClipboardManager.CopyXMLFromClipBoard();
                return true;
            }
        }
        catch (Exception ex)
        {
            DefaultCategory.Log.Critical($"[CC] Error: {ex.Message} -> {ex.StackTrace}");
        }
        return true;
    }
}
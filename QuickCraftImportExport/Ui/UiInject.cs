using System.Reflection;
using System.Reflection.Emit;
using Brutal.ImGuiApi;
using Brutal.Logging;
using HarmonyLib;
using KSA;
using QuickCraftImportExport.Core;
using RenderCore.Input;


namespace QuickCraftImportExport.Ui;
[HarmonyPatch]
public static class ImportExportButtons
{
    [HarmonyTargetMethod]
    static MethodBase Verify()
    {
        var method = AccessTools.Method(typeof(KSA.VehicleEditor), nameof(KSA.VehicleEditor.DrawLaunchUi)) ?? throw new InvalidOperationException("No OnApplicationStart found");
        DefaultCategory.Log.Debug("[CC] DrawLaunchUi found");
        return method;
    }

    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var found = false;

        var toFindMethod = AccessTools.Method(typeof(ConsoleStyle), nameof(ConsoleStyle.EndBody));

        if (toFindMethod is null){
            DefaultCategory.Log.Critical("[CC] NO SameLine Method found!");
        }

        //DefaultCategory.Log.Debug("[CC] SameLine Call found!");
        CodeInstruction toFind = new CodeInstruction(
            OpCodes.Call,
            toFindMethod
        );

        IEnumerable<CodeInstruction> toInsert = new List<CodeInstruction>() {
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ImportExportButtons), nameof(ImportExportButtons.Inject))),
        };

        for (int i = 0; i < instructions.Count(); i++){
            CodeInstruction instruction = instructions.ElementAt(i);
            //DefaultCategory.Log.Debug($"[CC] cIL: {instruction.opcode} -> {instruction.operand}");

            if (instruction.opcode == toFind.opcode && instruction.operand == toFind.operand){
                DefaultCategory.Log.Debug($"[CC] found the IL Code snippet: {toFind.operand}");

                foreach (CodeInstruction insertInstruction in toInsert){
                    yield return insertInstruction;
                }

                found = true;
            }
            yield return instruction;
        }

        if (!found)
        {
            DefaultCategory.Log.Critical("[CC] Couldnt Transpile");
        }
    }
    public static void Inject()
    {
        if (ConsoleWidgets.PrimaryButton("Copy".AsSpan())){
            ClipboardManager.SaveXMLToClipBoard();
        }
        ImGui.SameLine();
        if (ConsoleWidgets.PrimaryButton("Paste".AsSpan())){
            ClipboardManager.CopyXMLFromClipBoard();
        }
        ImGui.SameLine();
        if (ClipboardManager.clipBoardState is ClipboardManager.State.Copied){
            ImGui.Text("Copied!");
        }
        if (ClipboardManager.clipBoardState is ClipboardManager.State.Pasted){
            ImGui.Text("Pasted!");
        }
    }
}
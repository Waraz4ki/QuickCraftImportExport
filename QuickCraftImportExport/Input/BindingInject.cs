using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using Brutal.GlfwApi;
using Brutal.Logging;
using Brutal.Logging.Internal;
using Core;
using HarmonyLib;
using KSA;
using RenderCore.Input;


namespace QuickCraftImportExport.Input;


enum InputAction{
    Copy = 1267,
    Paste = 1286
}

//
//[HarmonyPatch]
//public static class BindingInject
//{
//    public static readonly Dictionary<string, BindingValue> customBindings = new Dictionary<string, BindingValue>
//    {
//        {"Copy", new BindingValue(BindingValueKind.Key, new KeyBindingValue(GlfwKey.C, GlfwModifier.Control), default(MouseButtonBindingValue))},
//        {"Paste", new BindingValue(BindingValueKind.Key, new KeyBindingValue(GlfwKey.V, GlfwModifier.Control), default(MouseButtonBindingValue))}
//    };
//
//    [HarmonyPatch(typeof(KSA.Input), "Sync")]
//    [HarmonyPrefix]
//    private static void PreSync()
//    {
//        try
//        {
//            Binding[]? a = (Binding[]?)AccessTools.Field(typeof(KSA.Input), "Bindings").GetValue(typeof(Binding[]));
//            DefaultCategory.Log.Debug($"[CC] Binding Field Recieved...");
//
//            a.AddItem(new Binding((KSA.InputAction)InputAction.Copy));
//            a.AddItem(new Binding((KSA.InputAction)InputAction.Paste));
//
//            //AccessTools.Field(typeof(KSA.Input), "Bindings").SetValue(a);
//        }
//        catch (Exception ex)
//        {
//            DefaultCategory.Log.Critical($"[CC] Error: {ex.Message} -> {ex.StackTrace}");
//        }
//    }
//
//    [HarmonyPatch(typeof(GameSettings), "OnLoaded")]
//    [HarmonyPrefix]
//    public static void PreOnLoad()
//    {
//        foreach (KeyValuePair<string, BindingValue> customBinding in customBindings)
//        {
//            GameSettings.Current.TomlKeyBindings.Add(customBinding.Key, customBinding.Value);
//        }
//    }
//}



//[HarmonyPatch]
//public static class BindingInject
//{
//    [HarmonyTargetMethod]
//    static MethodBase TargetMethod()
//    {
//        //var type = AccessTools.TypeByName("KSA.OnApplicationStart");
//        //if (type == null)
//        //    throw new InvalidOperationException("KSA.OnApplicationStart not found");
//
//        var method = AccessTools.Method(typeof(KSA.Input), nameof(KSA.Input.OnApplicationStart)) ?? throw new InvalidOperationException("No OnApplicationStart found");
//        DefaultCategory.Log.Debug("[CC] OnApplicationStart found");
//        return method;
//    }
//
//    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
//    {
//        var found = false;
//
//        var keyMethod = AccessTools.Method(typeof(KSA.Input), "BuildBindings");
//        var fiel = AccessTools.Field(typeof(KSA.Input), "DefaultAssignments");
//
//        if (fiel is null){
//            DefaultCategory.Log.Critical("[CC] NO DefaultAssignments Field found!");
//        }
//
//        DefaultCategory.Log.Debug("[CC] DefaultAssignments Field found!");
//        CodeInstruction toFind = new CodeInstruction(
//            OpCodes.Stsfld,
//            fiel
//        );
//        // IL CODE FOR DefaultAssignments = list; Define it and replace "MethodInfo mi && mi == keyMethod" with it
//        //stsfld: -> System.Collections.Generic.List`1[KSA.Input+DefaultAssignment] DefaultAssignments
//
//
//        IEnumerable<CodeInstruction> toInsert = new List<CodeInstruction>(){
//            //new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(KSA.Input), "Sync")),
//            //new CodeInstruction(OpCodes.Ldloca_S, 1),
//            new CodeInstruction(OpCodes.Ldc_I4_S, 100),
//            new CodeInstruction(OpCodes.Call, AccessTools.Method(
//                AccessTools.TypeByName("KSA.Input.DefaultAssignment"),
//                "get_item")),
//            //new CodeInstruction(OpCodes.Ldc_I4_S, 103),
//            //new CodeInstruction(OpCodes.Ldc_I4, 67),
//            //new CodeInstruction(OpCodes.Ldc_I4_0, 2),
//            //new CodeInstruction(OpCodes.Call, AccessTools.Method(
//            //    AccessTools.TypeByName("KSA.Input.DefaultAssignment"),
//            //    "Key")),
//            //new CodeInstruction(OpCodes.Stobj, AccessTools.TypeByName("KSA.Input.DefaultAssignment")),
//        };
//
//
//        for (int i = 0; i < instructions.Count(); i++){
//            CodeInstruction instruction = instructions.ElementAt(i);
//            DefaultCategory.Log.Debug($"[CC] cIL: {instruction.opcode} -> {instruction.operand}");
//
//            if (instruction.opcode == toFind.opcode && instruction.operand == toFind.operand){
//                DefaultCategory.Log.Debug($"[CC] found the IL Code snippet: {fiel}");
//                //var a = instructions.ToList();
//                //a.InsertRange(i - 1, toInsert);
//                //instructions = a;
//                foreach (CodeInstruction insertInstruction in toInsert){
//                    yield return insertInstruction;
//                }
//
//                found = true;
//                DefaultCategory.Log.Debug($"[CC] Transpiled maybe?");
//            }
//            yield return instruction;
//        }
//
//        //foreach (var instruction in instructions)
//        //{
//        //    DefaultCategory.Log.Debug($"[CC] current IL instruct: {instruction.opcode}: -> {instruction.operand}");
//        //    if (instruction.opcode == toFind.opcode && instruction.operand == toFind.operand)
//        //    {
//        //        DefaultCategory.Log.Debug($"[CC] found the IL Code snippet: {fiel}");
//        //        yield return new CodeInstruction(OpCodes.Ldloca_S, "System.Span`1[KSA.Input+DefaultAssignment] (1)");
//        //        yield return new CodeInstruction(OpCodes.Ldc_I4_S, "101");
//        //        yield return new CodeInstruction(OpCodes.Call, "DefaultAssignment& get_Item(Int32)");
//        //        yield return new CodeInstruction(OpCodes.Ldc_I4_S, "103");
//        //        yield return new CodeInstruction(OpCodes.Ldc_I4, "67");
//        //        yield return new CodeInstruction(OpCodes.Ldc_I4_0, null);
//        //        yield return new CodeInstruction(OpCodes.Call, "DefaultAssignment Key(KSA.InputAction, Brutal.GlfwApi.GlfwKey, Brutal.GlfwApi.GlfwModifier)");
//        //        yield return new CodeInstruction(OpCodes.Stobj, "KSA.Input+DefaultAssignment");
////
//        //        DefaultCategory.Log.Debug($"[CC] Transpiled maybe?");
//        //        //yield return new CodeInstruction(OpCodes.Call, typeof(BindingInject).GetMethod(nameof(Input)));
//        //        found = true;
//        //    }
////
//        //    yield return instruction;
//        //}
//        if (!found)
//        {
//            DefaultCategory.Log.Critical("[CC] Couldnt Transpile");
//        }
//    }
//    public static void Inject()
//    {
//        DefaultCategory.Log.Critical("[CC] YAYAYAYASAY");
//    }
//}
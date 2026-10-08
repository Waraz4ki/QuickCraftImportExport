using System.Text;
using Brutal.Logging;
using KSA;
using TextCopy;

namespace QuickCraftImportExport.Core;


public static class ClipboardManager
{
    public enum State{
        None,
        Copied,
        Pasted
    }

    public static State clipBoardState = State.None;

    private static string SpanToString(ReadOnlySpan<byte> span)
    {
        return Encoding.UTF8.GetString(span);
    }

    private static PartTree? ExtractPartTree()
    {
        PartTree? focusedTree = null;
        VehicleEditor? vehicleEditor = Program.Editor;
        if (vehicleEditor == null)
        {
            return null;
        }

        focusedTree = vehicleEditor.EditingSpace.Parts;
        if (focusedTree is not null)
        {
            DefaultCategory.Log.Debug($"[CC] Tree: {focusedTree.Count}");
        }
        return focusedTree;
    }

    private static ReadOnlySpan<byte> SerializeVehicleSaveData(VehicleSaveData vehicleSaveData)
    {
        MemoryStream memoryStream = new MemoryStream();
        return XmlHelper.SerializeWithoutNaN(VehicleSaves.VehicleSerializer, vehicleSaveData, memoryStream, true);
    }

    private static VehicleSaveData? DeserializeVehicleSaveData(string serializedVehicleSaveData)
    {
        VehicleSaveData? vehicleSave = null;
        using StringReader textReader = new StringReader(serializedVehicleSaveData);

        try
        {
            vehicleSave = (VehicleSaveData?)VehicleSaves.VehicleSerializer.Deserialize(textReader);
        }
        catch
        {
            return null;
        }

        if (!(vehicleSave is VehicleSaveData vehicleSaveData))
        {
            //throw new NullReferenceException("vehicle data is null");
            return null;
        }
        vehicleSaveData.OnDataLoad(Mod.Empty);
        return vehicleSaveData;
    }

    private static PartTree? DeserializePartTree(string serializedVehicleSaveData)
    {
        PartTree? partTree = null;
        VehicleSaveData? vehicleSaveData = DeserializeVehicleSaveData(serializedVehicleSaveData);
        if (vehicleSaveData == null)
        {
            return null;
        }

        if (vehicleSaveData.RootPartInstance != null)
        {
            partTree = KSA.PartTree.Deserialize(new PartTreeData
            {
                Root = vehicleSaveData.RootPartInstance,
                SequenceEnvironments = vehicleSaveData.SequenceEnvironments,
                FuelLinks = vehicleSaveData.FuelLinks
            });
        }
        return partTree;
    }

    public static void CopyXMLFromClipBoard()
    {
        try
        {
            PartTree? partTree = null;
            string? serializedVehicleSaveData = TextCopy.ClipboardService.GetText();
            if (serializedVehicleSaveData == null)
            {
                return;
            }

            partTree = DeserializePartTree(serializedVehicleSaveData);
            if (partTree == null)
            {
                return;
            }

            VehicleEditor? vehicleEditor = Program.Editor;
            if (vehicleEditor == null)
            {
                return;
            }

            vehicleEditor.LoadVehicle(partTree);
            clipBoardState = State.Pasted;
        }
        catch (Exception ex)
        {
            DefaultCategory.Log.Critical($"[CC] Error: {ex.Message} -> {ex.StackTrace}");
        }
    }

    public static void SaveXMLToClipBoard()
    {
        try
        {
            PartTree? partTree = ExtractPartTree();
            if (partTree == null)
            {
                return;
            }

            VehicleSaveData vehicleSaveData = VehicleSaveData.Create("temp", partTree);
            ReadOnlySpan<byte> bytes = SerializeVehicleSaveData(vehicleSaveData);
            TextCopy.ClipboardService.SetText(SpanToString(bytes));
            clipBoardState = State.Copied;
        }
        catch (Exception ex)
        {
            DefaultCategory.Log.Critical($"[CC] Error: {ex.Message} -> {ex.StackTrace}");
        }
    }
}
using UnityEditor;

public static class Reserializer
{
    [MenuItem("Tools/Reserialize assets")]
    public static void ReserializeAssets()
    {
        EditorUtility.DisplayProgressBar("Reserializing...", null, 0);
        AssetDatabase.ForceReserializeAssets();
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
    }
}

using UnityEngine;
using UnityEditor;

public class RenameSprites : EditorWindow
{
    [MenuItem("Tools/Rename Sprites After Last Underscore")]
    static void RenameSelectedSprites()
    {
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null && importer.textureType == TextureImporterType.Sprite)
            {
                // Lấy các sprite phụ trong texture (nếu là Sprite sheet)
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object asset in assets)
                {
                    if (asset is Sprite)
                    {
                        string oldName = asset.name;
                        int lastUnderscore = oldName.LastIndexOf('_');
                        if (lastUnderscore >= 0 && lastUnderscore < oldName.Length - 1)
                        {
                            string newName = oldName.Substring(lastUnderscore + 1);
                            asset.name = newName;
                            EditorUtility.SetDirty(asset);
                        }
                    }
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        Debug.Log("Đã đổi tên xong các sprite được chọn.");
    }
}

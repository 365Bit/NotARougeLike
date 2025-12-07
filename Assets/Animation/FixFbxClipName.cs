using UnityEditor;
using UnityEngine;
using System.IO;

public class FixFbxClipName
{
    [MenuItem("Tools/Animation/Fix All FBX Clip Names")]
    static void FixAllFbxClipNames()
    {
        // 查找所有 Model（包含 FBX）
        string[] guids = AssetDatabase.FindAssets("t:Model");
        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // 只处理 .fbx 文件（可按需去掉这个判断）
            if (!path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                continue;

            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
                continue;

            // 获取当前的动画 Clip 列表
            ModelImporterClipAnimation[] clips = importer.clipAnimations;

            // 如果 clipAnimations 为空，有可能在用 defaultClipAnimations
            if (clips == null || clips.Length == 0)
            {
                clips = importer.defaultClipAnimations;
                if (clips == null || clips.Length == 0)
                    continue; // 没有动画，跳过
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            bool changed = false;

            for (int i = 0; i < clips.Length; i++)
            {
                // 只有当名字不一样时才改，避免重复导入时无意义 Reimport
                if (clips[i].name != fileName)
                {
                    clips[i].name = fileName;
                    changed = true;
                }
            }

            if (changed)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                count++;
                Debug.Log($"[FixFbxClipName] Renamed clips in: {path} -> {fileName}");
            }
        }

        Debug.Log($"[FixFbxClipName] 完成，处理了 {count} 个 FBX。");
    }

    // 如果你只想处理选中的 FBX，可以用这个菜单
    [MenuItem("Tools/Animation/Fix Selected FBX Clip Names")]
    static void FixSelectedFbxClipNames()
    {
        var objs = Selection.objects;
        int count = 0;

        foreach (var obj in objs)
        {
            string path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                continue;

            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
                continue;

            ModelImporterClipAnimation[] clips = importer.clipAnimations;
            if (clips == null || clips.Length == 0)
            {
                clips = importer.defaultClipAnimations;
                if (clips == null || clips.Length == 0)
                    continue;
            }

            string fileName = Path.GetFileNameWithoutExtension(path);
            bool changed = false;

            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].name != fileName)
                {
                    clips[i].name = fileName;
                    changed = true;
                }
            }

            if (changed)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                count++;
                Debug.Log($"[FixFbxClipName] Renamed clips in: {path} -> {fileName}");
            }
        }

        Debug.Log($"[FixFbxClipName] 完成，处理了 {count} 个选中的 FBX。");
    }
}

using System.IO;
using UnityEngine;

namespace Game.Scripts.Help
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class PrefabSaver : MonoBehaviour
    {
        public static void SaveAsPrefab(GameObject obj, string path)
        {
#if UNITY_EDITOR
            // Убедимся, что папка существует
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string prefabPath = $"{path}/{obj.name}.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(obj, prefabPath, InteractionMode.UserAction);
            Debug.Log($"Prefab saved to {prefabPath}");
#else
        Debug.LogError("Prefab saving is only supported in the Unity Editor.");
#endif
        }
    }
}

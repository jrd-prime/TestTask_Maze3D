using System.IO;
using UnityEngine;

namespace Game.Scripts.Help
{
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class MeshSaver : MonoBehaviour
    {
        public static void SaveMesh(Mesh mesh, string name, string path)
        {
#if UNITY_EDITOR
            // Убедимся, что папка существует
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Путь к файлу
            string assetPath = $"{path}/{name}.asset";

            // Сохраняем меш в файл
            AssetDatabase.CreateAsset(mesh, assetPath);
            AssetDatabase.SaveAssets();
            Debug.Log($"Mesh saved to {assetPath}");
#else
        Debug.LogError("Mesh saving is only supported in the Unity Editor.");
#endif
        }
    }
}

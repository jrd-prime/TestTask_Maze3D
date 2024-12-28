using UnityEngine;
using System.Collections.Generic;

public class GridGenerator
{
    private readonly Camera _camera;
    private readonly GameObject _tilePrefab; // Префаб тайла (например, плоскость 2x2)
    private readonly GameObject _holePrefab; // Префаб ямы
    private readonly GameObject _wallPrefab;
    private readonly int _tilesPerGridX = 32; // Количество тайлов по оси X
    private readonly int _tilesPerGridY = 18; // Количество тайлов по оси Y
    private readonly Transform _parent;

    private List<Vector2> _holes; // Список для хранения координат ям

    public GridGenerator(Camera camera, GameObject tilePrefab, GameObject holePrefab, GameObject wallPrefab,
        int tilesPerGridX,
        int tilesPerGridY,
        Transform parent = null)
    {
        _tilePrefab = tilePrefab;
        _holePrefab = holePrefab;
        _wallPrefab = wallPrefab;
        _tilesPerGridX = tilesPerGridX * 2;
        _tilesPerGridY = tilesPerGridY * 2;
        _camera = camera;
        _parent = parent;

        _holes = new List<Vector2>();
    }

  public void GenerateGrid()
{
    _holes.Add(new Vector2(16, 12)); // Пример ямы

    float tileSize = 4.0f;
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    List<GameObject> allCreatedObjects = new List<GameObject>(); // Список для хранения всех созданных объектов
    for (int x = 0; x < _tilesPerGridX; x++)
    {
        for (int y = 0; y < _tilesPerGridY; y++)
        {
            // Генерация стен по краям карты
            if (x == 0 || x == _tilesPerGridX - 1 || y == 0 || y == _tilesPerGridY - 1)
            {
                // Генерация стены
                Vector3 wallPosition = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject wall = Object.Instantiate(_wallPrefab, wallPosition, Quaternion.identity, _parent);

                // Добавляем MeshFilter для каждого дочернего объекта в префабе стены
                MeshFilter[] wallMeshFilters = wall.GetComponentsInChildren<MeshFilter>();
                foreach (var mFilter in wallMeshFilters)
                {
                    meshFilters.Add(mFilter);
                }

                allCreatedObjects.Add(wall); // Добавляем объект стены в список
                continue; // Пропускаем создание тайла или ямы в этих ячейках
            }

            if (_holes.Contains(new Vector2(x, y)))
            {
                // Генерация ям
                Vector3 holePosition = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject hole = Object.Instantiate(_holePrefab, holePosition, Quaternion.identity, _parent);

                // Добавляем MeshFilter для каждого дочернего объекта в префабе ямы
                MeshFilter[] holeMeshFilters = hole.GetComponentsInChildren<MeshFilter>();
                foreach (var mFilter in holeMeshFilters)
                {
                    meshFilters.Add(mFilter);
                }

                allCreatedObjects.Add(hole); // Добавляем объект в список
                continue; // Пропускаем создание тайла в этих ячейках
            }

            // Генерация тайлов
            float offsetX = x * tileSize;
            float offsetY = y * tileSize;

            Vector3 position = new Vector3(offsetX, 0, offsetY);

            GameObject tile = Object.Instantiate(_tilePrefab, position, Quaternion.Euler(90, 0, 0), _parent);
            tile.transform.localScale = new Vector3(tileSize, tileSize, 1);
            MeshFilter meshFilter = tile.GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                meshFilters.Add(meshFilter);
            }

            allCreatedObjects.Add(tile); // Добавляем объект в список
        }
    }

    // Создаем комбинированный меш
    CombineMeshes(meshFilters);

    // Удаляем все объекты, кроме комбинированного
    foreach (var obj in allCreatedObjects)
    {
        Object.Destroy(obj); // Удаляем объект
    }
}


// Метод для объединения всех мешей
    private void CombineMeshes(List<MeshFilter> meshFilters)
    {
        if (meshFilters.Count == 0)
            return;

        CombineInstance[] combineInstances = new CombineInstance[meshFilters.Count];
        int i = 0;

        foreach (var meshFilter in meshFilters)
        {
            combineInstances[i].mesh = meshFilter.sharedMesh;
            combineInstances[i].transform = meshFilter.transform.localToWorldMatrix;
            i++;
        }

        GameObject combinedObject = new GameObject("CombinedMesh");
        combinedObject.transform.SetParent(_parent);

        MeshFilter combinedMeshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances);
        combinedMeshFilter.mesh = combinedMesh;

        combinedMesh.Optimize();
        meshRenderer.material = _tilePrefab.GetComponent<MeshRenderer>().sharedMaterial;
    }
}

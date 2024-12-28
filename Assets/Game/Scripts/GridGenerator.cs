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
        _holes.Add(new Vector2(5, 16));
        _holes.Add(new Vector2(7, 6));
        _holes.Add(new Vector2(10, 7));
        _holes.Add(new Vector2(14, 12));
        _holes.Add(new Vector2(22, 13));

        float tileSize = 4.0f;
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        List<GameObject> allCreatedObjects = new List<GameObject>();

        for (int x = 0; x < _tilesPerGridX; x++)
        {
            for (int y = 0; y < _tilesPerGridY; y++)
            {
                // Обработка стен
                if (x == 0 || x == _tilesPerGridX - 1 || y == 0 || y == _tilesPerGridY - 1)
                {
                    Vector3 wallPosition = new Vector3(x * tileSize, 0, y * tileSize);
                    GameObject wall = Object.Instantiate(_wallPrefab, wallPosition, Quaternion.identity, _parent);
                    MeshFilter[] wallMeshFilters = wall.GetComponentsInChildren<MeshFilter>();
                    foreach (var mFilter in wallMeshFilters)
                    {
                        meshFilters.Add(mFilter);
                    }

                    allCreatedObjects.Add(wall);
                    continue;
                }

                // Обработка ям
                if (_holes.Contains(new Vector2(x, y)))
                {
                    Vector3 holePosition = new Vector3(x * tileSize, 0, y * tileSize);
                    GameObject hole = Object.Instantiate(_holePrefab, holePosition, Quaternion.identity, _parent);

                    // Добавляем BoxCollider для каждой ямы
                    BoxCollider boxCollider = hole.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(tileSize, 1f, tileSize);
                    boxCollider.center = new Vector3(0, 0.5f, 0);

                    MeshFilter[] holeMeshFilters = hole.GetComponentsInChildren<MeshFilter>();
                    foreach (var mFilter in holeMeshFilters)
                    {
                        meshFilters.Add(mFilter);
                    }

                    allCreatedObjects.Add(hole);

                    continue;
                }

                // Генерация тайлов
                Vector3 position = new Vector3(x * tileSize, 0, y * tileSize);
                GameObject tile = Object.Instantiate(_tilePrefab, position, Quaternion.Euler(90, 0, 0), _parent);
                tile.transform.localScale = new Vector3(tileSize, tileSize, 1);
                MeshFilter meshFilter = tile.GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    meshFilters.Add(meshFilter);
                }

                allCreatedObjects.Add(tile);
            }
        }

        // Комбинирование сеток
        CombineMeshesSeparately(meshFilters);

        // Удаление временных объектов
        foreach (var obj in allCreatedObjects)
        {
            Object.Destroy(obj);
        }
    }


    private void CombineMeshesSeparately(List<MeshFilter> meshFilters)
    {
        List<CombineInstance> floorCombines = new List<CombineInstance>();
        List<CombineInstance> holeCombines = new List<CombineInstance>();
        List<CombineInstance> wallCombines = new List<CombineInstance>();

        foreach (var meshFilter in meshFilters)
        {
            Vector3 position = meshFilter.transform.position / 4.0f; // Tile size = 4
            Vector2Int tileCoords = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

            // Фильтрация объектов по тегу
            string tag = meshFilter.gameObject.tag;

            if (tag == "Hole")
            {
                // Обработка ям
                holeCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
            else if (tag == "Wall")
            {
                // Обработка стен
                wallCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
            else if (tag == "Floor" && position.y == 0)
            {
                // Обработка пола
                floorCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
        }

        // Создание комбинированных объектов
        CreateCombinedObject(floorCombines, "CombinedFloor", GetSharedMaterial(_tilePrefab));
        CreateCombinedObject(holeCombines, "CombinedHoles", GetSharedMaterial(_holePrefab));
        CreateCombinedObject(wallCombines, "CombinedWalls", GetSharedMaterial(_wallPrefab));
    }


    private Material GetSharedMaterial(GameObject prefab)
    {
        // Получаем материал из всех дочерних MeshRenderer
        MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();
        return renderers.Length > 0 ? renderers[0].sharedMaterial : null;
    }


    private void CreateCombinedObject(List<CombineInstance> combines, string name, Material material)
    {
        if (combines.Count == 0) return;

        GameObject combinedObject = new GameObject(name) { isStatic = true };
        combinedObject.transform.SetParent(_parent);

        MeshFilter meshFilter = combinedObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combines.ToArray(), true, true);
        meshFilter.mesh = combinedMesh;

        meshRenderer.material = material;

        // Добавляем коллайдер только для пола и стен
        if (name == "CombinedFloor")
        {
            MeshCollider collider = combinedObject.AddComponent<MeshCollider>();
            collider.sharedMesh = CreateMeshWithHoles(combinedMesh, _holes);
            collider.convex = false;
        }
        else if (name == "CombinedWalls")
        {
            MeshCollider collider = combinedObject.AddComponent<MeshCollider>();
            collider.sharedMesh = combinedMesh;
        }

        // Добавление BoxCollider только для CombinedHoles
        if (name == "CombinedHoles")
        {
            BoxCollider boxCollider = combinedObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true; // Включаем триггер

            // Устанавливаем размер коллайдера, примерный расчет для всей области
            boxCollider.size = new Vector3(4.0f * _tilesPerGridX, 1.0f, 4.0f * _tilesPerGridY);
            boxCollider.center = new Vector3((_tilesPerGridX * 4.0f) / 2, -2.5f, (_tilesPerGridY * 4.0f) / 2);
        }

        combinedObject.layer = LayerMask.NameToLayer("GroundAndWalls");
    }

    private Mesh CreateMeshWithHoles(Mesh floorMesh, List<Vector2> holes)
    {
        Mesh newMesh = new Mesh();

        Vector3[] vertices = floorMesh.vertices;
        int[] triangles = floorMesh.triangles;

        List<int> updatedTriangles = new List<int>();

        foreach (int i in triangles)
        {
            Vector3 vertex = vertices[i];

            // Округляем координаты до ближайшего целого числа, чтобы точно совпадать с координатами ям
            Vector2 tileCoords = new Vector2(Mathf.RoundToInt(vertex.x / 4), Mathf.RoundToInt(vertex.z / 4));

            if (!holes.Exists(h => Vector2.Distance(h, tileCoords) < 0.1f))
            {
                updatedTriangles.Add(i);
            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = updatedTriangles.ToArray();
        newMesh.RecalculateNormals();

        return newMesh;
    }
}

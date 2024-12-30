using UnityEngine;
using System.Collections.Generic;
using Game.Scripts.Help;

public class GridGenerator
{
    private readonly Camera _camera;
    private readonly GameObject _tilePrefab;
    private readonly GameObject _holePrefab;
    private readonly GameObject _wallPrefab;
    private readonly int _tilesPerGridX = 32;
    private readonly int _tilesPerGridY = 18;
    private readonly Transform _parent;

    private List<Vector2> _holes;

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

        var tileSize = 4.0f;
        List<MeshFilter> meshFilters = new();
        List<GameObject> allCreatedObjects = new();

        for (var x = 0; x < _tilesPerGridX; x++)
        {
            for (var y = 0; y < _tilesPerGridY; y++)
            {
                if (x == 0 || x == _tilesPerGridX - 1 || y == 0 || y == _tilesPerGridY - 1)
                {
                    Vector3 wallPosition = new(x * tileSize, 0, y * tileSize);
                    var wall = Object.Instantiate(_wallPrefab, wallPosition, Quaternion.identity, _parent);
                    var wallMeshFilters = wall.GetComponentsInChildren<MeshFilter>();
                    foreach (var mFilter in wallMeshFilters)
                    {
                        meshFilters.Add(mFilter);
                    }

                    allCreatedObjects.Add(wall);
                    continue;
                }

                if (_holes.Contains(new Vector2(x, y)))
                {
                    Vector3 holePosition = new(x * tileSize, 0, y * tileSize);
                    var hole = Object.Instantiate(_holePrefab, holePosition, Quaternion.identity, _parent);

                    var boxCollider = hole.AddComponent<BoxCollider>();
                    boxCollider.size = new Vector3(tileSize, 1f, tileSize);
                    boxCollider.center = new Vector3(0, 0.5f, 0);

                    var holeMeshFilters = hole.GetComponentsInChildren<MeshFilter>();
                    foreach (var mFilter in holeMeshFilters)
                    {
                        meshFilters.Add(mFilter);
                    }

                    allCreatedObjects.Add(hole);

                    continue;
                }

                Vector3 position = new(x * tileSize, 0, y * tileSize);
                var tile = Object.Instantiate(_tilePrefab, position, Quaternion.Euler(90, 0, 0), _parent);
                tile.transform.localScale = new Vector3(tileSize, tileSize, 1);
                var meshFilter = tile.GetComponent<MeshFilter>();

                if (meshFilter != null)
                {
                    meshFilters.Add(meshFilter);
                }

                allCreatedObjects.Add(tile);
            }
        }

        CombineMeshesSeparately(meshFilters);

        foreach (var obj in allCreatedObjects)
        {
            Object.Destroy(obj);
        }
    }


    private void CombineMeshesSeparately(List<MeshFilter> meshFilters)
    {
        List<CombineInstance> floorCombines = new();
        List<CombineInstance> holeCombines = new();
        List<CombineInstance> wallCombines = new();

        foreach (var meshFilter in meshFilters)
        {
            var position = meshFilter.transform.position / 4.0f;
            Vector2Int tileCoords = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));

            string tag = meshFilter.gameObject.tag;

            if (tag == "Hole")
            {
                holeCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
            else if (tag == "Wall")
            {
                wallCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
            else if (tag == "Floor" && position.y == 0)
            {
                floorCombines.Add(new CombineInstance
                {
                    mesh = meshFilter.sharedMesh,
                    transform = meshFilter.transform.localToWorldMatrix
                });
            }
        }

        CreateCombinedObject(floorCombines, "CombinedFloor", GetSharedMaterial(_tilePrefab));
        CreateCombinedObject(holeCombines, "CombinedHoles", GetSharedMaterial(_holePrefab));
        CreateCombinedObject(wallCombines, "CombinedWalls", GetSharedMaterial(_wallPrefab));
    }


    private Material GetSharedMaterial(GameObject prefab)
    {
        MeshRenderer[] renderers = prefab.GetComponentsInChildren<MeshRenderer>();
        return renderers.Length > 0 ? renderers[0].sharedMaterial : null;
    }


    private void CreateCombinedObject(List<CombineInstance> combines, string name, Material material)
    {
        if (combines.Count == 0) return;

        GameObject combinedObject = new(name) { isStatic = true };
        combinedObject.transform.SetParent(_parent);

        var meshFilter = combinedObject.AddComponent<MeshFilter>();
        var meshRenderer = combinedObject.AddComponent<MeshRenderer>();

        Mesh combinedMesh = new();
        combinedMesh.CombineMeshes(combines.ToArray(), true, true);

        MeshSaver.SaveMesh(combinedMesh, name + "_CombinedMesh_save", "Assets/Generated/Meshes");

        meshFilter.mesh = combinedMesh;

        meshRenderer.material = material;

        if (name == "CombinedFloor")
        {
            var collider = combinedObject.AddComponent<MeshCollider>();
            collider.sharedMesh = CreateMeshWithHoles(combinedMesh, _holes);
            collider.convex = true;

            combinedObject.layer = LayerMask.NameToLayer("Floor");
        }
        else if (name == "CombinedWalls")
        {
            var collider = combinedObject.AddComponent<MeshCollider>();
            collider.sharedMesh = combinedMesh;
            collider.convex = true;

            combinedObject.layer = LayerMask.NameToLayer("Walls");
        }

        if (name == "CombinedHoles")
        {
            // var boxCollider = combinedObject.AddComponent<BoxCollider>();
            // boxCollider.isTrigger = true;
            //
            // boxCollider.size = new Vector3(4.0f * _tilesPerGridX, 1.0f, 4.0f * _tilesPerGridY);
            // boxCollider.center = new Vector3((_tilesPerGridX * 4.0f) / 2, -2.5f, (_tilesPerGridY * 4.0f) / 2);

            combinedObject.layer = LayerMask.NameToLayer("Holes");
        }

        PrefabSaver.SaveAsPrefab(combinedObject, "Assets/Generated/Prefabs");
    }

    private Mesh CreateMeshWithHoles(Mesh floorMesh, List<Vector2> holes)
    {
        Mesh newMesh = new();

        var vertices = floorMesh.vertices;
        var triangles = floorMesh.triangles;

        List<int> updatedTriangles = new();

        foreach (var i in triangles)
        {
            Vector3 vertex = vertices[i];

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

using UnityEngine;

namespace Game.Scripts
{
    public class GroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject wallPrefab;

        private void Start()
        {
            var gridGen = new GridGenerator(Camera.main, tilePrefab, holePrefab, wallPrefab, 16, 9);
            gridGen.GenerateGrid();
        }
    }
}

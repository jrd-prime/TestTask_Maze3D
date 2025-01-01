using System;
using UnityEngine;

namespace Game.Scripts.Server
{
    public class GroundManager : MonoBehaviour
    {
        [SerializeField] private GameObject floorCombined;
        [SerializeField] private GameObject holesCombined;
        [SerializeField] private GameObject wallsCombined;

        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject holePrefab;
        [SerializeField] private GameObject wallPrefab;

        private void Start()
        {
            if (floorCombined == null || holesCombined == null || wallsCombined == null)
            {
                Debug.LogWarning("Combined objects not set! Generating...");
                var gridGen = new GridGenerator(Camera.main, tilePrefab, holePrefab, wallPrefab, 16, 9);
                gridGen.GenerateGrid();

                throw new Exception("You need to set generated combined objects!");
            }
            // Instantiate(floorCombined, transform);
            // Instantiate(holesCombined, transform);
            // Instantiate(wallsCombined, transform);
        }
    }
}

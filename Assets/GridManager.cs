using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 8;
    [SerializeField] private Object reference;
    [SerializeField] private float tileSize = 1;

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        var referenceTile = Instantiate(reference) as GameObject;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var tile = (GameObject)Instantiate(referenceTile, transform);
                var posX = col * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
            }
        }
        Destroy(referenceTile);

        var gridWidth = cols * tileSize;
        var gridHeight = rows * tileSize;
        transform.position = new Vector2(-gridWidth / 2 + tileSize / 2, gridHeight / 2 - tileSize / 2);
    }
}

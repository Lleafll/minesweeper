using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum Tile
{
    Default,
    Mine
}

public class GridManager : MonoBehaviour
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 8;
    [SerializeField] private Object defaultReference;
    [SerializeField] private Object mineReference;
    [SerializeField] private float tileSize = 1;
    [SerializeField] private int mineCount = 10;
    private List<Tile> tiles;

    void Start()
    {
        tiles = GenerateTiles(rows * columns, mineCount);
        GenerateGrid();
    }

    private static List<Tile> GenerateTiles(int length, int mineCount)
    {
        var tiles = Enumerable.Repeat(Tile.Default, length).ToList();
        var minesRemaining = mineCount;
        while (minesRemaining != 0)
        {
            var index = Random.Range(0, length);
            if (tiles[index] != Tile.Mine)
            {
                tiles[index] = Tile.Mine;
                --minesRemaining;
            }
        }
        return tiles;
    }

    private void GenerateGrid()
    {
        var defaultReferenceTile = Instantiate(defaultReference) as GameObject;
        var mineReferenceTile = Instantiate(mineReference) as GameObject;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = Instantiate(tileAt(row, column) == Tile.Default ? defaultReferenceTile : mineReferenceTile, transform) as GameObject;
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
            }
        }
        Destroy(defaultReferenceTile);
        Destroy(mineReferenceTile);

        var gridWidth = columns * tileSize;
        var gridHeight = rows * tileSize;
        transform.position = new Vector2(-gridWidth / 2 + tileSize / 2, gridHeight / 2 - tileSize / 2);
    }

    private Tile tileAt(int row, int column)
    {
        return tiles[columns * row + column];
    }
}

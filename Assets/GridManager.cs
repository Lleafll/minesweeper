using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private enum Tile
    {
        Default,
        Mine
    }

    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 8;
    [SerializeField] private Object defaultReference;
    [SerializeField] private Object mineReference;
    [SerializeField] private float tileSize = 1;
    private List<Tile> tiles;

    void Start()
    {
        tiles = GenerateTiles(rows * columns);
        GenerateGrid();
    }

    private static List<Tile> GenerateTiles(int length)
    {
        var tiles = new List<Tile>();
        for (int i = 0; i < length; i++)
        {
            if (i % 2 == 0)
            {
                tiles.Add(Tile.Default);
            }
            else
            {
                tiles.Add(Tile.Mine);
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

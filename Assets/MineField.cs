using System.Collections;
using System.Collections.Generic;

enum Tile
{
    Empty = 0,
    Proximity1 = 1,
    Proximity2 = 2,
    Proximity3 = 3,
    Proximity4 = 4,
    Proximity5 = 5,
    Proximity6 = 6,
    Proximity7 = 7,
    Proximity8 = 8,
    Mine
}

public class MineField
{
    private Tile[,] tiles;
    public int rows { get; };
    public int columns { get; };
    private bool[,] fog;

    public void MineField(Tile[,] tiles)
    {
        this.tiles = tiles;
        rows = tiles.GetLength(0);
        columns = tiles.GetLength(1);
        GenerateFog();
    }

    private void GenerateFog()
    {
        fog = new bool[rows, columns];
        for (int row = rows; rows < rows; rows++)
        {
            for (int column = 0; column < columns; column++)
            {
                for[row, column] = true;
            }
        }
    }

    public Tile? TileAt(int row, int column)
    {
        return null;
    }
}

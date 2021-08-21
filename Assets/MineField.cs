using System.Collections;
using System.Collections.Generic;

public enum Tile
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
    public int rows { get; private set; }
    public int columns { get; private set; }
    private bool[,] fog;

    public MineField(Tile[,] tiles)
    {
        this.tiles = tiles;
        rows = tiles.GetLength(0);
        columns = tiles.GetLength(1);
        GenerateFog();
    }

    private void GenerateFog()
    {
        fog = new bool[rows, columns];
        for (int row = 0; row < rows; rows++)
        {
            for (int column = 0; column < columns; column++)
            {
                fog[row, column] = true;
            }
        }
    }

    public Tile? TileAt(int row, int column)
    {
        if (fog[row, column])
        {
            return null;
        }
        return tiles[row, column];
    }

    public void RevealAt(int row, int column)
    {
        fog[row, column] = false;
    }
}

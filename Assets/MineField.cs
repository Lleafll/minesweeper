using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Mine,
    Fog,
    Flag
}

public class MineField
{
    private Tile[,] tiles;
    public int rows { get; private set; }
    public int columns { get; private set; }
    private bool[,] fog;
    private bool[,] flags;

    public MineField(bool[,] mines)
    {
        rows = mines.GetLength(0);
        columns = mines.GetLength(1);
        GenerateTiles(mines);
        GenerateFog();
        GenerateFlags();
    }

    public static MineField GenerateRandom(int rows, int columns, int mineCount)
    {
        var mines = new bool[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                mines[row, column] = false;
            }
        }
        while (mineCount != 0)
        {
            var index = Random.Range(0, rows * columns);
            var row = index / columns;
            var column = index % columns;
            if (!mines[row, column])
            {
                mines[row, column] = true;
                --mineCount;
            }
        }
        return new MineField(mines);
    }

    private void GenerateTiles(bool[,] mines)
    {
        tiles = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (mines[row, column])
                {
                    tiles[row, column] = Tile.Mine;
                }
                else
                {
                    tiles[row, column] = Tile.Empty;
                }
            }
        }
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (tiles[row, column] == Tile.Mine)
                {
                    continue;
                }
                var minesInProximity = 0;
                for (int x = row - 1; x <= row + 1; x++)
                {
                    if (x < 0 || x >= rows)
                    {
                        continue;
                    }
                    for (int y = column - 1; y <= column + 1; y++)
                    {
                        if (y < 0 || y >= columns)
                        {
                            continue;
                        }
                        if (tiles[x, y] == Tile.Mine)
                        {
                            minesInProximity++;
                        }
                    }
                }
                tiles[row, column] = (Tile)minesInProximity;
            }
        }
    }

    private void GenerateFog()
    {
        fog = new bool[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                fog[row, column] = true;
            }
        }
    }

    private void GenerateFlags()
    {
        flags = new bool[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                flags[row, column] = false;
            }
        }
    }

    public Tile TileAt(int row, int column)
    {
        if (flags[row, column])
        {
            return Tile.Flag;
        }
        if (fog[row, column])
        {
            return Tile.Fog;
        }
        return tiles[row, column];
    }

    public void RevealAt(int row, int column)
    {
        if (flags[row, column])
        {
            return;
        }
        fog[row, column] = false;
    }

    public void SetFlag(int row, int column)
    {
        if (!fog[row, column])
        {
            return;
        }
        flags[row, column] = !flags[row, column];
    }
}

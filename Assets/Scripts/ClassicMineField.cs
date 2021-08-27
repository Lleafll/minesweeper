using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tile
{
    Empty = -1,
    Proximity0 = 0,
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

public enum GameStatus
{
    Running,
    Won,
    Lost
}

public class ClassicMineField
{
    private Tile[,] tiles;
    public int rows { get; private set; }
    public int columns { get; private set; }
    private bool[,] fog;
    private bool[,] flags;
    private ITileGenerator generator;

    public ClassicMineField(bool[,] mines, bool staticTiles = true)
    {
        rows = mines.GetLength(0);
        columns = mines.GetLength(1);
        GenerateFog();
        GenerateFlags();
        if (staticTiles)
        {
            generator = new StaticTileGenerator(mines);
        }
        else
        {
            generator = new DynamicTileGenerator(mines, flags);
        }
        GenerateTiles();
    }

    public static ClassicMineField GenerateRandom(int rows, int columns, int mineCount, bool staticTiles)
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
        return new ClassicMineField(mines, staticTiles);
    }

    private void GenerateTiles()
    {
        tiles = generator.Generate();
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

    public bool RevealAt(int row, int column)
    {
        if (flags[row, column])
        {
            return false;
        }
        if (fog[row, column])
        {
            return ClearFog(row, column);
        }
        else
        {
            if (FlagsInProximity(row, column) == (int)tiles[row, column])
            {
                return ClearAround(row, column);
            }
        }
        return false;
    }

    private bool ClearFog(int row, int column)
    {
        if (flags[row, column])
        {
            return false;
        }
        if (fog[row, column])
        {
            fog[row, column] = false;
            ClearAutomatically();
            return true;
        }
        return false;
    }


    private int FlagsInProximity(int row, int column)
    {
        int flagsInProximity = 0;
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
                if (flags[x, y])
                {
                    flagsInProximity++;
                }
            }
        }
        return flagsInProximity;
    }

    private bool ClearAround(int row, int column)
    {
        var clearedSomething = false;
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
                clearedSomething |= ClearFog(x, y);
            }
        }
        return clearedSomething;
    }

    private void ClearAutomatically()
    {
        var changedSomething = false;
        do
        {
            changedSomething = false;
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (!fog[row, column] && tiles[row, column] == Tile.Empty)
                    {
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
                                if (x == row && y == column)
                                {
                                    continue;
                                }
                                if (fog[x, y] && !flags[x, y])
                                {
                                    fog[x, y] = false;
                                    changedSomething = true;
                                }
                            }
                        }
                    }
                }
            }
        } while (changedSomething);
    }

    public bool SetFlag(int row, int column)
    {
        if (fog[row, column])
        {
            flags[row, column] = !flags[row, column];
            GenerateTiles();
            return true;
        }
        else
        {
            return RevealAt(row, column);
        }
    }

    public GameStatus CheckGameStatus()
    {
        if (IsMineHit())
        {
            return GameStatus.Lost;
        }
        if (AreAllNonMinesRevealed())
        {
            return GameStatus.Won;
        }
        return GameStatus.Running;
    }

    private bool IsMineHit()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (!fog[row, column] && tiles[row, column] == Tile.Mine)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool AreAllNonMinesRevealed()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (fog[row, column] && tiles[row, column] != Tile.Mine)
                {
                    return false;
                }
            }
        }
        return true;
    }
}

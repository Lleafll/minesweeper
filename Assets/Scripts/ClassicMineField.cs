using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum Tile
{
    Inaccessible = -2,
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

    public ClassicMineField(Tile[,] mines, bool staticTiles = true)
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

    public static ClassicMineField GenerateRandom(int rows, int columns, int mineCount, bool staticTiles, bool rectangular)
    {
        if (rectangular)
        {
            return GenerateRandomRectangular(rows, columns, mineCount, staticTiles);
        }
        else
        {
            return GenerateRandomIrregular(rows, columns, mineCount, staticTiles);
        }
    }

    public static ClassicMineField GenerateRandomRectangular(int rows, int columns, int mineCount, bool staticTiles)
    {
        var mines = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                mines[row, column] = Tile.Empty;
            }
        }
        PopulateWithMines(mines, mineCount);
        return new ClassicMineField(mines, staticTiles);
    }

    public static ClassicMineField GenerateRandomIrregular(int rows, int columns, int mineCount, bool staticTiles)
    {
        var width = UnityEngine.Random.Range(rows / 2, rows);
        var start = UnityEngine.Random.Range(0, rows / 2);
        var mines = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            width = Math.Min(Math.Max(width, columns / 2 - 1), columns);
            start = Math.Min(Math.Max(start, 0), columns / 2 - 1);
            for (int column = 0; column < start; column++)
            {
                mines[row, column] = Tile.Inaccessible;
            }
            var end = Math.Min(start + width, columns);
            for (int column = start; column < end; column++)
            {
                mines[row, column] = Tile.Empty;
            }
            for (int column = end; column < columns; column++)
            {
                mines[rows, column] = Tile.Inaccessible;
            }
        }
        PopulateWithMines(mines, mineCount);
        return new ClassicMineField(mines, staticTiles);
    }

    private static void PopulateWithMines(Tile[,] mines, int mineCount)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        while (mineCount != 0)
        {
            var index = UnityEngine.Random.Range(0, rows * columns);
            var row = index / columns;
            var column = index % columns;
            if (mines[row, column] == Tile.Empty)
            {
                mines[row, column] = Tile.Mine;
                --mineCount;
            }
        }
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
            if (FlagsInProximity(row, column) == generator.MinesInProximity(row, column))
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
            ClearAutomatically(row, column);
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

    private void ClearAutomatically(int initialRow, int initialColumn)
    {
        var positions = new Queue<(int, int)>();
        positions.Enqueue((initialRow, initialColumn));
        while (positions.Count != 0)
        {
            var (row, column) = positions.Dequeue();
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
                            positions.Enqueue((x, y));
                        }
                    }
                }
            }
        }
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

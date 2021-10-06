#nullable enable 

using System.Collections.Generic;
using System;

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
    private readonly Tile[,] mines;
    private Tile[,] tiles;
    private bool[,] fog;
    private readonly bool[,] flags;
    private readonly ITileGenerator generator;
    private readonly int mineCount;

    public ClassicMineField(Tile[,] mines, bool staticTiles = true)
    {
        this.mines = mines;
        mineCount = CountMines(mines);
        flags = GenerateFlags();
        if (staticTiles)
        {
            generator = new StaticTileGenerator(mines);
        }
        else
        {
            generator = new DynamicTileGenerator(mines, flags);
        }
        tiles = GenerateTiles();
        fog = GenerateFog();
    }

    private static int CountMines(Tile[,] mineField)
    {
        var rows = mineField.GetLength(0);
        var columns = mineField.GetLength(1);
        var count = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (mineField[row, column] == Tile.Mine)
                {
                    ++count;
                }
            }
        }
        return count;
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
        var mines = new Tile[rows, columns];
        {
            var width = UnityEngine.Random.Range(columns / 2, columns);
            var width_start = UnityEngine.Random.Range(0, columns / 2);
            for (int row = 0; row < rows; row++)
            {
                width_start = Math.Min(Math.Max(width_start, 0), columns / 2 - 1);
                width = Math.Min(Math.Max(width, columns / 2 - 1), columns - width_start);
                for (int column = 0; column < width_start; column++)
                {
                    mines[row, column] = Tile.Inaccessible;
                }
                var width_end = Math.Min(width_start + width, columns);
                for (int column = width_start; column < width_end; column++)
                {
                    mines[row, column] = Tile.Empty;
                }
                for (int column = width_end; column < columns; column++)
                {
                    mines[row, column] = Tile.Inaccessible;
                }
                width -= UnityEngine.Random.Range(-1, 2);
                width_start -= UnityEngine.Random.Range(-1, 2);
            }
        }
        {
            var height = UnityEngine.Random.Range(rows / 2, rows);
            var height_start = UnityEngine.Random.Range(0, rows / 2);
            for (int column = 0; column < columns; column++)
            {
                height_start = Math.Min(Math.Max(height_start, 0), rows / 2 - 1);
                height = Math.Min(Math.Max(height, rows / 2 - 1), rows - height_start);
                for (int row = 0; row < height_start; row++)
                {
                    mines[row, column] = Tile.Inaccessible;
                }
                var height_end = Math.Min(height_start + height, rows);
                for (int row = height_end; row < rows; row++)
                {
                    mines[row, column] = Tile.Inaccessible;
                }
                height -= UnityEngine.Random.Range(-1, 2);
                height_start -= UnityEngine.Random.Range(-1, 2);
            }
        }
        RemoveOneTileIslands(mines);
        PopulateWithMines(mines, mineCount);
        mines = CullInaccessibleRowsAndColumns(mines);
        return new ClassicMineField(mines, staticTiles);
    }

    private static Tile[,] CullInaccessibleRowsAndColumns(Tile[,] mines)
    {
        var rowBegin = GetAccessibleRowBegin(mines);
        var rowEnd = GetAccessibleRowEnd(mines, rowBegin);
        var columnBegin = GetAccessibleColumnBegin(mines);
        var columnEnd = GetAccessibleColumnEnd(mines, columnBegin);
        var newRows = rowEnd - rowBegin;
        var newColumns = columnEnd - columnBegin;
        var newMines = new Tile[newRows, newColumns];
        for (var row = 0; row < newRows; ++row)
        {
            for (var column = 0; column < newColumns; ++column)
            {
                newMines[row, column] = mines[row + rowBegin, column + columnBegin];
            }
        }
        return newMines;
    }

    private static int GetAccessibleRowBegin(Tile[,] mines)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                if (mines[row, column] != Tile.Inaccessible)
                {
                    return row;
                }
            }
        }
        return 0;
    }

    private static int GetAccessibleRowEnd(Tile[,] mines, int begin)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        for (var row = begin; row < rows; row++)
        {
            var hasAccessible = false;
            for (var column = 0; column < columns; column++)
            {
                if (mines[row, column] != Tile.Inaccessible)
                {
                    hasAccessible = true;
                    break;
                }
            }
            if (!hasAccessible)
            {
                return row;
            }
        }
        return rows;
    }

    private static int GetAccessibleColumnBegin(Tile[,] mines)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        for (var column = 0; column < columns; column++)
        {
            for (var row = 0; row < rows; row++)
            {
                if (mines[row, column] != Tile.Inaccessible)
                {
                    return column;
                }
            }
        }
        return 0;
    }

    private static int GetAccessibleColumnEnd(Tile[,] mines, int begin)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        for (var column = begin; column < columns; column++)
        {
            var hasAccessible = false;
            for (var row = 0; row < rows; row++)
            {
                if (mines[row, column] != Tile.Inaccessible)
                {
                    hasAccessible = true;
                    break;
                }
            }
            if (!hasAccessible)
            {
                return column;
            }
        }
        return columns;
    }

    private static void RemoveOneTileIslands(Tile[,] mineField)
    {
        var rows = mineField.GetLength(0);
        var columns = mineField.GetLength(1);
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                if (mineField[row, column] != Tile.Inaccessible && !HasNeighbor(mineField, row, column))
                {
                    mineField[row, column] = Tile.Inaccessible;
                }
            }
        }
    }

    private static bool HasNeighbor(Tile[,] mineField, int row, int column)
    {
        var rows = mineField.GetLength(0);
        var columns = mineField.GetLength(1);
        var rowBegin = Math.Max(0, row - 1);
        var rowEnd = Math.Min(row + 2, rows);
        var columnBegin = Math.Max(0, column - 1);
        var columnEnd = Math.Min(column + 2, columns);
        for (var i = rowBegin; i < rowEnd; i++)
        {
            for (var k = columnBegin; k < columnEnd; k++)
            {
                if (i == row && k == column)
                {
                    continue;
                }
                if (mineField[i, k] != Tile.Inaccessible)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void RepopulateWithMinesRandomly()
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (mines[row, column] != Tile.Inaccessible)
                {
                    mines[row, column] = Tile.Empty;
                }
            }
        }
        PopulateWithMines(mines, mineCount);
        tiles = GenerateTiles();
        fog = GenerateFog();
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

    private Tile[,] GenerateTiles()
    {
        return generator.Generate();
    }

    private bool[,] GenerateFog()
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        var fog = new bool[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                fog[row, column] = tiles[row, column] != Tile.Inaccessible;
            }
        }
        return fog;
    }

    private bool[,] GenerateFlags()
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        var flags = new bool[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                flags[row, column] = false;
            }
        }
        return flags;
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
        if (tiles[row, column] == Tile.Inaccessible)
        {
            return false;
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
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
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
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
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
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
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
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
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
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
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

    public int RemainingMines()
    {
        return mineCount - FlagCount(flags);
    }

    private static int FlagCount(bool[,] flags)
    {
        var rows = flags.GetLength(0);
        var columns = flags.GetLength(1);
        var flagCount = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (flags[row, column])
                {
                    ++flagCount;
                }
            }
        }
        return flagCount;
    }

    public (int, int) Length()
    {
        return (mines.GetLength(0), mines.GetLength(1));
    }
}

public class DynamicTileGenerator : ITileGenerator
{
    private Tile[,] mines;
    private bool[,] flags;

    public DynamicTileGenerator(Tile[,] mines, bool[,] flags)
    {
        this.mines = mines;
        this.flags = flags;
    }

    public Tile[,] Generate()
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        var tiles = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (mines[row, column] == Tile.Mine)
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
                if (mines[row, column] == Tile.Mine)
                {
                    IncrementAround(tiles, row, column);
                }
                if (flags[row, column])
                {
                    DecrementAround(tiles, row, column);
                }
            }
        }
        return tiles;
    }

    public int MinesInProximity(int row, int column)
    {
        var rows = mines.GetLength(0);
        var columns = mines.GetLength(1);
        int minesInProximity = 0;
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
                if (mines[x, y] == Tile.Mine)
                {
                    minesInProximity++;
                }
            }
        }
        return minesInProximity;
    }

    static private void IncrementAround(Tile[,] tiles, int row, int column)
    {
        var rows = tiles.GetLength(0);
        var columns = tiles.GetLength(1);
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
                tiles[x, y] = Increment(tiles[x, y]);
            }
        }
    }

    static private Tile Increment(Tile tile)
    {
        if (tile == Tile.Mine)
        {
            return tile;
        }
        if (tile == Tile.Empty)
        {
            return Tile.Proximity1;
        }
        return (Tile)(((int)tile) + 1);
    }

    static private void DecrementAround(Tile[,] tiles, int row, int column)
    {
        var rows = tiles.GetLength(0);
        var columns = tiles.GetLength(1);
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
                tiles[x, y] = Decrement(tiles[x, y]);
            }
        }
    }

    static private Tile Decrement(Tile tile)
    {
        if (tile == Tile.Mine)
        {
            return tile;
        }
        if (tile == Tile.Empty)
        {
            return tile;
        }
        return (Tile)(((int)tile) - 1);
    }
}

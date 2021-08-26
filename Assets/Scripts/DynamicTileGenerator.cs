public class DynamicTileGenerator : ITileGenerator
{
    private bool[,] mines;
    private bool[,] flags;

    public DynamicTileGenerator(bool[,] mines, bool[,] flags)
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
                if (mines[row, column])
                {
                    IncrementAround(tiles, row, column);
                }
            }
        }
        return tiles;
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
        return (Tile)(((int)tile) + 1);
    }
}

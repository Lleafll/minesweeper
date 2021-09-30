public class StaticTileGenerator : ITileGenerator
{
    private Tile[,] mines;

    public StaticTileGenerator(Tile[,] mines)
    {
        this.mines = mines;
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
                tiles[row, column] = minesInProximity == 0 ? Tile.Empty : (Tile)minesInProximity;
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
}

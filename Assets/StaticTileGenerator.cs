public class StaticTileGenerator : ITileGenerator
{
    private bool[,] mines;

    public StaticTileGenerator(bool[,] mines)
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
        return tiles;
    }
}

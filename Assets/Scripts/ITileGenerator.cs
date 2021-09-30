public interface ITileGenerator
{
    public Tile[,] Generate();

    public int MinesInProximity(int row, int column);
}

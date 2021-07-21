using UnityEngine;
using UnityEngine.UI;

enum Tile
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

public class MineFieldManager : MonoBehaviour
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int columns = 8;
    [SerializeField] private Object defaultReference;
    [SerializeField] private Object emptyReference;
    [SerializeField] private Object mineReference;
    [SerializeField] private Object Proximity1Reference;
    [SerializeField] private Object Proximity2Reference;
    [SerializeField] private Object Proximity3Reference;
    [SerializeField] private Object Proximity4Reference;
    [SerializeField] private Object Proximity5Reference;
    [SerializeField] private Object Proximity6Reference;
    [SerializeField] private Object Proximity7Reference;
    [SerializeField] private Object Proximity8Reference;
    [SerializeField] private float tileSize = 1;
    [SerializeField] private int mineCount = 10;
    private Tile[,] tiles;
    private GameObject[,] fog;
    bool gameOver = false;

    void Start()
    {
        tiles = GenerateTiles(rows, columns, mineCount);
        GenerateGrid();
        fog = GenerateFog();
        var gridWidth = columns * tileSize;
        var gridHeight = rows * tileSize;
        Camera.main.transform.Translate(new Vector3(gridWidth / 2 - tileSize / 2, -gridHeight / 2 + tileSize / 2, 0));
    }

    private static Tile[,] GenerateTiles(int rows, int columns, int mineCount)
    {
        var tiles = new Tile[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                tiles[row, column] = Tile.Empty;
            }
        }
        var minesRemaining = mineCount;
        while (minesRemaining != 0)
        {
            var index = Random.Range(0, rows * columns);
            var row = index / columns;
            var column = index % columns;
            if (tiles[row, column] != Tile.Mine)
            {
                tiles[row, column] = Tile.Mine;
                --minesRemaining;
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

    private void GenerateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = InstantiateTile(tileAt(row, column));
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
            }
        }
    }

    private GameObject[,] GenerateFog()
    {
        var tiles = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = Instantiate(defaultReference, transform) as GameObject;
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector3(posX, posY, -1);
                tiles[row, column] = tile;
            }
        }
        return tiles;
    }

    private GameObject InstantiateTile(Tile tile)
    {
        switch (tile)
        {
            case Tile.Empty:
                return Instantiate(emptyReference, transform) as GameObject;
            case Tile.Mine:
                return Instantiate(mineReference, transform) as GameObject;
            case Tile.Proximity1:
                return Instantiate(Proximity1Reference, transform) as GameObject;
            case Tile.Proximity2:
                return Instantiate(Proximity2Reference, transform) as GameObject;
            case Tile.Proximity3:
                return Instantiate(Proximity3Reference, transform) as GameObject;
            case Tile.Proximity4:
                return Instantiate(Proximity4Reference, transform) as GameObject;
            case Tile.Proximity5:
                return Instantiate(Proximity5Reference, transform) as GameObject;
            case Tile.Proximity6:
                return Instantiate(Proximity6Reference, transform) as GameObject;
            case Tile.Proximity7:
                return Instantiate(Proximity7Reference, transform) as GameObject;
            case Tile.Proximity8:
                return Instantiate(Proximity8Reference, transform) as GameObject;
        }
        throw new System.InvalidOperationException("Tile case not handled");
    }

    private Tile tileAt(int row, int column)
    {
        return tiles[row, column];
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var row = (int)(mousePosition.y / -tileSize + tileSize / 2);
            var column = (int)(mousePosition.x / tileSize + tileSize / 2);
            if (row < 0 || column < 0 || row >= rows || column >= columns)
            {
                return;
            }
            clearFog(row, column);
            gameOver = checkIfMineHit(row, column);
            if (gameOver)
            {
                return;
            }
            clearAutomatically();
        }
    }

    void clearFog(int row, int column)
    {
        fog[row, column].SetActive(false);
    }

    bool checkIfMineHit(int row, int column)
    {
        if (tiles[row, column] == Tile.Mine)
        {
            var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
            gameOverText.enabled = true;
            return true;
        }
        return false;
    }

    void clearAutomatically()
    {
        var noChanges = true;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (!fog[row, column].activeSelf && tiles[row, column] == Tile.Empty)
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
                            if (fog[x, y].activeSelf)
                            {
                                fog[x, y].SetActive(false);
                                noChanges = false;
                            }
                        }
                    }
                }
            }
        }
        if (!noChanges)
        {
            clearAutomatically();
        }
    }
}

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
    [SerializeField] private int rows = 10;
    [SerializeField] private int columns = 10;
    [SerializeField] private Object defaultReference;
    [SerializeField] private Object emptyReference;
    [SerializeField] private Object mineReference;
    [SerializeField] private Object flagReference;
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
    private GameObject[,] grid;
    private GameObject[,] fog;
    private GameObject[,] flags;
    private bool gameOver = false;
    private bool firstHit = true;
    private bool flagButtonMode = true;

    void Start()
    {
        Reset();
        var gridWidth = columns * tileSize;
        var gridHeight = rows * tileSize;
        Camera.main.transform.Translate(new Vector3(gridWidth / 2 - tileSize / 2, -gridHeight / 2 + tileSize / 2, 0));
    }

    public void Reset()
    {
        mineCount = System.Math.Min(mineCount, rows * columns / 2);
        tiles = GenerateTiles(rows, columns, mineCount);
        GenerateGrid();
        GenerateFog();
        GenerateFlags();
        gameOver = false;
        var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
        gameOverText.enabled = false;
        var gameWonText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameWonText.enabled = false;
        firstHit = true;
    }

    public void SetMineCount(string value)
    {
        mineCount = int.Parse(value);
    }

    public void SetRows(string value)
    {
        rows = int.Parse(value);
    }

    public void SetColumns(string value)
    {
        columns = int.Parse(value);
    }

    public void setFlagButtonMode(bool value)
    {
        flagButtonMode = value;
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
        if (grid != null)
        {
            foreach (var g in grid)
            {
                Destroy(g);
            }
        }
        grid = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = InstantiateTile(tileAt(row, column));
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
                grid[row, column] = tile;
            }
        }
    }

    private void GenerateFog()
    {
        if (fog != null)
        {
            foreach (var f in fog)
            {
                Destroy(f);
            }
        }
        fog = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = Instantiate(defaultReference, transform) as GameObject;
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector3(posX, posY, -1);
                fog[row, column] = tile;
            }
        }
    }

    private void GenerateFlags()
    {
        if (flags != null)
        {
            foreach (var flag in flags)
            {
                Destroy(flag);
            }
        }
        flags = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = Instantiate(flagReference, transform) as GameObject;
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector3(posX, posY, -2);
                tile.SetActive(false);
                flags[row, column] = tile;
            }
        }
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
        else if (Input.GetMouseButtonDown(0))
        {
            var (row, column) = getClickedRowAndColumn();
            if (row < 0 || column < 0 || row >= rows || column >= columns)
            {
                return;
            }
            else if (IsHiddenByFog(row, column))
            {
                if (firstHit)
                {
                    gameOver = clearFog(row, column);
                    firstHit = false;
                }
                else
                {
                    if (flagButtonMode)
                    {
                        flags[row, column].SetActive(!flags[row, column].activeSelf);
                    }
                    else
                    {
                        clearFog(row, column);
                    }
                }
            }
            else
            {
                if (FlagsInProximityMatchTile(row, column))
                {
                    gameOver = clearAround(row, column);
                }
            }
            if (gameOver)
            {
                return;
            }
            clearAutomatically();
            gameOver = checkIfWon();
        }
    }

    private bool IsHiddenByFog(int row, int column)
    {
        return fog[row, column].activeSelf;
    }

    private (int, int) getClickedRowAndColumn()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var row = (int)System.Math.Round(mousePosition.y / -tileSize + tileSize / 2 - 0.5);
        var column = (int)System.Math.Round(mousePosition.x / tileSize + tileSize / 2 - 0.5);
        return (row, column);
    }

    private bool FlagsInProximityMatchTile(int row, int column)
    {
        return FlagsInProximity(row, column) == (int)tiles[row, column];
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
                if (IsProtectedByFlag(x, y))
                {
                    flagsInProximity++;
                }
            }
        }
        return flagsInProximity;
    }

    private bool clearAround(int row, int column)
    {
        bool mineHit = false;
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
                if (!IsProtectedByFlag(x, y) && IsHiddenByFog(x, y))
                {
                    var mineUnderFog = clearFog(x, y);
                    if (mineUnderFog)
                    {
                        mineHit = true;
                    }
                }
            }
        }
        return mineHit;
    }

    private bool IsProtectedByFlag(int row, int column)
    {
        return flags[row, column].activeSelf;
    }

    private bool clearFog(int row, int column)
    {
        fog[row, column].SetActive(false);
        return checkIfMineHit(row, column);
    }

    private bool checkIfMineHit(int row, int column)
    {
        if (tiles[row, column] == Tile.Mine)
        {
            var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
            gameOverText.enabled = true;
            return true;
        }
        return false;
    }

    private void clearAutomatically()
    {
        var noChanges = true;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (!IsHiddenByFog(row, column) && tiles[row, column] == Tile.Empty)
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
                            if (IsHiddenByFog(x, y))
                            {
                                clearFog(x, y);
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

    private bool checkIfWon()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (tiles[row, column] == Tile.Mine)
                {
                    continue;
                }
                if (IsHiddenByFog(row, column))
                {
                    return false;
                }
            }
        }
        var gameOverText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameOverText.enabled = true;
        return true;
    }
}

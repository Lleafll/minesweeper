using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private float longPressDurationInSeconds = 0.5F;
    private MineField field;
    private GameObject[,] grid;
    private bool gameOver = false;
    private bool flagButtonMode = true;
    private bool isButtonDown = false;
    private float buttonDownDuration = 0;
    private bool isFirstReveal = true;

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
        field = MineField.GenerateRandom(rows, columns, mineCount);
        GenerateGrid();
        gameOver = false;
        var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
        gameOverText.enabled = false;
        var gameWonText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameWonText.enabled = false;
        isFirstReveal = true;
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
                var tile = InstantiateTile(field.TileAt(row, column));
                var posX = column * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
                grid[row, column] = tile;
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
            case Tile.Fog:
                return Instantiate(defaultReference, transform) as GameObject;
            case Tile.Flag:
                return Instantiate(flagReference, transform) as GameObject;
        }
        throw new System.InvalidOperationException("Tile case not handled");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            buttonDownDuration = 0;
            isButtonDown = true;
        }
        else if (isButtonDown && Input.GetMouseButton(0))
        {
            buttonDownDuration += Time.deltaTime;
            if (buttonDownDuration >= longPressDurationInSeconds)
            {
                ExecuteClick(false);
            }
        }
        else if (isButtonDown && Input.GetMouseButtonUp(0))
        {
            ExecuteClick(true);
        }
    }

    void ExecuteClick(bool directClick)
    {
        isButtonDown = false;
        var (row, column) = getClickedRowAndColumn();
        if (row < 0 || column < 0 || row >= rows || column >= columns)
        {
            return;
        }
        if (flagButtonMode == directClick)
        {
            field.SetFlag(row, column);
        }
        else
        {
            field.RevealAt(row, column);
            if (isFirstReveal)
            {
                if (field.CheckGameStatus() == GameStatus.Lost)
                {
                    Reset();
                    ExecuteClick(directClick);
                }
                isFirstReveal = false;
            }
        }
        GenerateGrid();
        gameOver = CheckIfGameOver();
    }

    private (int, int) getClickedRowAndColumn()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var row = (int)System.Math.Round(mousePosition.y / -tileSize + tileSize / 2 - 0.5);
        var column = (int)System.Math.Round(mousePosition.x / tileSize + tileSize / 2 - 0.5);
        return (row, column);
    }

    private bool CheckIfGameOver()
    {
        var gameStatus = field.CheckGameStatus();
        switch (gameStatus)
        {
            case GameStatus.Won:
                ShowGameWonScreen();
                break;
            case GameStatus.Lost:
                ShowGameLostScreen();
                break;
        }
        return gameStatus != GameStatus.Running;
    }

    private void ShowGameWonScreen()
    {
        var gameOverText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameOverText.enabled = true;
    }

    private void ShowGameLostScreen()
    {
        var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
        gameOverText.enabled = true;
    }
}

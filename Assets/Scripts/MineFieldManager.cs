#nullable enable

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System;

public class MineFieldManager : MonoBehaviour
{
    public int rows { get; private set; }
    public int columns { get; private set; }
    [SerializeField] private TileBase? defaultReference;
    [SerializeField] private TileBase? emptyReference;
    [SerializeField] private TileBase? mineReference;
    [SerializeField] private TileBase? flagReference;
    [SerializeField] private TileBase? Proximity0Reference;
    [SerializeField] private TileBase? Proximity1Reference;
    [SerializeField] private TileBase? Proximity2Reference;
    [SerializeField] private TileBase? Proximity3Reference;
    [SerializeField] private TileBase? Proximity4Reference;
    [SerializeField] private TileBase? Proximity5Reference;
    [SerializeField] private TileBase? Proximity6Reference;
    [SerializeField] private TileBase? Proximity7Reference;
    [SerializeField] private TileBase? Proximity8Reference;
    private int mineCount = 10;
    [SerializeField] private long vibrationDurationInMs = 20;
    private ClassicMineField? field;
    private bool gameOver = false;
    private bool flagButtonMode = true;
    private bool isFirstReveal = true;
    [SerializeField] private PlayerSettings? settings;
    [SerializeField] private Button? tryAgainButton;
    [SerializeField] private Tilemap? tileMap;
    [SerializeField] private bool staticTiles = false;
    [SerializeField] private bool rectangular = true;
    [SerializeField] private int maxCameraSize = 50;

    void Start()
    {
        if (settings == null)
        {
            throw new InvalidOperationException("settings not initialized");
        }
        Vibration.Init();
        rows = settings.GetRowCount();
        columns = settings.GetColumnCount();
        mineCount = settings.GetMineCount();
        rectangular = !settings.GetIrregularMineField();
        Reset();
        CenterCamera();
    }

    private void CenterCamera()
    {
        var gridWidth = columns;
        var gridHeight = rows;
        Camera.main.transform.Translate(new Vector3(gridWidth / 2 - 0.5f, -gridHeight / 2 + 0.5f, 0));
    }

    public void Reset()
    {
        if (tryAgainButton == null)
        {
            throw new InvalidOperationException("tryAgainButton not initialized");
        }
        mineCount = System.Math.Min(mineCount, rows * columns / 2);
        field = ClassicMineField.GenerateRandom(rows, columns, mineCount, staticTiles, rectangular);
        GenerateGrid();
        ZoomOut();
        gameOver = false;
        var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
        gameOverText.enabled = false;
        var gameWonText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameWonText.enabled = false;
        tryAgainButton.gameObject.SetActive(false);
        isFirstReveal = true;
    }

    public void SetFlagButtonMode(bool value)
    {
        flagButtonMode = value;
    }

    private void GenerateGrid()
    {
        if (field == null)
        {
            throw new InvalidOperationException("field not initialized");
        }
        if (tileMap == null)
        {
            throw new InvalidOperationException("tileMap not initialized");
        }
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                var tile = InstantiateTile(field.TileAt(row, column));
                tileMap.SetTile(new Vector3Int(column, -row, 0), tile);
            }
        }
    }

    private void ZoomOut()
    {
        Camera.main.orthographicSize = rows * 0.5F * 1.1F;
        var screenAspect = (float)Screen.width / (float)Screen.height;
        var camHalfHeight = Camera.main.orthographicSize;
        var camHalfWidth = screenAspect * camHalfHeight;
        var camWidth = 2.0f * camHalfWidth;
        var necessaryWidth = columns * 1.1F;
        if (camWidth < necessaryWidth)
        {
            Camera.main.orthographicSize *= necessaryWidth / camWidth;
        }
        Camera.main.orthographicSize = System.Math.Min(maxCameraSize, Camera.main.orthographicSize);
    }

    private TileBase? InstantiateTile(Tile tile)
    {
        switch (tile)
        {
            case Tile.Inaccessible:
                return null;
            case Tile.Empty:
                return emptyReference;
            case Tile.Mine:
                return mineReference;
            case Tile.Proximity0:
                return Proximity0Reference;
            case Tile.Proximity1:
                return Proximity1Reference;
            case Tile.Proximity2:
                return Proximity2Reference;
            case Tile.Proximity3:
                return Proximity3Reference;
            case Tile.Proximity4:
                return Proximity4Reference;
            case Tile.Proximity5:
                return Proximity5Reference;
            case Tile.Proximity6:
                return Proximity6Reference;
            case Tile.Proximity7:
                return Proximity7Reference;
            case Tile.Proximity8:
                return Proximity8Reference;
            case Tile.Fog:
                return defaultReference;
            case Tile.Flag:
                return flagReference;
        }
        throw new System.InvalidOperationException("Tile case not handled");
    }

    public void ExecuteClick(bool directClick)
    {
        if (field == null)
        {
            throw new InvalidOperationException("field not initialized");
        }
        if (gameOver)
        {
            return;
        }
        var (row, column) = GetClickedRowAndColumn();
        if (row < 0 || column < 0 || row >= rows || column >= columns)
        {
            return;
        }
        var clicked = false;
        if (flagButtonMode == directClick)
        {
            clicked = field.SetFlag(row, column);
        }
        else
        {
            clicked = field.RevealAt(row, column);
            if (isFirstReveal)
            {
                if (field.CheckGameStatus() == GameStatus.Lost)
                {
                    field.RepopulateWithMinesRandomly();
                    ExecuteClick(directClick);
                }
                isFirstReveal = false;
            }
        }
        if (clicked)
        {
            Vibration.Vibrate(vibrationDurationInMs);
        }
        GenerateGrid();
        gameOver = CheckIfGameOver();
    }

    private (int, int) GetClickedRowAndColumn()
    {
        var mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var row = (int)System.Math.Round(-mousePosition.y);
        var column = (int)System.Math.Round(mousePosition.x);
        return (row, column);
    }

    private bool CheckIfGameOver()
    {
        if (field == null)
        {
            throw new InvalidOperationException("field not initialized");
        }
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
        if (tryAgainButton == null)
        {
            throw new InvalidOperationException("tryAgainButton not initialized");
        }
        var gameOverText = GameObject.Find("GameWonScreen").GetComponent<Text>();
        gameOverText.enabled = true;
        tryAgainButton.gameObject.SetActive(true);
    }

    private void ShowGameLostScreen()
    {
        if (tryAgainButton == null)
        {
            throw new InvalidOperationException("tryAgainButton not initialized");
        }
        var gameOverText = GameObject.Find("GameOverScreen").GetComponent<Text>();
        gameOverText.enabled = true;
        tryAgainButton.gameObject.SetActive(true);
    }
}

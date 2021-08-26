using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    private const int defaultMineCount = 30;
    private const int defaultRowCount = 15;
    private const int defaultColumnCount = 10;
    private const string mineCountKey = "mineCount";
    private const string rowCountKey = "rowCount";
    private const string columnCountKey = "columnCount";

    public void SetMineCount(string mineCount)
    {
        PlayerPrefs.SetInt(mineCountKey, int.Parse(mineCount));
    }

    public int GetMineCount()
    {
        return PlayerPrefs.GetInt(mineCountKey, defaultMineCount);
    }

    public void SetRowCount(string rowCount)
    {
        PlayerPrefs.SetInt(rowCountKey, int.Parse(rowCount));
    }

    public int GetRowCount()
    {
        return PlayerPrefs.GetInt(rowCountKey, defaultRowCount);
    }

    public void SetColumnCount(string columnCount)
    {
        PlayerPrefs.SetInt(columnCountKey, int.Parse(columnCount));
    }

    public int GetColumnCount()
    {
        return PlayerPrefs.GetInt(columnCountKey, defaultColumnCount);
    }
}

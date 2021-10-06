using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    private const int defaultMineCount = 30;
    private const int defaultRowCount = 15;
    private const int defaultColumnCount = 10;
    private const int defaultIrregularMineField = 0;
    private const int defaultIrregularSize = 25;
    private const string mineCountKey = "mineCount";
    private const string rowCountKey = "rowCount";
    private const string columnCountKey = "columnCount";
    private const string irregularMineFieldKey = "irregularMineField";
    private const string irregularSizeKey = "irregularSize";

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

    public void SetIrregularMineField(bool irregularMineField)
    {
        PlayerPrefs.SetInt(irregularMineFieldKey, irregularMineField ? 1 : 0);
    }

    public bool GetIrregularMineField()
    {
        return PlayerPrefs.GetInt(irregularMineFieldKey, defaultIrregularMineField) == 1;
    }

    public void SetIrregularSize(string irregularSize)
    {
        PlayerPrefs.SetInt(irregularSizeKey, int.Parse(irregularSize));
    }

    public int GetIrregularSize()
    {
        return PlayerPrefs.GetInt(irregularSizeKey, defaultIrregularSize);
    }
}

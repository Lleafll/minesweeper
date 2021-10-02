using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPlayerSettings : MonoBehaviour
{
    [SerializeField] private PlayerSettings settings;
    [SerializeField] private InputField mineCount;
    [SerializeField] private InputField rows;
    [SerializeField] private InputField columns;
    [SerializeField] private Toggle irregularMineField;

    // Start is called before the first frame update
    void Start()
    {
        mineCount.text = settings.GetMineCount().ToString();
        rows.text = settings.GetRowCount().ToString();
        columns.text = settings.GetColumnCount().ToString();
        irregularMineField.isOn = settings.GetIrregularMineField();
    }
}

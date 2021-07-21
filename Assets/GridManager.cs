using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int rows = 5;
    [SerializeField] private int cols = 8;
    [SerializeField] private Object reference;
    private float tileSize = 1;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        var referenceTile = Instantiate(reference) as GameObject;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var tile = (GameObject)Instantiate(referenceTile, transform);
                var posX = col * tileSize;
                var posY = row * -tileSize;
                tile.transform.position = new Vector2(posX, posY);
            }
        }
        Destroy(referenceTile);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

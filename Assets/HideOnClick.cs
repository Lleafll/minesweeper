using UnityEngine;

public class HideOnClick : MonoBehaviour
{
    private bool mouseOver = false;

    void OnMouseOver()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!mouseOver)
        {
            return;
        }
        if (Input.GetMouseButton(0))
        {
            gameObject.SetActive(false);
        }
    }
}

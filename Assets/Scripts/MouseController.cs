using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public GameObject food;

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            mouseDown();
        }
    }

    private void mouseDown()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition, Camera.MonoOrStereoscopicEye.Mono);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.tag != "Food")
            {
                GameObject newFood = Instantiate(food, mousePos2D, Quaternion.Euler(Vector3.zero));
            }
        }
        else
        {
            GameObject newFood = Instantiate(food, mousePos2D, Quaternion.Euler(Vector3.zero));
        }
    }
}

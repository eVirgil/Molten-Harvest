using UnityEngine;
using System.Collections;

public class MagmaSliderController : MonoBehaviour
{
    private Vector3 distance;
    private float posX, posY;

    void OnMouseDown()
    {
        distance = Camera.main.WorldToScreenPoint(transform.position);
        posX = Input.mousePosition.x - distance.x;
        posY = Input.mousePosition.y - distance.y;
    }

    void OnMouseDrag()
    {
        Vector3 currentPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance.z);
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(currentPosition);
        transform.position = worldPosition;
    }
}

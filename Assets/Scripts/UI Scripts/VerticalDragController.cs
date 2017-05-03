using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VerticalDragController : MonoBehaviour
{

    public void OnDrag()
    {
        //transform.position = Input.mousePosition;
        transform.position = new Vector3(transform.position.x, Input.mousePosition.y, transform.position.y);
    }

}

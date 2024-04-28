using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Camera cam;
    float sizeY;
    float sizeX;
    float ratio;
    public BoxCollider2D camBox;


    // Update is called once per frame
    void Update()
    {
        sizeY = cam.orthographicSize * 3;
        ratio = (float)Screen.width / (float)Screen.height;
        sizeX = sizeY * ratio;
        camBox.size = new Vector2 (sizeX, sizeY);
    }
   
}

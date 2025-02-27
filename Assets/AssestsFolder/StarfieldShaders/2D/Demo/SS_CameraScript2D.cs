using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SS_CameraScript2D : MonoBehaviour
{    
    public float scrollSpeed = 5;

    void Update()
    {
        transform.Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * scrollSpeed * Time.deltaTime);
    }
}

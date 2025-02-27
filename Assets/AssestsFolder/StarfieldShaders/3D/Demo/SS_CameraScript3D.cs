using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SS_CameraScript3D : MonoBehaviour
{
    public float movementSpeed = 10;
    public float rotationSpeed = 10;

    private Vector2 prevMousePos;

    void Start()
    {
        CreateUI();    
    }

    void Update()
    {
        MovementController();
        RotationController();
    }

    private void MovementController()
    {
        transform.Translate(
            new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical") ) * Time.deltaTime * movementSpeed * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1),
            Space.Self
        );
    }

    private void RotationController()
    {
        if (!Input.GetKey(KeyCode.Mouse1))
            return;
            
        Vector2 mousePos = Input.mousePosition;
        mousePos = new Vector2(-mousePos.y, mousePos.x);

        if (Input.GetKeyDown(KeyCode.Mouse1))
            prevMousePos = mousePos;

        transform.Rotate((mousePos - prevMousePos) * rotationSpeed * Time.deltaTime, Space.Self);
        prevMousePos = mousePos;
    }

    private void CreateUI()
    {
        GameObject UIparent = new GameObject(GetType() + "_UI");
        UIparent.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        UIparent.AddComponent<CanvasScaler>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(UIparent.transform);

        Text textComp = textObj.AddComponent<Text>();
        textComp.text = "Movement controls : WASD / Arrow Keys (Left Shift to speed-up)\nRotation controls: Right Mouse Button";
        textComp.font = Font.CreateDynamicFontFromOSFont("Arial", 14);

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.one;
    }
}

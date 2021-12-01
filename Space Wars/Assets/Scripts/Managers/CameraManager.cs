using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public Vector2 GetScreenSize()
    {
        return new Vector2(Camera.main.aspect * Camera.main.orthographicSize * 2.0f, Camera.main.orthographicSize * 2.0f);
    }
}

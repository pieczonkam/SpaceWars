using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;        
    }

    public static Vector2 GetScreenSize()
    {
        return new Vector2(mainCamera.aspect * mainCamera.orthographicSize * 2.0f, mainCamera.orthographicSize * 2.0f);
    }
}

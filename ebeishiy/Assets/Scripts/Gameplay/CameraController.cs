using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    PlayerInputs inputs;
    private Player player;
    [Header("CameraAndPlayerDirections")]
    [SerializeField] private float sensitivity;
    [SerializeField] private Transform cameraDir;
    private float x, y;
    public Transform orientation;
    [HideInInspector] public bool moveCamera;

    private void Awake()
    {
        player = GetComponent<Player>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inputs = player.inputs;

        moveCamera = true;
    }

    private void Update()
    {   
        if (moveCamera)
        {
            CameraMovement(inputs.Main.Camera.ReadValue<Vector2>());
        }
    }

    private void CameraMovement(Vector2 cv)
    {
        x += cv.y * Time.deltaTime * sensitivity;
        y += cv.x * Time.deltaTime * sensitivity;

        x = Mathf.Clamp(x, -60, 60);

        cameraDir.rotation = Quaternion.Euler(-x, y, 0);
        orientation.rotation = Quaternion.Euler(0, y, 0);
    }
}

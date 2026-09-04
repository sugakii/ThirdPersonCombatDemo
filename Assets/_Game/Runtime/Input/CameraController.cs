using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTarget;

    [SerializeField]
    private float mouseSensitivity = 3f;

    private PlayerInputReader inputReader;
    private float xRotation;
    private float yRotation;

    private void Awake()
    {
        inputReader = GetComponent<PlayerInputReader>();
    }

    void Update()
    {
        Vector2 lookinput = inputReader.LookInput;

        float mouseX = lookinput.x;
        float mouseY = lookinput.y;

        yRotation += mouseX * mouseSensitivity;
        xRotation -= mouseY * mouseSensitivity;

        xRotation = Mathf.Clamp(
            xRotation,
            -40f,
            70f
        );

        cameraTarget.rotation =
        Quaternion.Euler(
            xRotation,
            yRotation,
            0
        );
    }
}

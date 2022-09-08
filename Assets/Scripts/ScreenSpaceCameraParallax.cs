using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenSpaceCameraParallax : MonoBehaviour
{
    [SerializeField] float smoothTime = 0.3f;
    [SerializeField] float[] minMaxXAxis; //0 min, 1 max
    [SerializeField] float[] minMaxYAxis; //0 min, 1 max
    [SerializeField] float mouseSensitivity = 50.0f;
    float xVelocity = 0.0f;
    float yVelocity = 0.0f;
    float x;
    float y;
    float xSmooth;
    float ySmooth;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        x -= Input.GetAxis("Mouse X") * mouseSensitivity * 0.02f;
        y -= Input.GetAxis("Mouse Y") * mouseSensitivity * .02f;

        if (y > minMaxYAxis[1]) y = minMaxYAxis[1];
        if (y < minMaxYAxis[0]) y = minMaxYAxis[0];
        if (x > minMaxXAxis[1]) x = minMaxXAxis[1];
        if (x < minMaxXAxis[0]) x = minMaxXAxis[0];

        xSmooth = Mathf.SmoothDamp(xSmooth, x, ref xVelocity, smoothTime);
        ySmooth = Mathf.SmoothDamp(ySmooth, y, ref yVelocity, smoothTime);

        Quaternion parallax = new Quaternion();
        parallax = Quaternion.Euler(ySmooth / 7, xSmooth / 7, 0);

        transform.localPosition = new Vector3(xSmooth, ySmooth, 0f);
        transform.localRotation = parallax;
    }
}
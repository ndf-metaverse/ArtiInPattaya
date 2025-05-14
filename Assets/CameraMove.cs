using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 5f; // ความเร็วในการขยับกล้อง

    //void Start()
    //{
    //    float targetWidth = 6450f;
    //    float targetHeight = 1440f;

    //    float targetAspect = targetWidth / targetHeight;
    //    float windowAspect = (float)Screen.width / (float)Screen.height;

    //    float sizeFactor = windowAspect / targetAspect;

    //    Camera.main.orthographicSize = Camera.main.orthographicSize / sizeFactor;
    //}

    void Update()
    {
        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1f;
        }

        transform.position += new Vector3(horizontal, 0, 0) * moveSpeed * Time.deltaTime;
    }
}

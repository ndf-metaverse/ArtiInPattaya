using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    private bool fromLeft;
    private float speed;
    private Camera cam;

    public void Initialize(bool fromLeft, float speed, Camera cam)
    {
        this.fromLeft = fromLeft;
        this.speed = speed;
        this.cam = cam;
    }

    void Update()
    {
        float moveDir = fromLeft ? 1f : -1f;
        if(fromLeft)
        {
            transform.Translate(Vector3.right * moveDir * speed * Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.left * moveDir * speed * Time.deltaTime);
        }

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);
        
            Destroy(gameObject,30);
        
    }
}

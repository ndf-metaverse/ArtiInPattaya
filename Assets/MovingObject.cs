using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Action<MovingObject> OnDestroyed;


    private bool fromLeft;
    public float speed;
    private Camera cam;

    private float screenBuffer = 100f;
    public int bounceCount = 0;
    public const int maxBounce = 3;
    private bool hasReturned = false;

    public void Initialize(bool fromLeft, float speed, Camera cam)
    {
        this.fromLeft = fromLeft;
        this.speed = speed;
        this.cam = cam;
    }

    void Update()
    {
        float moveAmount = speed * Time.deltaTime;
        
        transform.Translate(Vector3.right * moveAmount);

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

        if (!hasReturned && (screenPos.x < -screenBuffer || screenPos.x > Screen.width + screenBuffer))
        {
            hasReturned = true;

            fromLeft = !fromLeft;

            Vector3 pos = transform.position;
            pos.z += fromLeft ? 1f : -1f;
            transform.position = pos;

            Vector3 scale = transform.localScale;
            scale.z = -scale.z;
            transform.localScale = scale;

            if (pos.z <= 0)
            {
                transform.Rotate(0f, 180f, 0f);
            }
            bounceCount++;
            if (bounceCount >= maxBounce -1)
            {
                OnDestroyed?.Invoke(this);
                Destroy(gameObject);
            }
        }

        if (hasReturned && screenPos.x >= 0 && screenPos.x <= Screen.width)
        {
            hasReturned = false;
        }

    }
}

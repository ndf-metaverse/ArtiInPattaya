using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public Action<MovingObject> OnDestroyed;


    public bool fromLeft;
    public float speed;
    private Camera cam;

    private float screenBuffer = 100f;
    public int bounceCount = 0;
    public const int maxBounce = 3;
    private bool hasReturned = false;
    public GameObject[] refLaneGameobject;
    public GameObject[] refScaleGameobject;
    public float idleTime = 2f;
    public float idleCount = 0f;
    public void Initialize(bool fromLeft, float speed, Camera cam, GameObject[] refLaneGameobject,float idelTime, GameObject[] refScaleLane)
    {
        this.fromLeft = fromLeft;
        this.speed = speed;
        this.cam = cam;
        this.refLaneGameobject = refLaneGameobject;
        this.idleTime = idelTime;
        this.refScaleGameobject = refScaleLane;
    }

    void Update()
    {
        if (idleCount < idleTime)
        {
            idleCount += Time.deltaTime;

        }
        else
        {
            float moveAmount = speed * Time.deltaTime;

            transform.Translate(Vector3.right * moveAmount);
        }
        

        Vector3 screenPos = cam.WorldToScreenPoint(transform.position);

        if (!hasReturned && (screenPos.x < -screenBuffer || screenPos.x > Screen.width + screenBuffer))
        {
            hasReturned = true;

            fromLeft = !fromLeft;

            Vector3 pos = transform.position;

            if (refLaneGameobject != null && refLaneGameobject.Length > 0)
            {
                bounceCount++;
                if (bounceCount >= maxBounce)
                {
                    OnDestroyed?.Invoke(this);
                    Destroy(gameObject);
                }

                int randomLaneIndex = bounceCount;
                if(refLaneGameobject[randomLaneIndex] == null)
                {
                    Destroy(gameObject);
                }
                float newY = refLaneGameobject[randomLaneIndex].transform.position.y;
                pos.y = newY;
                float newZ = refLaneGameobject[randomLaneIndex].transform.position.z;
                pos.z = newZ;
                transform.localScale = new Vector3(refScaleGameobject[bounceCount].transform.localScale.x, refScaleGameobject[bounceCount].transform.localScale.y, transform.localScale.z);

            }

            pos.z += fromLeft ? 1f : -1f;
            transform.position = pos;

            Vector3 scale = transform.localScale;
            scale.z = -scale.z;
            transform.localScale = scale;

            
                if (bounceCount > 2)
                {
                    transform.Rotate(0f, 0, 0f);

                }
                else
                {
                    transform.Rotate(0f, 180f, 0f);
                }
            
        }

        if (hasReturned && screenPos.x >= 0 && screenPos.x <= Screen.width)
        {
            hasReturned = false;
        }

    }
}

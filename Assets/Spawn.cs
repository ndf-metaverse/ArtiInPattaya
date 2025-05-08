using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public GameObject[] objectToSpawn;
    public float spawnInterval = 2f;
    public float objectSpeed = 5f;
    public Camera mainCamera;
    public static Spawn instance;

    public void Start()
    {
        instance = this; 
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObject();
        }
    }
    public void SpawnObject()
    {
        float y = Random.Range(-7, -5); 

        bool spawnLeft = Random.value < 0.5f;

        Vector3 screenEdge = spawnLeft ? Vector3.zero : new Vector3(Screen.width, 0, 0);
        Vector3 worldEdge = mainCamera.ScreenToWorldPoint(new Vector3(screenEdge.x, Screen.height / 2f, 0));
        worldEdge.y = y;

        float z = Mathf.Lerp(-7f, -2f, Mathf.InverseLerp(-8f, -6f, y));
        worldEdge.z = z;
        worldEdge.x = spawnLeft ? -25f : 25f; // Set the x position based on spawn direction
        int rotateY = spawnLeft ? 0 : 180;
        int randomIndex = Random.Range(0, objectToSpawn.Length);
        GameObject selectedPrefab = objectToSpawn[randomIndex];

        GameObject obj = Instantiate(selectedPrefab, worldEdge, Quaternion.Euler(0, rotateY, 0));
        if (spawnLeft)
        {
            Vector3 scale = obj.transform.localScale;
            scale.z *= -1;
            obj.transform.localScale = scale;
        }
        obj.AddComponent<MovingObject>().Initialize(spawnLeft, objectSpeed, mainCamera);
    }
}

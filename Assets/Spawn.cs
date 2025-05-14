using System;
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
    public GameObject spawnParticle;

    public string[] objectScanName;

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
        StartCoroutine(SpawnObjectRoutine());
    }
    private IEnumerator SpawnObjectRoutine()
    {
        float y = UnityEngine.Random.Range(-7, -5);
        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        Vector3 screenEdge = spawnLeft ? Vector3.zero : new Vector3(Screen.width, 0, 0);
        Vector3 worldEdge = mainCamera.ScreenToWorldPoint(new Vector3(screenEdge.x, Screen.height / 2f, 0));
        worldEdge.y = -8.35f;
        worldEdge.z = -2;
        worldEdge.x = spawnLeft ? -5f : 5f;

        Quaternion particleRotation = Quaternion.identity;
        GameObject particle = Instantiate(spawnParticle, worldEdge, particleRotation);
        particle.transform.localScale = new Vector3(2f, 2f, 2f);
        Destroy(particle, 8f);

        yield return new WaitForSeconds(2f);

        int rotateY = spawnLeft ? 0 : 180;
        int selectIndex = UnityEngine.Random.Range(0, objectToSpawn.Length);
        //Turtle
        if (QRDecodeTest.instance.textScan == objectScanName[0])
        {
            selectIndex = 0;
        }
        //Peacock
        else if (QRDecodeTest.instance.textScan == objectScanName[1])
        {
            selectIndex = 1;
        }
        //Dear
        else if (QRDecodeTest.instance.textScan == objectScanName[2])
        {
            selectIndex = 2;
        }
        //Giraffe
        else if (QRDecodeTest.instance.textScan == objectScanName[3])
        {
            selectIndex = 3;
        }
        //Lion
        else if (QRDecodeTest.instance.textScan == objectScanName[4])
        {
            selectIndex = 4;
        }
        //Elephant
        else if (QRDecodeTest.instance.textScan == objectScanName[5])
        {
            selectIndex = 5;
        }

        GameObject selectedPrefab = objectToSpawn[selectIndex];
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

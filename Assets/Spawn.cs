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

    public GameObject[] refLaneGameobject;
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
        bool spawnLeft = UnityEngine.Random.value < 0.5f;

        int randomLaneIndex = UnityEngine.Random.Range(0, refLaneGameobject.Length);
        Vector3 lanePosition = refLaneGameobject[randomLaneIndex].transform.position;
        int leftZSpawn = spawnLeft ? 0 : 1;
        Vector3 spawnPosition = new Vector3(spawnLeft ? -5f : 5f, lanePosition.y, lanePosition.z + leftZSpawn);

        Quaternion particleRotation = Quaternion.identity;
        GameObject particle = Instantiate(spawnParticle, spawnPosition + new Vector3(0,1,0), particleRotation);
        particle.transform.localScale = new Vector3(2f, 2f, 2f);
        Destroy(particle, 8f);

        yield return new WaitForSeconds(2f);

        int rotateY = spawnLeft ? 0 : 180;
        int selectIndex = UnityEngine.Random.Range(0, objectToSpawn.Length);

        string scanned = QRDecodeTest2.instance.textScan;
        for (int i = 0; i < objectScanName.Length; i++)
        {
            if (scanned == objectScanName[i])
            {
                selectIndex = i;
                break;
            }
        }

        GameObject selectedPrefab = objectToSpawn[selectIndex];
        GameObject obj = Instantiate(selectedPrefab, spawnPosition, Quaternion.Euler(0, rotateY, 0));

        if (spawnLeft)
        {
            Vector3 scale = obj.transform.localScale;
            scale.z *= -1;
            obj.transform.localScale = scale;
        }

        obj.AddComponent<MovingObject>().Initialize(spawnLeft, objectSpeed, mainCamera);
    }

}

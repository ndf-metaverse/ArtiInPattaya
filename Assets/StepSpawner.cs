using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepSpawner : MonoBehaviour
{
    public GameObject prefab;
    public float stepDistance = 1f;
    public float distanceRandom = 0.25f;
    public float rotateRange = 20f;
    public float scaleRange = 0.25f;

    private float spawnDistance;
    private Vector3 lastStepPos;
    
    void Start()
    {
        lastStepPos = transform.position;
        spawnDistance = stepDistance + Random.Range(-distanceRandom, distanceRandom);
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, lastStepPos) >= spawnDistance)
        {
            SpawnPrefab();
            lastStepPos = transform.position;
            spawnDistance = stepDistance + Random.Range(-distanceRandom, distanceRandom);
        }
    }

    void SpawnPrefab()
    {
        Quaternion rotation = Quaternion.Euler(Random.Range(0, rotateRange), Random.Range(0, 360), 0);
        float scale = 1 + Random.Range(-scaleRange, scaleRange);
        GameObject go = Instantiate(prefab, transform.position, rotation);
        go.transform.localScale = new Vector3(scale, scale, scale);
    }
}

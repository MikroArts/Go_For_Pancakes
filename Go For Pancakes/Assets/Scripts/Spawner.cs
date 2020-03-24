using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] vehicles;
    public float startWaitTime;
    float waitTime;

    void Start()
    {
        waitTime = startWaitTime;
    }

    void Update()
    {
        int rnd = Random.Range(0, vehicles.Length);
        waitTime -= Time.deltaTime;

        if (waitTime <= 0)
        {
            Instantiate(vehicles[rnd], transform.position, Quaternion.identity);
            waitTime = startWaitTime;
        }
    }
}

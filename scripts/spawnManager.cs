using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public static spawnManager instance;
    private void Awake()
    {
        instance = this;
    }
    public void Start()
    {
        foreach(Transform spawn in spawnPoints)
        {
            spawn.gameObject.SetActive(false);
        }
    }
    public Transform SpawnPointSelect()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

}


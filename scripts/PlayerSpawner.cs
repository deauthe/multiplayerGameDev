using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;
    private void Awake()
    {
        instance = this;
    }
    public  GameObject playerPrefab;
    private GameObject player;

    void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }
    public void SpawnPlayer()
    {
        Transform playerSpawner = spawnManager.instance.SpawnPointSelect();
        player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawner.position, playerSpawner.rotation);

    }

  
}

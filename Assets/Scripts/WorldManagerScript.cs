using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManagerScript : MonoBehaviour
{
    public GameObject world;
    private string filePath;
    public static WorldManagerScript instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        filePath = Application.persistentDataPath + "/Savedata/";
    }
    // Funktio, jota k‰ytet‰‰n maailman tilanteen tallentamiseen.
    public void SaveWorld(string fileName)
    {
        Debug.Log(filePath + fileName);
    }
}

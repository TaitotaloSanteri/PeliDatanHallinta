using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldManagerScript : MonoBehaviour
{
    public GameObject world;
    private string filePath;
    public static WorldManagerScript instance;
    private GameObject[] worldObjects;
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
        Application.targetFrameRate = 120;
        worldObjects = Resources.LoadAll<GameObject>("WorldObjects");
    }

    // Dekoodaus, kun ladataan pelin tilanne.
    private string Decode(string baseString)
    {
        byte[] bytes = System.Convert.FromBase64String(baseString);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    // Enkoodaus, kun halutaan tallentaa pelin tilanne.
    private string Encode(string jsonData)
    {
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
        return System.Convert.ToBase64String(bytes);
    }

    // Käytetään silloin, kun halutaan ladata pelin tilanne, niin siirretään
    // BaseSaveDatasta Unityn transformiin
    private void CopyToUnityFromSave(Transform copyTo, BaseSaveData copyFrom)
    {
        copyTo.position = copyFrom.position;
        copyTo.rotation = copyFrom.rotation;
    }

    // Käytetään silloin, kun siirrettään Unityn transformista tietoa BaseSaveDataan
    // eli kun halutaan tallentaa pelin tilanne
    private void CopyToSaveFromUnity(BaseSaveData copyTo, Transform copyFrom)
    {
        copyTo.position = copyFrom.position;
        copyTo.rotation = copyFrom.rotation;
    }
    private void CreateFolderIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    // Funktio, jota käytetään maailman tilanteen tallentamiseen.
    public void SaveWorld(string fileName)
    {
        // Etitään PlayerController maailmasta
        PlayerController player = world.GetComponentInChildren<PlayerController>();
        // Luodaan WorldSaveData, jonka avulla tallennetaan kaikki maailmasta löytyvä tieto.
        WorldSaveData worldSaveData = new WorldSaveData() { dateAndTime = DateTime.Now.ToString() };
        
        // Kopioidaan pelaajan tiedot WorldSaveDataan
        worldSaveData.psd = player.psd;
        CopyToSaveFromUnity(worldSaveData.psd, player.transform);
        
        // Muutetaan WorldSaveData JSON -muotoon.
        string jsonData = JsonUtility.ToJson(worldSaveData, true);
        string finalData = Encode(jsonData);

        // Tarkistetaan, että hakemisto on olemassa
        CreateFolderIfNotExists(filePath);

        // Tallennetaan tiedot tiedostoon
        File.WriteAllText(filePath + fileName, finalData);
        Debug.Log(filePath);
    }
    private GameObject FindGameObjectByComponentName(string componentName)
    {
        // Käydään läpi kaikki worldObjects taulusta löytyvät GameObjektit. Nämä objektit
        // ovat siis kansiossa Resources/WorldObjects/
        foreach(GameObject obj in worldObjects)
        {
            Component component = obj.GetComponent(componentName);
            // Jos GameObjektista löytyy komponentti, jota haetaan, palautetaan kyseinen objekti
            if (component)
            {
                return obj;
            }
        }
        // Jos mistään GameObjektista ei löytynyt haettua komponenttia, palautetaan NULL 
        // virheilmoituksen kera
        Debug.Log($"Komponenttia {componentName} ei löytynyt mistään Resources/WorldObjects/ kansiosta olevasta objektista");
        return null;
    }


    private void DestroyWorldAndCreateNewWorld()
    {
        Destroy(world);
        world = new GameObject("World");
        world.transform.position = Vector3.zero;
        world.transform.rotation = Quaternion.identity;
        world.transform.SetParent(null);
    }

    public void LoadWorld(string fileName)
    {
        CreateFolderIfNotExists(filePath);
        // Tarkistetaan, että tiedosto löytyy
        if (!File.Exists(filePath + fileName))
        {
            Debug.Log($"Tiedostoa {fileName} ei löytynyt.");
            return;
        }
        // Tuhotaan vanha maailma ja luodaan uusi tyhjä maailma tilalle. Uutta maailmaa ruvetaa
        // täyttämään objekteilla jotka löytyvät tiedostosta
        DestroyWorldAndCreateNewWorld();
        // Luetaan tiedoston data
        string fileData = File.ReadAllText(filePath + fileName);
        // Muutetaan se JSON -muotoon
        string jsonData = Decode(fileData);
        // Muutetan vielä lopuksi se C# -muotoon WorldSaveData -luokan objektiksi
        WorldSaveData worldSaveData = JsonUtility.FromJson<WorldSaveData>(jsonData);
        // Spawnataan uusi pelaaja maailmaan
        GameObject newPlayer = Instantiate(FindGameObjectByComponentName("PlayerController"), world.transform);
        // Asetetaan pelaajan PlayerSaveData tiedostosta löytyvän PlayerSaveDatan mukaan.
        newPlayer.GetComponent<PlayerController>().psd = worldSaveData.psd;
        // Pitää myös päivittää transformin position ja rotation erikseen.
        CopyToUnityFromSave(newPlayer.transform, worldSaveData.psd);


    }
}

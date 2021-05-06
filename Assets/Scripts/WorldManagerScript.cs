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

    // K�ytet��n silloin, kun halutaan ladata pelin tilanne, niin siirret��n
    // BaseSaveDatasta Unityn transformiin
    private void CopyToUnityFromSave(Transform copyTo, BaseSaveData copyFrom)
    {
        copyTo.position = copyFrom.position;
        copyTo.rotation = copyFrom.rotation;
    }

    // K�ytet��n silloin, kun siirrett��n Unityn transformista tietoa BaseSaveDataan
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

    // Funktio, jota k�ytet��n maailman tilanteen tallentamiseen.
    public void SaveWorld(string fileName)
    {
        // Etit��n PlayerController maailmasta
        PlayerController player = world.GetComponentInChildren<PlayerController>();
        // Luodaan WorldSaveData, jonka avulla tallennetaan kaikki maailmasta l�ytyv� tieto.
        WorldSaveData worldSaveData = new WorldSaveData() { dateAndTime = DateTime.Now.ToString() };
        
        // Kopioidaan pelaajan tiedot WorldSaveDataan
        worldSaveData.psd = player.psd;
        CopyToSaveFromUnity(worldSaveData.psd, player.transform);
        
        // Muutetaan WorldSaveData JSON -muotoon.
        string jsonData = JsonUtility.ToJson(worldSaveData, true);
        string finalData = Encode(jsonData);

        // Tarkistetaan, ett� hakemisto on olemassa
        CreateFolderIfNotExists(filePath);

        // Tallennetaan tiedot tiedostoon
        File.WriteAllText(filePath + fileName, finalData);
        Debug.Log(filePath);
    }
    private GameObject FindGameObjectByComponentName(string componentName)
    {
        // K�yd��n l�pi kaikki worldObjects taulusta l�ytyv�t GameObjektit. N�m� objektit
        // ovat siis kansiossa Resources/WorldObjects/
        foreach(GameObject obj in worldObjects)
        {
            Component component = obj.GetComponent(componentName);
            // Jos GameObjektista l�ytyy komponentti, jota haetaan, palautetaan kyseinen objekti
            if (component)
            {
                return obj;
            }
        }
        // Jos mist��n GameObjektista ei l�ytynyt haettua komponenttia, palautetaan NULL 
        // virheilmoituksen kera
        Debug.Log($"Komponenttia {componentName} ei l�ytynyt mist��n Resources/WorldObjects/ kansiosta olevasta objektista");
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
        // Tarkistetaan, ett� tiedosto l�ytyy
        if (!File.Exists(filePath + fileName))
        {
            Debug.Log($"Tiedostoa {fileName} ei l�ytynyt.");
            return;
        }
        // Tuhotaan vanha maailma ja luodaan uusi tyhj� maailma tilalle. Uutta maailmaa ruvetaa
        // t�ytt�m��n objekteilla jotka l�ytyv�t tiedostosta
        DestroyWorldAndCreateNewWorld();
        // Luetaan tiedoston data
        string fileData = File.ReadAllText(filePath + fileName);
        // Muutetaan se JSON -muotoon
        string jsonData = Decode(fileData);
        // Muutetan viel� lopuksi se C# -muotoon WorldSaveData -luokan objektiksi
        WorldSaveData worldSaveData = JsonUtility.FromJson<WorldSaveData>(jsonData);
        // Spawnataan uusi pelaaja maailmaan
        GameObject newPlayer = Instantiate(FindGameObjectByComponentName("PlayerController"), world.transform);
        // Asetetaan pelaajan PlayerSaveData tiedostosta l�ytyv�n PlayerSaveDatan mukaan.
        newPlayer.GetComponent<PlayerController>().psd = worldSaveData.psd;
        // Pit�� my�s p�ivitt�� transformin position ja rotation erikseen.
        CopyToUnityFromSave(newPlayer.transform, worldSaveData.psd);


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Perusta kaikille pelimaailmassa oleville objekteille. Kaikilla objekteilla, joiden tilanne
// halutaan tallentaa lˆytyy position ja rotation.
[System.Serializable] // System.Serializable t‰rke‰, jotta tiedot n‰kyy Unityn inspectorissa ja ne pystyy tallentamaan
public class BaseSaveData
{
    [HideInInspector] public Vector3 position;
    [HideInInspector] public Quaternion rotation;
}

// Pelaajan data joka halutaan tallentaa. Perii BaseSaveDatan.
[System.Serializable]
public class PlayerSaveData : BaseSaveData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float health;
    public int experiencePoints;
}


[System.Serializable]
public class CollectibleSaveData : BaseSaveData
{
    // ComponentName -muuttujassa pidet‰‰n kirjaa ker‰tt‰v‰n esineen varsinaisesta tyypist‰.
    // esim. HealthCollectible tai MoveSpeedCollectible
    [HideInInspector] public string componentName;
    // Value -muuttujassa pidet‰‰n kirjaa kuinka isolla m‰‰r‰ll‰ ker‰tt‰v‰ kasvattaa joko esimerkiksi
    // healthia tai movespeedia.
    public float value;
}

[System.Serializable]
public class WorldSaveData
{
    public string dateAndTime;
    public PlayerSaveData psd;
}

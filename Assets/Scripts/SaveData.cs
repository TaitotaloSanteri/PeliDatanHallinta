using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Perusta kaikille pelimaailmassa oleville objekteille. Kaikilla objekteilla, joiden tilanne
// halutaan tallentaa löytyy position ja rotation.
[System.Serializable] // System.Serializable tärkeä, jotta tiedot näkyy Unityn inspectorissa ja ne pystyy tallentamaan
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
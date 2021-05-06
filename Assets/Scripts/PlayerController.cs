using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerSaveData psd;

    public void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        transform.position += transform.forward * y * psd.moveSpeed * Time.deltaTime;
        transform.Rotate(0f, x * psd.rotationSpeed * Time.deltaTime, 0f);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            WorldManagerScript.instance.SaveWorld("SaveFile.dat");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class CollectibleBase : MonoBehaviour
{
    public CollectibleSaveData csd;
    public abstract void OnCollected(PlayerSaveData psd);
}

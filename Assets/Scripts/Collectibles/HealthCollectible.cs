using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectible : CollectibleBase
{
    public override void OnCollected(PlayerSaveData psd)
    {
        psd.health += csd.value;
    }
}

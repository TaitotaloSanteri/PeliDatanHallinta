using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedCollectible : CollectibleBase
{
    public override void OnCollected(PlayerSaveData psd)
    {
        psd.moveSpeed += csd.value;
    }
}

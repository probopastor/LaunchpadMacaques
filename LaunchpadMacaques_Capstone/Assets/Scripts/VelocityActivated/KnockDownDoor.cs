using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockDownDoor : VelocityActivated
{

    public override void Activate()
    {
        this.gameObject.SetActive(false);
    }
}

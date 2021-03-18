using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInstancedColor : MonoBehaviour
{
    public Color color;

    Renderer renderer1;
    MaterialPropertyBlock pBlock;

    private void OnValidate()
    {
        if (renderer1 == null)
        {
            renderer1 = GetComponent<Renderer>();
        }
        if (pBlock == null)
        {
            pBlock = new MaterialPropertyBlock();
        }

        renderer1.GetPropertyBlock(pBlock);

        pBlock.SetColor("AdjustmentColor", color);

        renderer1.SetPropertyBlock(pBlock);
    }
}

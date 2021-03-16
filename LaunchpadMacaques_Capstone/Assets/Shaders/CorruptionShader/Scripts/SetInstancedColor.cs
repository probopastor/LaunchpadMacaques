using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInstancedColor : MonoBehaviour
{
    public Color color;

    Renderer rendererTarget;
    MaterialPropertyBlock pBlock;

    private void OnValidate()
    {
        if (rendererTarget == null)
        {
            rendererTarget = GetComponent<Renderer>();
        }
        if (pBlock == null)
        {
            pBlock = new MaterialPropertyBlock();
        }

        rendererTarget.GetPropertyBlock(pBlock);

        pBlock.SetColor("AdjustmentColor", color);

        rendererTarget.SetPropertyBlock(pBlock);
    }
}

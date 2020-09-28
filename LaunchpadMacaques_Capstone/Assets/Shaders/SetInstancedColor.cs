using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInstancedColor : MonoBehaviour
{
    public Color color;

    Renderer renderer;
    MaterialPropertyBlock pBlock;

    private void OnValidate()
    {
        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
        }
        if (pBlock == null)
        {
            pBlock = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(pBlock);

        pBlock.SetColor("AdjustmentColor", color);

        renderer.SetPropertyBlock(pBlock);
    }
}

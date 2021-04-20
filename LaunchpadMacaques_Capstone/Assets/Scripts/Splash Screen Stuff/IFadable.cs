using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IFadable
{
    IEnumerator FadeIn(GameObject fadableObject, float targetOpacity, float duration);

    IEnumerator FadeOut(GameObject fadableObject, float targetOpacity, float duration);
}

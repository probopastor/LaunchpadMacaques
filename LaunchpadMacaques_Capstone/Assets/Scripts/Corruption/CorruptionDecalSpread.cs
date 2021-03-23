using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CorruptionDecalSpread : MonoBehaviour
{
    [SerializeField] Material spreadMaterial = null;
    [SerializeField] Material finishedMaterial = null;

    [SerializeField] float spreadSpeed = 1;

    DecalProjector projector;
    float progress;
    Material tempMat;

    // Start is called before the first frame update
    void Start()
    {
        projector = GetComponent<DecalProjector>();
        tempMat = new Material(spreadMaterial);
        projector.material = tempMat;
        progress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        progress += spreadSpeed * Time.deltaTime;
        if (progress >= 1f)
        {
            projector.material = finishedMaterial;
            Destroy(tempMat);
        }
        else
        {
            projector.material.SetFloat("Progress", progress);
        }
    }
}

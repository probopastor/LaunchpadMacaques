using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class CorruptionDecalSpread : MonoBehaviour
{
    [SerializeField] Material spreadMaterial;
    [SerializeField] Material finishedMaterial;

    [SerializeField] float spreadSpeed = 1;

    DecalProjector projector;
    float progress;
    Material tempMat;

    public CorruptionDecalSpread(Material spreadMaterial, Material finishedMaterial)
    {
        this.spreadMaterial = spreadMaterial;
        this.finishedMaterial = finishedMaterial;
    }

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

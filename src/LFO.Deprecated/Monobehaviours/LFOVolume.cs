using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(Renderer))]
public class LFOVolume : MonoBehaviour
{
    private static Resolution _resolution;
    public Resolution Resolution = Resolution.Medium;
    private Material material
    {
        get
        {
            if (Application.isEditor)
                return GetComponent<Renderer>().sharedMaterial;
            else
                return GetComponent<Renderer>().material;
        }
    }

    private void Start()
    {
        if (material is null)
            return;
        material.SetFloat("_TimeOffset", UnityEngine.Random.Range(-10f, 10f));
        material.SetInt("_Resolution", (int)_resolution);
        material.SetVector("scale", transform.lossyScale);
        material.SetMatrix("rotation", Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation)));
        material.SetVector("position", transform.position);
    }

    private void LateUpdate()
    {
        if (material is null)
            return;
        material.SetVector("scale", transform.lossyScale);
        material.SetMatrix("rotation", Matrix4x4.Rotate(Quaternion.Inverse(transform.rotation)));
        material.SetVector("position", transform.position);
        if (Resolution != _resolution)
        {
            _resolution = Resolution;
            material.SetInt("_Resolution", (int)_resolution);
            //Debug.Log($"Changed resolution of {gameObject.name}");
        }
    }

}
public enum Resolution
{
    Minimal,
    Low,
    Medium,
    High,
    Extreme
}

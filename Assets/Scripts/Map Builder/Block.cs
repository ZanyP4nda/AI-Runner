using UnityEngine;

public class Block : MonoBehaviour
{
    private MeshRenderer meshRend;
    private Collider collider;
    [SerializeField]
    private bool isPerim = false;

    public bool isExist = true;

    private void Start()
    {
        meshRend = (MeshRenderer)GetComponentInChildren(typeof(MeshRenderer));
    }

    public void RemoveBlock()
    {
        if(isExist && !isPerim)
        {
            // Make block transparent
            meshRend.material = MapBuilder.mapBuilder.transparent;

            // Get collider if not yet
            if(!collider)
                collider = (Collider)GetComponentInChildren(typeof(Collider));
            // Disable collider
            collider.enabled = false;
            // Disable block's collider
            // Set exist to false
            isExist = false;
        }
    }
    
    public void AddBlock()
    {
        if(!isExist)
        {
            // Make block opaque
            meshRend.material = MapBuilder.mapBuilder.defaultMat;
            // Enable collider
            collider.enabled = true;
            // Set exist to true
            isExist = true;

            Debug.Log($"Add {gameObject.name}");
        }
    }
}
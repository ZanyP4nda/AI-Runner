using UnityEngine;

public class Block : MonoBehaviour
{
    private MeshRenderer meshRend;
    private GameObject colliderObj;
    [SerializeField]
    private bool isPerim = false;

    public bool IsExist = true;

    private void Start()
    {
        meshRend = (MeshRenderer)GetComponentInChildren(typeof(MeshRenderer));
        colliderObj = transform.GetChild(1).gameObject;
    }

    public void RemoveBlock()
    {
        if(IsExist && !isPerim)
        {
            // Make block transparent
            meshRend.material = MapBuilder.mapBuilder.transparent;
            // Set layer to "disabled"
            colliderObj.layer = 8;
            // Set exist to false
            IsExist = false;
        }
    }
    
    public void AddBlock()
    {
        if(!IsExist)
        {
            // Make block opaque
            meshRend.material = MapBuilder.mapBuilder.defaultMat;
            // Set layer back to default
            colliderObj.layer = 0;
            // Set exist to true
            IsExist = true;
        }
    }
}
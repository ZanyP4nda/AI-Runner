using UnityEngine;

public class Block : MonoBehaviour
{
    private MeshRenderer meshRend;
    public bool isExist = true;

    private void Start()
    {
        meshRend = (MeshRenderer)GetComponentInChildren(typeof(MeshRenderer));
    }

    public void RemoveBlock()
    {
        if(isExist)
        {
            // Make block transparent
            meshRend.material = MapBuilder.mapBuilder.transparent;
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
            // Set exist to true
            isExist = true;
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

public class MapBuilder : MonoBehaviour
{
    [SerializeField]
    private Camera lvlBuilderCam;
    [SerializeField]
    private Transform mapParent;

    public Material defaultMat;
    public Material transparent;

    public static MapBuilder mapBuilder;

    private void Awake()
    {
        mapBuilder = this;
    }

    private void Update()
    {
        // Remove block on left click
        if(Input.GetMouseButton(0))
        {
            RemoveBlock();
        }

        // Add block on right click
        if(Input.GetMouseButton(1))
        {
            AddBlock();
        }
    }

    private Block GetBlockUnderMouse()
    {
        Block output = null;
        RaycastHit hit;
        // Get a ray from the mouse position on the screen
        Ray ray = lvlBuilderCam.ScreenPointToRay(Input.mousePosition);
        // Cast a ray
        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            // Return the block
            output = (Block)hit.collider.GetComponentInParent(typeof(Block));
        }
        return output;
    }

    private void RemoveBlock()
    {
        try
        {
            GetBlockUnderMouse().RemoveBlock();
        }
        catch{}
    }

    private void AddBlock()
    {
        try
        {
            GetBlockUnderMouse().AddBlock();
        }
        catch{}
    }

    public void SaveMap()
    {
        Debug.Log(MapBuilder.mapBuilder.name);
//        foreach(Transform blockTransform in mapParent)
//        {
//            if(!blockTransform.GetComponent<Block>().isExist)
//                Destroy(blockTransform.gameObject);
//        }
    }
}

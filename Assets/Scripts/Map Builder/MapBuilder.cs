using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [Header("Level Builder")]
    [SerializeField]
    private Camera lvlBuilderCam;
    [SerializeField]
    private Transform mapParent;

    public Material defaultMat;
    public Material transparent;

    [Header("Checkpoint Config")]
    [SerializeField]
    private RouteAlignmentFinder finder;
    [SerializeField]
    private GameObject checkpointParent;
    [SerializeField]
    private GameObject checkpointPrefab;

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

        // Add checkpoint on middle click
        if(Input.GetMouseButtonDown(2))
        {
            AddCheckpoint();
        }
    }

    // Get a raycast from the mouse pointer to the world
    private RaycastHit GetMouseToWorld()
    {
        RaycastHit hit;
        // Get a ray from the mouse position on the screen
        Ray ray = lvlBuilderCam.ScreenPointToRay(Input.mousePosition);
        // Cast a ray
        Physics.Raycast(ray, out hit, Mathf.Infinity);

        return hit;
    }

    private Block GetBlockUnderMouse()
    {
        Block output = null;

        RaycastHit hit = GetMouseToWorld();
        if(!hit.Equals(null))
            output = (Block)hit.collider.GetComponentInParent(typeof(Block)); // Get the block the ray hits

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

    private void AddCheckpoint()
    {
        Debug.Log("Add checkpoint");
        RaycastHit hit = GetMouseToWorld();
        if(!hit.Equals(null) && hit.collider.CompareTag("map floor")) {
            // Instantiate the checkpoint
            GameObject newCheckpoint = Instantiate(checkpointPrefab, hit.point, Quaternion.AngleAxis(finder.GetRouteAngle(hit.point), Vector3.up));
            // Set checkpoint parent
            newCheckpoint.transform.SetParent(checkpointParent.transform);
        }
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
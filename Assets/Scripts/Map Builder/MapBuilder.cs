using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// Custom class to serialize checkpoints to a JSON file
[Serializable]
public class CheckpointData
{
    public float[] pos;
    public float[] rot;

    public CheckpointData(Vector3 _pos, Quaternion _rot)
    {
        pos = new float[3] {_pos.x, _pos.y, _pos.z};
        rot = new float[3] {_rot.x, _rot.y, _rot.z};
    }
}

/// Custom class to serialize maps to a JSON file
public class Map
{
    public List<int> DisabledBlockIndexes = new();
    public List<CheckpointData> Checkpoints = new();
}

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
    private Transform checkpointParent;
    [SerializeField]
    private GameObject checkpointPrefab;

    private static readonly float checkpointClickDelay = 0.2f;
    private bool isCheckpointClickCooldown = false;

    [Header("UI")]
    [SerializeField]
    private GameObject saveTextInput;

    // TODO: Move to lvl manager
    [SerializeField]
    private GameObject map;

    public static MapBuilder mapBuilder;

    private void Awake()
    {
        mapBuilder = this;
    }

    private void Start()
    {
        LoadMap("test");
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

    private int GetCombinedLayerMask(int[] layerNums)
    {
        int output = 1 << layerNums[0];
        for (int i = 1; i < layerNums.Length; i++)
        {
            output |= 1 << layerNums[i];
        }
        return output;
    }

    // Get a raycast from the mouse pointer to the world
    private RaycastHit GetMouseToWorld(int[] layerNums)
    {
        RaycastHit hit;
        // Get a ray from the mouse position on the screen
        Ray ray = lvlBuilderCam.ScreenPointToRay(Input.mousePosition);
        // Cast a ray
        Physics.Raycast(ray, out hit, Mathf.Infinity, GetCombinedLayerMask(layerNums));

        return hit;
    }

    private Block GetBlockUnderMouse()
    {
        Block output = null;

        RaycastHit hit = GetMouseToWorld(new int[] {0, 8});
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

    /// Add a checkpoint at the mouse point
    private void AddCheckpoint()
    {
        // Exit function if still cooling down
        if(isCheckpointClickCooldown)
            return;

        // Get the point where the mouse points at the map (on layers 0 and 7)
        RaycastHit hit = GetMouseToWorld(new int[] {0, 7});
        if(!hit.collider.CompareTag("map floor"))
            return;

        // Instantiate the checkpoint
        GameObject newCheckpoint = Instantiate(checkpointPrefab, hit.point, Quaternion.AngleAxis(finder.GetRouteAngle(hit.point), Vector3.up));
        // Set checkpoint parent
        newCheckpoint.transform.SetParent(checkpointParent);
        // Set checkpoint click delay
        StartCoroutine(CheckpointClickDelay());
    }

    /// Coroutine to delay checkpoint clicks
    private IEnumerator CheckpointClickDelay()
    {
        isCheckpointClickCooldown = true;
        yield return new WaitForSecondsRealtime(checkpointClickDelay);
        isCheckpointClickCooldown = false;
    }

    /// Save active map to a JSON file
    public void SaveMap()
    {
        string saveName = saveTextInput.GetComponent<TMP_InputField>().text;
        if(saveName.Equals(null))
        {
            Debug.LogError("No map save name entered.");
            return;
        }
        Map newMap = new();

        // Get all blocks in mapParent
        Block[] blocks = mapParent.GetComponentsInChildren<Block>();

        // Add the disabled blocks indices to newMap
        for (int i = 0; i < blocks.Length; i++)
        {
            if(!blocks[i].IsExist)
                newMap.DisabledBlockIndexes.Add(i);
        }

        // Add all checkpoints to newMap
        for(int i = 0; i < checkpointParent.childCount; i++)
        {
            Transform checkpoint = checkpointParent.GetChild(i);
            newMap.Checkpoints.Add(new CheckpointData(checkpoint.position, checkpoint.rotation));
        }

        // Write newMap to a JSON file
        string jsonMap = JsonUtility.ToJson(newMap);
        StreamWriter writer = new(File.Create($"{Application.dataPath}/Maps/{saveName}.json"));
        writer.Write(jsonMap);
        writer.Close();

        Debug.Log("Finished saving map...");
    }

    // TODO: Move to lvl manager
    public void LoadMap(string saveName)
    {
        StreamReader reader = new($"{Application.dataPath}/Maps/{saveName}.json");
        string jsonMap = reader.ReadToEnd();
        GameObject _map = Instantiate(map, Vector3.zero, Quaternion.identity);
        Map readMap = JsonUtility.FromJson<Map>(jsonMap);

        foreach(int blockIndex in readMap.DisabledBlockIndexes)
            Destroy(_map.transform.GetChild(blockIndex).gameObject);

        foreach(CheckpointData checkpoint in readMap.Checkpoints)
        {
            GameObject newCheckpoint = Instantiate(checkpointPrefab, new Vector3(checkpoint.pos[0], checkpoint.pos[1], checkpoint.pos[2]), Quaternion.Euler(checkpoint.rot[0], checkpoint.rot[1], checkpoint.rot[2]));
            newCheckpoint.transform.SetParent(checkpointParent);
        }

        reader.Close();
    }
}
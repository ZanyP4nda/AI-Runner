using System.Linq;
using ZestyP4nda.Core;
using UnityEngine;

public class RouteAlignmentFinder : MonoBehaviour
{
    private Vector3[] rangefinders;
    private float[] ranges;

    private void Awake()
    {
        SetRangefinders();
        // Initialise ranges
        ranges = new float[rangefinders.Length];
    }

    // Initialise rangefinders
    private void SetRangefinders()
    {
        rangefinders = new Vector3[] 
        {
            transform.TransformDirection(Vector3.left),
            transform.TransformDirection(Vector3.forward + Vector3.left),
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.forward + Vector3.right),
            transform.TransformDirection(Vector3.right),
            transform.TransformDirection(Vector3.back + Vector3.left),
            transform.TransformDirection(Vector3.back),
            transform.TransformDirection(Vector3.back + Vector3.right)
        };
    }

    // Get the distance of a wall from a rangefinder
    private float RangeFind(Vector3 rayDir)
    {
        float output = 0f;
        RaycastHit ray;
        // Cast a ray forwards from the runner at the 'rayDir' direction at an infinite range
        if(Physics.Raycast(transform.position, rayDir, out ray, Mathf.Infinity))
        {
            output = ray.distance;
        }

        return output;
    }

    // RangeFind from all rangefinders
    private void Scan()
    {
        for (int i = 0; i < rangefinders.Length; i++)
        {
            ranges[i] = RangeFind(rangefinders[i]);
        }
    }

    // Get route angle in degrees
    public float GetRouteAngle(Vector3 finderPos)
    {
        transform.position = new Vector3(finderPos.x, 0, finderPos.z);
        Scan();
        return 45 * DataHelper.GetMinIndex(ranges);
    }
}
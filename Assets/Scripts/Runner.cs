using System.Linq;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float rotSpeed = 3f;
    [SerializeField]
    private bool isMoving = true;

    [Header("Laps")]
    private int currentCheckpoint = 1;
    private int currentLap = 1;

    private Vector3[] rangefinders;
    private float[] ranges;

    private Rigidbody rb;

    private NN _nn;
    public NN nn { get {return _nn;} } // public getter of private variable nn
    public float fitness;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Initialise rangefinders
        SetRangefinders();
        // Initialise ranges
        ranges = new float[rangefinders.Length];

        // Instantiate a neural network for this runner
        _nn = new NN(5, 4);
    }

    private void FixedUpdate()
    {
        if(isMoving)
        {
            // Get inputs
            Scan();
            // Normalise all inputs
            NormaliseRanges();

            // Send inputs to NN
            float nnOut = _nn.FeedForward(ranges);

            // Move
            Move(nnOut);
        }
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
            transform.TransformDirection(Vector3.right)
        };
    }

    // Get the distance of a wall from a rangefinder
    private float RangeFind(Vector3 rayDir)
    {
        float output = 0f;
        RaycastHit ray;
        // Cast a ray forwards from the runner at the 'rayDir' direction at an infinite range, excluding the runner layer
        if(Physics.Raycast(transform.position, rayDir, out ray, Mathf.Infinity, ~(1<<6)))
        {
            // If the ray hits a collider, draw a ray
            Debug.DrawRay(transform.position, rayDir * ray.distance, Color.green);

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

    // Normalise all ranges
    private void NormaliseRanges()
    {
        float minRange = ranges.Min();
        float maxRange = ranges.Max();
        for (int i = 0; i < ranges.Length; i++)
        {
            ranges[i] = (ranges[i] - minRange) / (maxRange - minRange);
        }
    }

    // Rotate runner by 'rotAmt' and move forwards
    private void Move(float rotAmt)
    {
        // Rotate runner by 'rotAmt' amount
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * rotAmt * rotSpeed);
        rb.MovePosition(transform.position + transform.forward * speed * Time.deltaTime);
    }

    // Set up lap system
    private void OnTriggerEnter(Collider trigger)
    {
        // If pass through a checkpoint
        if(trigger.CompareTag("Checkpoint"))
        {
            // Increment checkpoint count
            currentCheckpoint++;
            // If exceeded max number checkpoints, means got back to start
            if(currentCheckpoint >= Manager.manager.checkpoints.Length)
            {
                // Reset checkpoint count
                currentCheckpoint = 1;
                // Increment lap count
                currentLap++;
            }
        }
    }

    // Get a fitness value based on the distance from the nearest checkpoint, the number of checkpoints the runner has run in this lap, and the current no. of laps
    private void GetFitness()
    {
        fitness = Vector3.Distance(transform.position, Manager.manager.checkpoints[currentCheckpoint - 1].position) * currentCheckpoint * currentLap;
    }

    // Run on collision with a wall
    private void OnCollisionEnter(Collision collision)
    {
        isMoving = false; // Stop moving
        GetFitness(); // Get fitness score
        Manager.manager.RunnerDie(); // Call manager method
        Debug.Log($"{gameObject.name} fitness: {fitness}");
    }
}
using UnityEngine;

public class Runner : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float rotSpeed = 3f;

    [Header("Laps")]
    [SerializeField]
    private Transform[] checkpoints;
    private int currentCheckpoint = 1;
    private int currentLap = 1;

    private Vector3[] rangefinders;
    private float[] ranges;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        SetRangefinders();
        // Initialise ranges
        ranges = new float[rangefinders.Length];
    }

    private void FixedUpdate()
    {
        Scan();
        transform.position += transform.forward * speed * Time.deltaTime;
        // Send inputs to NN
        // Move
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

    // Rotate runner by 'rotAmt' and move forwards
    private void Move(float rotAmt)
    {
        // Rotate runner by 'rotAmt' amount
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * rotAmt * rotSpeed);
        rb.MovePosition(transform.forward * speed * Time.deltaTime);
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
            if(currentCheckpoint >= checkpoints.Length)
            {
                // Reset checkpoint count
                currentCheckpoint = 1;
                // Increment lap count
                currentLap++;
            }
        }
    }

    private float GetFitness()
    {
        return Vector3.Distance(transform.position, checkpoints[currentCheckpoint - 1].position) * currentCheckpoint * currentLap;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fitness = GetFitness();
        Debug.LogError($"Runner collided with {collision.collider.name}. Fitness: {fitness}");
    }
}
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CarController))]
public class CarNavAI : MonoBehaviour
{
    public Color color;
    enum CarNavAIState
    {
        FollowPath,
        Stationary,
        Backward,
    }

    private CarNavAIState carNavAIState;

    public Transform target;
    public float pathUpdateInterval = 0.5f;
    public float reachThreshold = 1.0f;

    [Header("Recovery Settings")]
    private float stuckDetectionTime = 0.5f;

    public float stationaryDuration = 0.1f;
    private float currentStationaryTime = 0f;

    public float backwardDuration = 2.0f;
    private float currentBackwardTime = 0f;

    private NavMeshPath path;
    private float pathTimer;
    private int currentCornerIndex;

    private CarController carController;
    private Rigidbody carRigidbody;

    private float stuckTime = 0f;

    void Start()
    {
        path = new NavMeshPath();
        carController = GetComponent<CarController>();
        carRigidbody = GetComponent<Rigidbody>();
        RecalculatePath();
    }

    void Update()
    {
        pathTimer += Time.deltaTime;
        if (pathTimer >= pathUpdateInterval)
        {
            pathTimer = 0f;
            RecalculatePath();
        }

        if (path.corners.Length > 1)
        {
            switch (carNavAIState)
            {
                case CarNavAIState.FollowPath:
                    FollowPath();
                    break;
                case CarNavAIState.Stationary:
                    Stationary();
                    break;
                case CarNavAIState.Backward:
                    Backward();
                    break;
            }
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
    }

    private void RecalculatePath()
    {
        if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            currentCornerIndex = 1;
    }

    private void FollowPath()
    {
        if (carRigidbody.linearVelocity.magnitude < 0.1f)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= stuckDetectionTime)
            {
                carNavAIState = CarNavAIState.Stationary;
                stuckTime = 0f;
                return;
            }
        }

        if (currentCornerIndex >= path.corners.Length)
        {
            carController.MoveInput(0f);
            carController.SteerInput(0f);
            return;
        }

        carController.MoveInput(1.0f);
        carController.SteerInput(CalculateSteer());
    }

    private void Stationary()
    {
        currentStationaryTime += Time.deltaTime;
        carController.MoveInput(0f);
        carController.SteerInput(0f);
        if (currentStationaryTime >= stationaryDuration)
        {
            carNavAIState = CarNavAIState.Backward;
            currentStationaryTime = 0f;
        }
    }

    private void Backward()
    {
        currentBackwardTime += Time.deltaTime;

        float steerSign = Mathf.Sign(CalculateSteer());

        carController.MoveInput(-1f);
        carController.SteerInput(-steerSign * 0.9f);

        if (currentBackwardTime >= backwardDuration)
        {
            carNavAIState = CarNavAIState.FollowPath;
            currentBackwardTime = 0f;
        }
    }

    private float CalculateSteer()
    {
        Vector3 corner = path.corners[currentCornerIndex];
        Vector3 dir = corner - transform.position;
        float distance = dir.magnitude;

        if (distance < reachThreshold && currentCornerIndex < path.corners.Length - 1)
        {
            currentCornerIndex++;
            corner = path.corners[currentCornerIndex];
            dir = corner - transform.position;
        }

        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);
        return Mathf.Clamp(localDir.x, -1f, 1f);
    }
}

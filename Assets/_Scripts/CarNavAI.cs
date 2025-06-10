using UnityEngine;
using UnityEngine.AI;
using System.Linq;

[RequireComponent(typeof(CarController))]
public class CarNavAI : MonoBehaviour
{
    public LayerMask anotherAILayerMask;
    public Color color;

    enum CarNavAIState
    {
        FollowPath,
        Stationary,
        Backward,
        Avoid,
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
    public float avoidDuration = 1.5f;
    private float currentAvoidTime = 0f;

    private NavMeshPath path;
    private float pathTimer;
    private int currentCornerIndex;

    private CarController carController;
    private Rigidbody carRigidbody;

    private float stuckTime = 0f;
    public float avoidRadius = 2.0f;

    private Vector3 avoidanceDirection;

    void Start()
    {
        path = new NavMeshPath();
        carController = GetComponent<CarController>();
        carRigidbody = GetComponent<Rigidbody>();
        carNavAIState = CarNavAIState.FollowPath;
        RecalculatePath();
    }

    void Update()
    {
        if (carNavAIState == CarNavAIState.FollowPath)
        {
            pathTimer += Time.deltaTime;
            if (pathTimer >= pathUpdateInterval)
            {
                pathTimer = 0f;
                RecalculatePath();
            }
        }

        if (path.status != NavMeshPathStatus.PathInvalid && path.corners.Length > 1)
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
                case CarNavAIState.Avoid:
                    AvoidAnotherAI();
                    break;
            }
        }

        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
    }

    private void RecalculatePath()
    {
        if (target != null && NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            currentCornerIndex = 1;
        }
    }

    private void FollowPath()
    {
        if (CheckForAvoidance()) return;
        
        CheckForStuck();

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
        carController.SteerInput(-steerSign);

        if (currentBackwardTime >= backwardDuration)
        {
            carNavAIState = CarNavAIState.FollowPath;
            currentBackwardTime = 0f;
            RecalculatePath();
        }
    }

    private void AvoidAnotherAI()
    {
        currentAvoidTime += Time.deltaTime;

        Vector3 localAvoidDir = transform.InverseTransformDirection(avoidanceDirection);
        float steer = Mathf.Clamp(localAvoidDir.x, -1f, 1f);

        carController.MoveInput(1f);
        carController.SteerInput(steer);

        if (currentAvoidTime >= avoidDuration)
        {
            carNavAIState = CarNavAIState.FollowPath;
            currentAvoidTime = 0f;
        }

        CheckForStuck();
    }

    private float CalculateSteer()
    {
        if (currentCornerIndex >= path.corners.Length) return 0f;

        Vector3 corner = path.corners[currentCornerIndex];
        Vector3 dir = corner - transform.position;
        float distance = dir.magnitude;

        if (distance < reachThreshold && currentCornerIndex < path.corners.Length - 1)
        {
            currentCornerIndex++;
        }

        Vector3 localDir = transform.InverseTransformDirection(dir.normalized);
        return Mathf.Clamp(localDir.x, -1f, 1f);
    }

    private void CheckForStuck()
    {
        if (carRigidbody.linearVelocity.magnitude < 0.1f)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= stuckDetectionTime)
            {
                carNavAIState = CarNavAIState.Stationary;
                stuckTime = 0f;
            }
        }
        else
        {
            stuckTime = 0f;
        }
    }

    private bool CheckForAvoidance()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidRadius, anotherAILayerMask);

        if (colliders.Length <= 1)
        {
            return false;
        }

        Collider closestCollider = colliders
            .Where(c => c.gameObject != this.gameObject)
            .OrderBy(c => (transform.position - c.transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (closestCollider != null)
        {
            avoidanceDirection = (transform.position - closestCollider.transform.position).normalized;

            carNavAIState = CarNavAIState.Avoid;
            currentAvoidTime = 0f;
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        if (carNavAIState == CarNavAIState.Avoid)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + avoidanceDirection * 3f);
        }
    }
}
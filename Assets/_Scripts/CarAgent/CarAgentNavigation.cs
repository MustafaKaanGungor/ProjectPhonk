using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CarController))]
public class CarAgentNavigation : MonoBehaviour
{
    public LayerMask agentLayerMask;
    public Color color;

    public Transform target;
    public float pathUpdateInterval = 0.5f;
    public float reachThreshold = 1.0f;

    [Header("Recovery Settings")]
    public float stationaryDuration = 0.1f;
    public float backwardDuration = 2.0f;
    public float avoidDuration = 1.0f;
    public float avoidRadius = 1.0f;

    private enum CarAgentNavigationState
    {
        FollowPath,
        Stationary, 
        Backward, 
        Avoid
    }
    private CarAgentNavigationState carAgentNavigationState = CarAgentNavigationState.FollowPath;

    private NavMeshPath path;
    private float pathTimer;
    private int currentCornerIndex;

    private CarController carController;
    private Rigidbody carRigidbody;

    private float stuckTime = 0f;
    private float currentStationaryTime = 0f;
    private float currentBackwardTime = 0f;
    private float currentAvoidTime = 0f;
    private Vector3 avoidanceDirection;

    private const float stuckDetectionTime = 0.5f;
    private const float minVelocityThreshold = 0.1f;

    private void Start()
    {
        path = new NavMeshPath();
        carController = GetComponent<CarController>();
        carRigidbody = GetComponent<Rigidbody>();

        RecalculatePath();
    }

    private void Update()
    {
        if (carAgentNavigationState == CarAgentNavigationState.FollowPath)
        {
            pathTimer += Time.deltaTime;
            if (pathTimer >= pathUpdateInterval)
            {
                pathTimer = 0f;
                RecalculatePath();
            }
        }

        if (path.status == NavMeshPathStatus.PathInvalid || path.corners.Length <= 1)
            return;

        switch (carAgentNavigationState)
        {
            case CarAgentNavigationState.FollowPath:
                FollowPath();
                break;
            case CarAgentNavigationState.Stationary:
                Stationary();
                break;
            case CarAgentNavigationState.Backward:
                Backward();
                break;
            case CarAgentNavigationState.Avoid:
                AvoidAnotherAgent();
                break;
        }

        //for (int i = 0; i < path.corners.Length - 1; i++)
        //{
        //    Debug.DrawLine(path.corners[i], path.corners[i + 1], color);
        //}
    }

    private void RecalculatePath()
    {
        if (target != null)
        {
            if (NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            {
                currentCornerIndex = 1;
            }
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

        carController.MoveInput(1f);
        carController.SteerInput(CalculateSteer());
    }

    private void Stationary()
    {
        currentStationaryTime += Time.deltaTime;
        carController.MoveInput(0f);
        carController.SteerInput(0f);

        if (currentStationaryTime >= stationaryDuration)
        {
            carAgentNavigationState = CarAgentNavigationState.Backward;
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
            carAgentNavigationState = CarAgentNavigationState.FollowPath;
            currentBackwardTime = 0f;
            RecalculatePath();
        }
    }

    private void AvoidAnotherAgent()
    {
        currentAvoidTime += Time.deltaTime;

        Vector3 localAvoidDir = transform.InverseTransformDirection(avoidanceDirection);
        float steer = Mathf.Clamp(localAvoidDir.x, -1f, 1f);

        carController.MoveInput(1f);
        carController.SteerInput(steer);

        if (currentAvoidTime >= avoidDuration)
        {
            carAgentNavigationState = CarAgentNavigationState.FollowPath;
            currentAvoidTime = 0f;
        }

        CheckForStuck();
    }

    private float CalculateSteer()
    {
        if (currentCornerIndex >= path.corners.Length)
            return 0f;

        Vector3 corner = path.corners[currentCornerIndex];
        Vector3 direction = corner - transform.position;
        float distance = direction.magnitude;

        if (distance < reachThreshold && currentCornerIndex < path.corners.Length - 1)
        {
            currentCornerIndex++;
        }

        Vector3 localDir = transform.InverseTransformDirection(direction.normalized);
        return Mathf.Clamp(localDir.x, -1f, 1f);
    }

    private void CheckForStuck()
    {
        if (carRigidbody.linearVelocity.magnitude < minVelocityThreshold)
        {
            stuckTime += Time.deltaTime;
            if (stuckTime >= stuckDetectionTime)
            {
                carAgentNavigationState = CarAgentNavigationState.Stationary;
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
        Collider[] colliders = Physics.OverlapSphere(transform.position, avoidRadius, agentLayerMask);

        Collider closest = colliders
            .Where(c => c.gameObject != gameObject)
            .OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();

        if (closest != null)
        {
            avoidanceDirection = (transform.position - closest.transform.position).normalized;
            carAgentNavigationState = CarAgentNavigationState.Avoid;
            currentAvoidTime = 0f;
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRadius);

        if (carAgentNavigationState == CarAgentNavigationState.Avoid)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + avoidanceDirection * 3f);
        }
    }
}
using UnityEngine;

public class Aiming : MonoBehaviour
{
    public static Aiming Instance { get; private set; }

    [SerializeField]
    private LayerMask groundMask;
    
    [SerializeField] 
    private Transform pointer;
    public Vector3 PointerPosition => pointer.position;

    private Vector3 lastMousePosition = Vector3.zero;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.mousePosition == lastMousePosition) return;
        lastMousePosition = Input.mousePosition;

        Aim();
    }

    private void Aim()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, float.MaxValue, groundMask))
        {
            pointer.position = hitInfo.point;
        }
    }
}
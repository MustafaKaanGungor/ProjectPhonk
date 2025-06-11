using UnityEngine;

public class Aiming:MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    public GameObject pointer;

    private Camera mainCamera;

    public Vector3 PointerPosition => pointer.transform.position;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Aim();
    }

    public void Aim()
    {
        var (success, position) = GetMousePosition();

        if (success)
        {
            pointer.transform.position = position;
            Debug.DrawLine(transform.position, position, Color.red);
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }
}
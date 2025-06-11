using UnityEngine;
using UnityEngine.Pool;

public class Vision : MonoBehaviour
{
    public Color color;
    public Vector3 boxSize = new Vector3(10f, 10f, 10f);
    public LayerMask pointerLayerMask;
    public Aiming aiming;

    public Projectile projectilePrefab;

    private ObjectPool<Projectile> projectilePool;

    private void Start()
    {
        if (projectilePrefab != null)
        {
            projectilePool = new ObjectPool<Projectile>(
               createFunc: () =>
               {
                   Projectile projectile = Instantiate(projectilePrefab);
                   projectile.gameObject.GetComponent<TrailRenderer>().endColor = color;
                   projectile.targetTag = "Untagged";
                   projectile.OnLifeTimeEnd = () =>
                   {
                       projectilePool.Release(projectile);
                   };
                   return projectile;
               },
               actionOnGet: projectile =>
               {
                   Vector3 direction = (aiming.PointerPosition - aiming.transform.position).normalized;
                   projectile.Initialize(aiming.transform.position, direction);
               },
               actionOnRelease: projectile => projectile.gameObject.SetActive(false),
               actionOnDestroy: projectile => Destroy(projectile),
               collectionCheck: false,
               defaultCapacity: 10,
               maxSize: 20
            );
        }
    }

    private void Update()
    {
        if (projectilePrefab == null) return;

        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize / 2, transform.rotation, pointerLayerMask);

        if (colliders.Length != 0 && Input.GetMouseButtonDown(0))
        {
                projectilePool.Get();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, boxSize);
    }
}
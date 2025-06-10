using UnityEngine;
using UnityEngine.Pool;

public class CarShooter : MonoBehaviour
{
    public LayerMask targetLayerMask;
    public Transform target;
    public Projectile projectilePrefab;

    public float fireRate = 2.5f;
    public float fireRange = 5.0f;

    private float fireTimer = 0f;

    private ObjectPool<Projectile> projectilePool;

    private void Start()
    {
        projectilePool = new ObjectPool<Projectile>(
           createFunc: () =>
           {
               Projectile projectile = Instantiate(projectilePrefab);
               projectile.OnLifeTimeEnd = () =>
               {
                   projectilePool.Release(projectile);
               };
               return projectile;
           },
           actionOnGet: projectile =>
           {
               Vector3 direction = (target.position - transform.position).normalized;
               projectile.Initialize(transform.position, direction);
           },
           actionOnRelease: projectile => projectile.gameObject.SetActive(false),
           actionOnDestroy: projectile => Destroy(projectile),
           collectionCheck: false,
           defaultCapacity: 10,
           maxSize: 20
        );
    }

    private void Update()
    {
        bool hit = Physics.Raycast(transform.position, (target.position - transform.position).normalized, fireRange, targetLayerMask);
        fireTimer += Time.deltaTime;

        if (hit && fireTimer >= fireRate)
        {
            projectilePool.Get();
            fireTimer = 0f;
        }

        //Debug.DrawRay(transform.position, (target.position - transform.position).normalized * fireRange, Color.red);
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            //Gizmos.color = Color.red;
            //Gizmos.DrawLine(transform.position, target.position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, fireRange);
        }
    }
}

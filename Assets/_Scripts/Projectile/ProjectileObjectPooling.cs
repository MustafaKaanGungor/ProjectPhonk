using UnityEngine;
using UnityEngine.Pool;

public class ProjectileObjectPooling : MonoBehaviour
{
    [SerializeField]
    private Projectile projectilePrefab;

    private ObjectPool<Projectile> projectilePool;

    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnGetProjectile,
            actionOnRelease: OnReleaseProjectile,
            actionOnDestroy: OnDestroyProjectile,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 20
        );
    }

    private Projectile CreateProjectile()
    {
        Projectile projectile = Instantiate(projectilePrefab, transform);
        projectile.OnLifeTimeEnd = () =>
        {
            projectilePool.Release(projectile);
        };
        return projectile;
    }

    private void OnGetProjectile(Projectile projectile)
    {
        //projectile.gameObject.SetActive(true);
    }

    private void OnReleaseProjectile(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public Projectile GetProjectile()
    {
       return projectilePool.Get();
    }
}
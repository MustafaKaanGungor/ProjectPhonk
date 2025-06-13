using UnityEngine;

public class CarAgentShooter : Shooter
{
    [SerializeField]
    private LayerMask targetLayerMask;
    [SerializeField]
    private Transform target;

    private void Update()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        bool hit = Physics.Raycast(transform.position, direction, fireRange, targetLayerMask);

        Shoot(() =>
        {
            if (hit)
            {
                Projectile projectile = projectilePool.GetProjectile();
                projectile.Launch(transform.position, Quaternion.LookRotation(direction));
                fireTimer = 0;
            }
        });
    }

    private void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, fireRange);
        }
    }
}

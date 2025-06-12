using UnityEngine;
using UnityEngine.Pool;

public class Vision : Shooter
{
    [TagSelector]
    [SerializeField]
    private string pointerTag;

    private bool isShooting = false;
  
    private void Update()
    {
        Shoot(() =>
        {
            if (isShooting && Input.GetMouseButtonDown(0))
            {
                Vector3 direction = (Aiming.Instance.PointerPosition - transform.position).normalized;
                Projectile projectile = projectilePool.GetProjectile();
                projectile.Launch(transform.position, Quaternion.LookRotation(direction));
                fireTimer = 0;
            }
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(pointerTag))
        {
            isShooting = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(pointerTag))
        {
            isShooting = false;
        }
    }
}
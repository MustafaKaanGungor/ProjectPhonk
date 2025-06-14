using System;
using UnityEngine;

public abstract class Shooter : MonoBehaviour
{
    [SerializeField]
    protected ProjectileObjectPooling projectilePool;

    [SerializeField]
    protected float fireRate = 2.5f;
    [SerializeField]
    protected float fireRange = 5.0f;

    protected float fireTimer = 0f;

    protected void Shoot(Action shoot)
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            shoot?.Invoke();
        }
    }
}
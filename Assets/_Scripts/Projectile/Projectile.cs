using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [TagSelector]
    [SerializeField]
    private string targetTag;

    [SerializeField]
    protected Rigidbody rb;
    [SerializeField]
    protected TrailRenderer tr;

    [SerializeField]
    protected float damage = 20f;
    [SerializeField]
    protected float speed = 10f;
    [SerializeField]
    protected float maxLifetime = 5f;
    protected float currentLifetime;

    public Action OnLifeTimeEnd;

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= maxLifetime)
        {
            OnLifeTimeEnd?.Invoke();
        }
    }

    public void Launch(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
        tr.enabled = true;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward.normalized * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag) && other.TryGetComponent(out Health health))
        {
            health.TakeDamage(damage);

            OnLifeTimeEnd?.Invoke();
        }
    }

    private void OnDisable()
    {
        currentLifetime = 0f;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        tr.enabled = false;
        tr.Clear();
    }
}

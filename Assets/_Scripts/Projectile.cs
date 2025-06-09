using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody rb;
    private TrailRenderer tr;

    public float speed = 10f;
    public float maxLifetime = 5f;

    private float currentLifetime;
    private Action onLifeTimeEnd;

    public Action OnLifeTimeEnd
    {
        set
        {
            onLifeTimeEnd ??= value;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        tr = GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        rb.linearVelocity = Vector3.zero;
        tr.enabled = false;
        tr.Clear();
        currentLifetime = maxLifetime;
    }

    private void Update()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime <= 0f)
        {
            onLifeTimeEnd?.Invoke();
        }
    }

    public void Initialize(Vector3 startPosition, Vector3 direction)
    {
        transform.SetPositionAndRotation(startPosition, Quaternion.LookRotation(direction));
        tr.enabled = true;

        gameObject.SetActive(true);
        rb.AddForce(direction.normalized * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onLifeTimeEnd?.Invoke();
        }
    }
}

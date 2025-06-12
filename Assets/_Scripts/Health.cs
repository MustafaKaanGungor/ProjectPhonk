using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float CurrentHealth { get; private set; }

    private Material material;
    private Coroutine glowCoroutine;

    [SerializeField] private Color damageGlowColor = Color.red;
    [SerializeField] private float damageGlowMaxIntensity = 5f;
    [SerializeField] private float damageGlowDuration = 0.25f;

    private void Awake()
    {
        material = GetComponentInChildren<Renderer>().material;
    }

    private void Start()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;

        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
        }
        glowCoroutine = StartCoroutine(TakeDamageGlowEffect(damageGlowColor, damageGlowMaxIntensity, damageGlowDuration));

        if (CurrentHealth <= 0f)
        {
            StopCoroutine(glowCoroutine);
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator TakeDamageGlowEffect(Color glowColor, float maxIntensity, float duration)
    {
        material.EnableKeyword("_EMISSION");

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentIntensity = Mathf.Lerp(maxIntensity, 0f, elapsedTime / duration);
            material.SetColor("_EmissionColor", glowColor * currentIntensity);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        material.SetColor("_EmissionColor", Color.black);
        material.DisableKeyword("_EMISSION");
    }
}
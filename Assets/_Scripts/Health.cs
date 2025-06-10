using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }

        StartCoroutine(TakeDamageGlowEffect(Color.red, 5f, 0.25f));
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }

    IEnumerator TakeDamageGlowEffect(Color glowColor, float maxIntensity, float duration)
    {
        Material mat = GetComponentInChildren<Renderer>().material;

        mat.EnableKeyword("_EMISSION");

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float currentIntensity = Mathf.Lerp(maxIntensity, 0f, elapsedTime / duration);

            mat.SetColor("_EmissionColor", glowColor * currentIntensity);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
    }
}
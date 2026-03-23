using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    float currentHealth;

    public Slider healthSlider;

    [Header("Damage Feedback")]
    public Image damageImg;
    public float flashDuration = 0.15f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip hitClip;

    Coroutine flashRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();

        if (damageImg)
            damageImg.enabled = false;
    }

    void Update()
    {
        // ⭐ Test Damage
        if (Input.GetKeyDown(KeyCode.H))
            TakeDamage(10);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        UpdateUI();

        // Play Hit Sound
        if (audioSource && hitClip)
            audioSource.PlayOneShot(hitClip);

        //Flash Damage Image
        if (damageImg)
        {
            if (flashRoutine != null)
                StopCoroutine(flashRoutine);

            flashRoutine = StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        damageImg.enabled = true;

        yield return new WaitForSeconds(flashDuration);

        damageImg.enabled = false;
    }

    void UpdateUI()
    {
        if (healthSlider)
            healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log("Player Died");
        FindObjectOfType<GameManager>().PlayerDied();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    float currentHealth;

    public Slider healthSlider;

    GameManager gm;

    void Start()
    {
        currentHealth = maxHealth;

        gm = FindObjectOfType<GameManager>();

        if (gm != null)
            gm.RegisterEnemy();
        Debug.Log("Enemy Registered: " + gameObject.name);

        UpdateUI();
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        if (healthSlider)
            healthSlider.value = currentHealth / maxHealth;
    }

    void Die()
    {
        if (gm != null)
            gm.EnemyDied();

        Destroy(gameObject);
    }
}
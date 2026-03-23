using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 20f;
    public float lifeTime = 4f;

    [Header("Target Tag")]
    public string enemyTag = "Enemy";

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag(enemyTag))
        {
            EnemyHealth enemy =
                col.collider.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
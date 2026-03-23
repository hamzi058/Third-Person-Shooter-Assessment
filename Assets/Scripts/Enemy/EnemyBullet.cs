using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 12f;
    public float damage = 10f;

    void Start()
    {
        Destroy(gameObject, 4f);
    }

    void OnCollisionEnter(Collision col)
    {
        PlayerHealth player = col.gameObject.GetComponent<PlayerHealth>();

        if (player)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
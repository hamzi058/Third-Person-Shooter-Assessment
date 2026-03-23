using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public GameObject projectilePrefab;
    public Transform firePoint;

    Camera cam;

    public float shootForce = 20f;
    public float fireRate = 0.3f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootClip;

    float nextFireTime;

    void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (playerMovement.controlMode == PlayerMovement.ControlMode.PC)
        {
            if (Input.GetMouseButton(0))
            {
                TryShoot();
            }
        }
    }

    public void MobileShootButton()
    {
        TryShoot();
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        Shoot();
    }

    void Shoot()
    {
        Ray ray;

        // ⭐ PC → shoot where mouse cursor is
        if (playerMovement.controlMode == PlayerMovement.ControlMode.PC)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
        }
        else
        {
            // ⭐ Mobile → shoot from screen center
            ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        }

        Vector3 aimPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            aimPoint = hit.point;
        }
        else
        {
            aimPoint = ray.origin + ray.direction * 100f;
        }

        Vector3 shootDir = (aimPoint - firePoint.position).normalized;

        GameObject proj =
            Instantiate(projectilePrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDir));

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.linearVelocity = shootDir * shootForce;

        // ⭐ Shoot Audio
        if (audioSource && shootClip)
            audioSource.PlayOneShot(shootClip);
    }
}
using UnityEngine;

public class Tower : MonoBehaviour
{
    public GameObject projectilePrefab; // กระสุนที่ยิงออกไป
    public Transform firePoint;         // จุดยิงกระสุน
    public float attackRate = 1f;       // อัตราการยิงกระสุน
    private float attackCooldown = 0f;  // ตัวจับเวลา Cooldown
    private Transform target;           // เป้าหมายในระยะ

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (target != null && attackCooldown <= 0f)
        {
            Shoot();
            attackCooldown = 1f / attackRate; // รีเซ็ต Cooldown
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy")) // ตรวจจับศัตรู
        {
            target = collision.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform == target)
        {
            target = null;
        }
    }
}

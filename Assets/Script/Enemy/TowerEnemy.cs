using UnityEngine;

public class TowerEnemy : MonoBehaviour
{
    public int health = 100;               // พลังชีวิตของ Tower
    public float detectionRadius = 5f;     // ระยะการตรวจจับของ Tower
    public GameObject projectilePrefab;    // Prefab ของกระสุนที่ Tower ยิง
    public Transform firePoint;            // จุดที่กระสุนจะออกจาก Tower
    public float attackRate = 1f;          // อัตราการยิงกระสุน
    private float attackCooldown = 0f;     // ตัวจับเวลา Cooldown
    private bool isTargetInRange = false;  // ตัวแปรตรวจสอบว่ามีเป้าหมายในระยะหรือไม่

    void Update()
    {
        attackCooldown -= Time.deltaTime; // ลดเวลาการ Cooldown ของการยิง

        // หากมีเป้าหมายในระยะและยังไม่ได้ทำการยิง
        if (isTargetInRange && attackCooldown <= 0f)
        {
            Shoot();  // ยิงกระสุน
            attackCooldown = 1f / attackRate; // รีเซ็ต Cooldown
        }
    }

    void Shoot()
    {
        // ตรวจสอบว่า firePoint และ projectilePrefab ถูกตั้งค่าหรือไม่
        if (firePoint != null && projectilePrefab != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation); // ยิงกระสุน
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจจับว่า Tag คือ "Ranger" 
        if (collision.CompareTag("Ranger"))
        {
            // ตรวจสอบว่าตัวละครที่เข้ามาอยู่ในระยะ
            if (Vector2.Distance(transform.position, collision.transform.position) <= detectionRadius)
            {
                isTargetInRange = true; // ตั้งสถานะให้มีเป้าหมายในระยะ
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ถ้าเป้าหมายออกจากระยะให้ลบสถานะ
        if (collision.CompareTag("Ranger"))
        {
            isTargetInRange = false; // ไม่มีเป้าหมายในระยะ
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("TowerEnemy ถูกทำลาย!");
        GameManager.Instance.EndGame("TowerEnemy");
        Destroy(gameObject); // ทำลาย TowerEnemy
    }
}

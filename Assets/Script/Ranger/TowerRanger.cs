using UnityEngine;
using UnityEngine.UI;

public class TowerRanger : MonoBehaviour
{
    public int health = 100;               // พลังชีวิตของ Tower
    public float detectionRadius = 5f;     // ระยะการตรวจจับของ Tower
    public GameObject projectilePrefab;    // Prefab ของกระสุนที่ Tower ยิง
    public Transform firePoint;            // จุดที่กระสุนจะออกจาก Tower
    public float attackRate = 1f;          // อัตราการยิงกระสุน
    private float attackCooldown = 0f;     // ตัวจับเวลา Cooldown
    private bool isTargetInRange = false;  // ตัวแปรตรวจสอบว่ามีเป้าหมายในระยะหรือไม่

    public Slider hpBar; // Slider แสดงค่า HP
    
    void Start()
    {
        if (hpBar != null)
        {
            hpBar.maxValue = health;
            hpBar.value = health;
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>(); // หรือ CircleCollider2D ตามต้องการ
        }
        col.isTrigger = true; // ต้องเปิดให้เป็น Trigger

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = true; // ตั้งค่าเป็น Kinematic เพื่อไม่ให้มีผลกับฟิสิกส์
    }

    
    void Update()
    {
        attackCooldown -= Time.deltaTime; 

        if (isTargetInRange && attackCooldown <= 0f)
        {
            Shoot();
            attackCooldown = 1f / attackRate; 
        }
    }

    void Shoot()
    {
        if (firePoint != null && projectilePrefab != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (Vector2.Distance(transform.position, collision.transform.position) <= detectionRadius)
            {
                isTargetInRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            isTargetInRange = false;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (hpBar != null)
        {
            hpBar.value = health;
        }
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("TowerRanger ถูกทำลาย!");
        GameManager.Instance.EndGame("TowerRanger");
        Destroy(gameObject);
    }
}

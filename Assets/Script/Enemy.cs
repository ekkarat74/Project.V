using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 1.5f;          // ความเร็วการเคลื่อนที่
    public int health = 50;            // พลังชีวิต
    public GameObject projectilePrefab; // กระสุนที่ยิงออกไป
    public Transform firePoint;        // จุดยิงกระสุน
    public float attackRate = 1f;      // อัตราการยิงกระสุน
    private float attackCooldown = 0f; // ตัวจับเวลา Cooldown
    public float detectionRadius = 5f; // ระยะการตรวจจับ Ranger

    public int expValue = 20;          // ค่าประสบการณ์ที่ให้เมื่อศัตรูถูกทำลาย

    private Transform target;          // เป้าหมาย (Ranger)

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        // ค้นหา Ranger และ TowerRanger ในระยะ
        Collider2D detectedTarget = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Ranger", "TowerRanger"));
        if (detectedTarget != null)
        {
            target = detectedTarget.transform; // เก็บตำแหน่งของเป้าหมาย

            // ยิงเมื่อ Cooldown หมด
            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = 1f / attackRate; // รีเซ็ต Cooldown
            }
        }
        else
        {
            target = null; // ล้างเป้าหมายเมื่อไม่มี Ranger ในระยะ
            MoveForward();
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime); // เดินไปทางซ้าย
    }

    void Shoot()
    {
        if (target != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
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
        // เพิ่ม EXP ให้ Ranger เมื่อ Enemy ตาย
        Ranger ranger = FindObjectOfType<Ranger>();
        if (ranger != null)
        {
            ranger.GainExp(expValue);
        }
        Destroy(gameObject); // ทำลาย Enemy
    }

    void OnDrawGizmos()
    {
        // วาดระยะ detectionRadius เพื่อช่วยในการดีบัก
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

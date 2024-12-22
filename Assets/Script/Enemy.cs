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

    private bool isTargetInRange = false; // ตรวจสอบว่ามีเป้าหมายในระยะหรือไม่
    private Transform target;            // เก็บตำแหน่งของเป้าหมาย

    void Update()
    {
        attackCooldown -= Time.deltaTime; // ลดเวลารอการยิง

        if (isTargetInRange)
        {
            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = 1f / attackRate; // รีเซ็ต Cooldown
            }
        }
        else
        {
            MoveForward(); // ถ้าไม่มีเป้าหมายในระยะ ให้เดินต่อ
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime); // เดินไปทางซ้าย
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ranger"))
        {
            isTargetInRange = true;
            target = collision.transform; // เก็บเป้าหมาย
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ranger"))
        {
            isTargetInRange = false;
            target = null; // ล้างเป้าหมาย
        }
    }

    void Shoot()
    {
        if (target != null) // ยิงเฉพาะเมื่อมีเป้าหมาย
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation); // ยิงกระสุน
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
        // ค้นหา Ranger และเพิ่ม EXP
        Ranger ranger = FindObjectOfType<Ranger>();
        if (ranger != null)
        {
            ranger.GainExp(expValue); // เพิ่ม EXP ให้ Ranger
        }
        Destroy(gameObject); // ทำลาย Enemy
    }
}
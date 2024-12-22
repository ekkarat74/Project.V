using UnityEngine;

public class Ranger : MonoBehaviour
{
    public float speed = 2f;             // ความเร็วการเคลื่อนที่
    public int health = 100;            // พลังชีวิต
    public GameObject projectilePrefab; // กระสุนที่ยิงออกไป
    public Transform firePoint;         // จุดยิงกระสุน
    public float attackRate = 1f;       // อัตราการยิงกระสุน
    private float attackCooldown = 0f;  // ตัวจับเวลา Cooldown
    public float detectionRadius = 5f;  // ระยะการตรวจจับศัตรู

    public int mineralCost = 50;        // จำนวนแร่ที่ต้องใช้ในการผลิต Ranger

    // ระบบเลเวล
    public int level = 1;               // เลเวลเริ่มต้น
    public int currentExp = 0;          // ค่าประสบการณ์ปัจจุบัน
    public int expToNextLevel = 100;    // EXP ที่ต้องใช้สำหรับเลื่อนเลเวล
    public int bonusHealthPerLevel = 20; // พลังชีวิตที่เพิ่มต่อเลเวล

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
        transform.Translate(Vector2.right * speed * Time.deltaTime); // เดินไปทางขวา
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            isTargetInRange = true;
            target = collision.transform; // เก็บเป้าหมาย
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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

    public void GainExp(int exp)
    {
        currentExp += exp;
        if (currentExp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        currentExp -= expToNextLevel;   // ยกยอด EXP ที่เกิน
        expToNextLevel += 50;          // เพิ่ม EXP ที่ต้องการสำหรับเลเวลถัดไป
        health += bonusHealthPerLevel; // เพิ่มพลังชีวิตเมื่อเลเวลอัพ
        Debug.Log($"Ranger เลเวลอัพ! เลเวล: {level}");
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
        Destroy(gameObject); // ทำลายตัวเองเมื่อพลังชีวิตหมด
    }
}
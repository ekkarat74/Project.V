using UnityEngine;
using System.Linq; // สำหรับการค้นหา Ranger ที่มีพลังชีวิตต่ำที่สุด

public class Enemy : MonoBehaviour
{
    [Header("Basic Attributes")]
    public float speed = 1.5f;          // ความเร็วการเคลื่อนที่
    public int health = 50;            // พลังชีวิต
    
    [Header("Attack System")]
    public GameObject projectilePrefab; // กระสุนที่ยิงออกไป
    public Transform firePoint;        // จุดยิงกระสุน
    public float attackRate = 1f;      // อัตราการยิงกระสุน (ยิงต่อวินาที)
    private float attackCooldown = 0f; // ตัวจับเวลา Cooldown
    public float detectionRadius = 5f; // ระยะการตรวจจับ Ranger

    [Header("EXP")]
    public int expValue = 20;          // ค่าประสบการณ์ที่ให้เมื่อศัตรูถูกทำลาย

    private Transform target;          // เป้าหมาย (Ranger)

    // Enum สำหรับเลือกประเภทการโจมตี
    public enum AttackMode
    {
        TargetLowestHealth, // เลือกโจมตี Ranger ที่มีพลังชีวิตต่ำที่สุด
        NearestRanger,      // เลือกโจมตี Ranger ที่อยู่ใกล้ที่สุด
        NormalAttack        // โจมตีปกติ
    }

    public AttackMode currentAttackMode = AttackMode.NormalAttack; // โหมดโจมตีเริ่มต้น

    void Update()
    {
        attackCooldown -= Time.deltaTime; // ลด cooldown ทุกๆ frame

        // ค้นหา Ranger และ TowerRanger ในระยะ
        Collider2D[] detectedTargets = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Ranger", "TowerRanger"));

        if (detectedTargets.Length > 0)
        {
            // เลือกเป้าหมายตามโหมดการโจมตี
            target = SelectTarget(detectedTargets);

            // เพิ่มความเร็วการโจมตีถ้าเป็น NearestRanger
            IncreaseAttackSpeed();

            // ยิงกระสุนเมื่อ attackCooldown ถึง 0 หรือหมด
            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = 1f / attackRate; // รีเซ็ต cooldown ตาม attackRate
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
            // ยิงกระสุน
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    // ฟังก์ชันเลือกเป้าหมาย
    Transform SelectTarget(Collider2D[] detectedTargets)
    {
        switch (currentAttackMode)
        {
            case AttackMode.TargetLowestHealth:
                return detectedTargets
                    .Select(target => target.transform)
                    .OrderBy(target => target.GetComponent<Ranger>().health) // ค้นหาที่มีพลังชีวิตต่ำที่สุด
                    .FirstOrDefault();
            case AttackMode.NearestRanger:
                return detectedTargets
                    .Select(target => target.transform)
                    .OrderBy(target => Vector2.Distance(transform.position, target.position)) // ค้นหาที่ใกล้ที่สุด
                    .FirstOrDefault();
            case AttackMode.NormalAttack:
            default:
                return detectedTargets[0].transform; // โจมตีเป้าหมายแรก
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

    // ฟังก์ชันที่ใช้สำหรับเพิ่มความเร็วโจมตีเมื่อเลือก NearestRanger
    void IncreaseAttackSpeed()
    {
        if (currentAttackMode == AttackMode.NearestRanger)
        {
            attackRate = 2f; // เพิ่มอัตราการโจมตีเป็นสองเท่า
        }
        else
        {
            attackRate = 1f; // ค่าเริ่มต้น (ปกติ)
        }
    }
}

using UnityEngine;

public class Ranger : MonoBehaviour
{
    // คุณสมบัติพื้นฐานของ Ranger
    [Header("Basic Attributes")]
    public float speed = 2f;                 // ความเร็วการเคลื่อนที่
    public int health = 100;                // พลังชีวิต

    // ระบบการโจมตี
    [Header("Attack System")]
    public GameObject projectilePrefab;     // กระสุนที่ใช้ยิง
    public Transform firePoint;             // ตำแหน่งที่ยิงกระสุน
    public float attackRate = 1f;           // อัตราการยิงต่อวินาที
    private float attackCooldown = 0f;      // คูลดาวน์สำหรับการยิง

    // ระบบตรวจจับศัตรู
    [Header("Detection System")]
    public float detectionRadius = 5f;      // ระยะตรวจจับศัตรู

    // ระบบสร้างยูนิต
    [Header("Unit Spawn System")]
    public float spawnCooldown = 5f;        // คูลดาวน์ในการสร้างยูนิต
    public int mineralCost = 50;            // ต้นทุนแร่ในการสร้างยูนิต

    // ระบบเลเวล
    [Header("Level System")]
    public int level = 1;                   // เลเวลปัจจุบัน
    public int currentExp = 0;              // ค่าประสบการณ์ปัจจุบัน
    public int expToNextLevel = 100;        // EXP ที่ต้องการสำหรับเลเวลถัดไป
    public int bonusHealthPerLevel = 20;    // โบนัสพลังชีวิตต่อเลเวล

    private Transform target;               // เป้าหมายปัจจุบัน

    // คุณสมบัติของกระสุน
    [Header("Projectile Properties")]
    public float projectileSpeed = 5f;      // ความเร็วของกระสุน
    public int projectileDamage = 10;       // ความเสียหายของกระสุน
    public float projectileLifetime = 3f;   // อายุของกระสุนก่อนทำลายตัวเอง

    void Update()
    {
        attackCooldown -= Time.deltaTime;

        // ค้นหาเป้าหมายในระยะ
        Collider2D detectedTarget = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Enemy", "TowerEnemy"));
        if (detectedTarget != null)
        {
            target = detectedTarget.transform;
            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = 1f / attackRate;
            }
        }
        else
        {
            target = null;
            MoveForward();
        }
    }

    void MoveForward()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void Shoot()
    {
        if (target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            ProjectileBehavior projectileBehavior = projectile.AddComponent<ProjectileBehavior>();
            projectileBehavior.Initialize(target, projectileSpeed, projectileDamage, projectileLifetime);
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
        currentExp -= expToNextLevel;
        expToNextLevel += 50;
        health += bonusHealthPerLevel;
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
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}

public class ProjectileBehavior : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;
    private float lifetime;

    public void Initialize(Transform target, float speed, int damage, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.lifetime = lifetime;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("TowerEnemy"))
        {
            if (collision.CompareTag("Enemy"))
            {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }
            else if (collision.CompareTag("TowerEnemy"))
            {
                TowerEnemy towerEnemy = collision.GetComponent<TowerEnemy>();
                if (towerEnemy != null)
                {
                    towerEnemy.TakeDamage(damage);
                }
            }

            Destroy(gameObject);
        }
    }
}
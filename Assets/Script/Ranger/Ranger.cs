using UnityEngine;
using System.Linq;

public class Ranger : MonoBehaviour
{
    public enum SkillType
    {
        None,
        PowerShot,      // สกิล 1: ยิงดาเมจแรงขึ้น
        ExplosiveShot,  // สกิล 2: ยิงกระสุนแบบดาเมจวงกว้าง
        RapidFire,      // สกิล 3: ยิงเร็วขึ้นเมื่อศัตรูเข้าใกล้
        HealRanger      // สกิล 4: ฮีล Ranger ตัวอื่นใน Scene
    }

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

    private static RangerUIController uiController;

    [Header("Ranger Type")]
    public int rangerTypeIndex; // ประเภทของ Ranger (Index สำหรับเลือก UI)

    [Header("Skill System")]
    public SkillType currentSkill = SkillType.None;  // สกิลปัจจุบัน
    
    [Header("Damage Multiplier")]
    public float skillRangeMultiplier = 1.5f;       // ตัวคูณระยะของสกิล PowerShot
    public int skillDamageMultiplier = 2;           // ตัวคูณดาเมจของสกิล PowerShot
    
    [Header("Explosive Shot Properties")]
    public float explosiveRadius = 3f; // รัศมีการระเบิดของกระสุน
    public int explosiveDamage = 20;   // ดาเมจจากการระเบิด
    
    [Header("Rapid Fire Properties")]
    public float rapidFireMultiplier = 2f; // ตัวคูณอัตราการยิงเร็วขึ้น
    
    [Header("Heal Ranger Properties")]
    public int healAmount = 50;            // จำนวนที่ Heal
    public float healThreshold = 0.2f;     // เปอร์เซ็นต์พลังชีวิตต่ำสุดที่ต้อง Heal

    void Start()
    {
        if (uiController == null)
        {
            uiController = FindObjectOfType<RangerUIController>();
        }

        // เพิ่ม UI ของ Ranger โดยระบุ Index
        uiController?.AddRangerUI(this, rangerTypeIndex);
    }
    
    void Update()
    {
        attackCooldown -= Time.deltaTime;

        // ตรวจสอบสกิล RapidFire
        if (currentSkill == SkillType.RapidFire)
        {
            Collider2D nearbyEnemy = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Enemy", "TowerEnemy"));
            if (nearbyEnemy != null)
            {
                attackRate *= rapidFireMultiplier; // เพิ่มอัตราการยิง
            }
        }

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
            switch (currentSkill)
            {
                case SkillType.PowerShot:
                    ShootPowerShot();
                    break;
                case SkillType.ExplosiveShot:
                    ShootExplosiveShot();
                    break;
                case SkillType.HealRanger:
                    HealLowHealthRanger();
                    break;
                default:
                    ShootNormal();
                    break;
            }
        }
    }

    void ShootNormal()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ProjectileBehavior projectileBehavior = projectile.AddComponent<ProjectileBehavior>();
        projectileBehavior.Initialize(target, projectileSpeed, projectileDamage, projectileLifetime);
    }

    void ShootPowerShot()
    {
        if (Vector2.Distance(transform.position, target.position) <= detectionRadius * skillRangeMultiplier)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            ProjectileBehavior projectileBehavior = projectile.AddComponent<ProjectileBehavior>();
            projectileBehavior.Initialize(target, projectileSpeed, projectileDamage * skillDamageMultiplier, projectileLifetime);
        }
    }
    
    void ShootExplosiveShot()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        ExplosiveProjectileBehavior projectileBehavior = projectile.AddComponent<ExplosiveProjectileBehavior>();
        projectileBehavior.Initialize(target, projectileSpeed, explosiveDamage, explosiveRadius, projectileLifetime);
    }
    
    void HealLowHealthRanger()
    {
        Ranger[] rangers = FindObjectsOfType<Ranger>();
        foreach (Ranger ranger in rangers)
        {
            if (ranger != this && ranger.health <= ranger.health * healThreshold)
            {
                ranger.health += healAmount;
                if (ranger.health > 100) // สมมติว่า Max Health คือ 100
                {
                    ranger.health = 100;
                }
                Debug.Log($"Ranger {ranger.name} ได้รับการ Heal {healAmount} HP");
                break; // Heal แค่ตัวเดียวในรอบ
            }
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
        // ลบ UI เมื่อ Ranger ตาย
        uiController?.RemoveRangerUI(this);
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
        else
        {
            // ค้นหาเป้าหมายใหม่เมื่อเป้าหมายเดิมหายไป
            FindNewTarget();
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

    private void FindNewTarget()
    {
        Collider2D[] detectedTargets = Physics2D.OverlapCircleAll(transform.position, 5f, LayerMask.GetMask("Enemy", "TowerEnemy"));
        if (detectedTargets.Length > 0)
        {
            target = detectedTargets
                .Select(c => c.transform)
                .OrderBy(t => Vector2.Distance(transform.position, t.position))
                .FirstOrDefault();
        }

        if (target == null)
        {
            // หากไม่มีเป้าหมายใหม่ ทำลายกระสุน
            Destroy(gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5f); // ใช้แสดงระยะค้นหาเป้าหมาย
    }
}
public class ExplosiveProjectileBehavior : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;
    private float radius;
    private float lifetime;

    public void Initialize(Transform target, float speed, int damage, float radius, float lifetime)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.radius = radius;
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
            Explode();
        }
    }

    private void Explode()
    {
        // ตรวจสอบศัตรูในรัศมีการระเบิด
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy", "TowerEnemy"));

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Enemy e = enemy.GetComponent<Enemy>();
                e?.TakeDamage(damage);
            }
            else if (enemy.CompareTag("TowerEnemy"))
            {
                TowerEnemy te = enemy.GetComponent<TowerEnemy>();
                te?.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        // แสดงรัศมีระเบิดใน Editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
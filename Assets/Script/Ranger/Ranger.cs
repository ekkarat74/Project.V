using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Ranger : MonoBehaviour
{
    public enum SkillType
    {
        None,
        PowerShot,      // สกิล 1: ยิงดาเมจแรงขึ้น
        ExplosiveShot,  // สกิล 2: ยิงกระสุนแบบดาเมจวงกว้าง
        RapidFire,      // สกิล 3: ยิงเร็วขึ้นเมื่อศัตรูเข้าใกล้
    }

    public enum RangerType
    {
        None,
        Agile,   // ว่องไว
        Durable, // ทนทาน
        Power    // พลัง
    }
    
    // ประเภทของ Ranger
    [Header("Ranger Type")]
    public RangerType rangerType;
    
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

    [Header("UI Components")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;  // เพิ่มตัวแสดงค่าตัวเลข HP
    public Image hpFill;  // อ้างอิงไปที่ Fill Area ของ Slider
    
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;
    private Transform target;               // เป้าหมายปัจจุบัน

    // คุณสมบัติของกระสุน
    [Header("Projectile Properties")]
    public float projectileSpeed = 5f;      // ความเร็วของกระสุน
    [Range(100, 200)]                      // เพิ่มตัวควบคุมช่วงใน Inspector
    public int projectileDamage = 100;     // ความเสียหายของกระสุน
    public float projectileLifetime = 3f;  // อายุของกระสุนก่อนทำลายตัวเอง

    [Header("Critical Hit System")]
    public float criticalChance = 0.1f; // โอกาสในการคริติคอล 
    public float criticalMultiplier = 2f; // คูณดาเมจเมื่อคริติคอล
    
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

    private float lastHealTime = -Mathf.Infinity; // เก็บเวลาครั้งสุดท้ายที่ทำการ Heal

    [Header("Equipment System")]
    public GameObject[] equipmentPrefabs; // อุปกรณ์ที่ติดตั้งบน Ranger
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (uiController == null)
        {
            uiController = FindObjectOfType<RangerUIController>();
        }

        uiController?.AddRangerUI(this, rangerTypeIndex);

        ApplyEquipmentStats();
        ApplyRangerTypeStats();

        // ตั้งค่า HP Slider
        if (hpSlider != null)
        {
            hpSlider.maxValue = health;
            hpSlider.value = health;
        }
        
        if (hpText != null)
        {
            hpText.text = health.ToString();
        }
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

        // สุ่มดาเมจในช่วง 100-200
        int randomizedDamage = Random.Range(100, 201);

        // ตรวจสอบคริติคอล
        if (Random.value <= criticalChance)
        {
            randomizedDamage = Mathf.RoundToInt(randomizedDamage * criticalMultiplier);
            Debug.Log($"Critical Hit! Damage: {randomizedDamage}");
        }
        else
        {
            Debug.Log($"Projectile Damage: {randomizedDamage}");
        }

        projectileBehavior.Initialize(target, projectileSpeed, randomizedDamage, projectileLifetime);
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

        if (hpSlider != null)
        {
            hpSlider.maxValue = health;
            hpSlider.value = health;
        }

        if (hpText != null)
        {
            hpText.text = health.ToString();
        }

        Debug.Log($"Ranger เลเวลอัพ! เลเวล: {level}");
    }
    
    //Use ITEM
    void ApplyEquipmentStats()
    {
        // ตรวจสอบว่ามีอุปกรณ์ติดตั้งหรือไม่
        if (equipmentPrefabs != null && equipmentPrefabs.Length > 0)
        {
            foreach (GameObject equipmentPrefab in equipmentPrefabs)
            {
                if (equipmentPrefab != null)
                {
                    // เพิ่มค่าความเร็ว
                    if (equipmentPrefab.TryGetComponent(out SpeedBooster speedBooster))
                    {
                        speed += speedBooster.speedBonus;
                    }

                    // เพิ่มอัตราการยิง
                    if (equipmentPrefab.TryGetComponent(out AttackRateBooster attackRateBooster))
                    {
                        attackRate += attackRateBooster.attackRateBonus;
                    }

                    // เพิ่มระยะตรวจจับ
                    if (equipmentPrefab.TryGetComponent(out DetectionRadiusBooster detectionRadiusBooster))
                    {
                        detectionRadius += detectionRadiusBooster.detectionRadiusBonus;
                    }
                    
                    //เพิ่มดาเมจ
                    if (equipmentPrefab.TryGetComponent(out DamageBooster damageBooster))
                    {
                        projectileDamage += damageBooster.damage;
                    }
                }
            }
        }

        // ล็อกค่าหลังจากเซ็ตอุปกรณ์
        Debug.Log($"Ranger Stats - Speed: {speed}, Attack Rate: {attackRate}, Detection Radius: {detectionRadius}");
    }
    
    void ApplyRangerTypeStats()
    {
        switch (rangerType)
        {
            case RangerType.Agile: // ว่องไว
                speed += 1.5f; // เพิ่มความเร็ว
                attackRate += 0.5f; // เพิ่มอัตราการยิง
                break;

            case RangerType.Durable: // ทนทาน
                health += 50; // เพิ่มพลังชีวิต
                expToNextLevel -= 20; // อัพเลเวลเร็วขึ้น
                break;

            case RangerType.Power: // พลัง
                projectileDamage += 20; // เพิ่มดาเมจของกระสุน
                criticalChance += 0.1f; // เพิ่มโอกาสคริติคอล
                break;
        }
    }
    
    void UpdateHPBar()
    {
        if (hpFill != null)
        {
            float hpPercentage = health / hpSlider.maxValue;
            if (hpPercentage > 0.6f)
                hpFill.color = Color.green;
            else if (hpPercentage > 0.3f)
                hpFill.color = Color.yellow;
            else
                hpFill.color = Color.red;
        }
    }
    
    IEnumerator BlinkHPBar()
    {
        while (health > 0 && health / hpSlider.maxValue < 0.3f)
        {
            hpFill.enabled = !hpFill.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        hpFill.enabled = true;
        isBlinking = false;
    }
    
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (hpSlider != null)
        {
            hpSlider.value = health;
        }

        if (hpText != null)
        {
            hpText.text = health.ToString();
        }
        
        UpdateHPBar();
        StartCoroutine(FlashRed()); 
        
        if (health / hpSlider.maxValue < 0.3f && !isBlinking)
        {
            isBlinking = true;
            StartCoroutine(BlinkHPBar());
        }
        
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
        if (target == null || !target.gameObject.activeInHierarchy) // ตรวจสอบว่าเป้าหมายถูกทำลายหรือไม่
        {
            FindNewTarget();
        }

        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject); // ถ้าไม่มีเป้าหมายใหม่ให้ทำลายตัวเอง
        }
    }

    private void FindNewTarget()
    {
        Collider2D[] detectedTargets = Physics2D.OverlapCircleAll(transform.position, 5f, LayerMask.GetMask("Enemy", "TowerEnemy"));
        target = detectedTargets
            .Where(c => c != null && c.gameObject.activeInHierarchy) // กรองเฉพาะเป้าหมายที่ยังอยู่ในเกม
            .Select(c => c.transform)
            .OrderBy(t => Vector2.Distance(transform.position, t.position))
            .FirstOrDefault();
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

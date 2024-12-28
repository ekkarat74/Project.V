using UnityEngine;

public class RangerProjectile : MonoBehaviour
{
    public float speed = 5f;     // ความเร็วกระสุน
    public int damage = 10;      // ความเสียหายของกระสุน
    public float lifetime = 3f;  // ระยะเวลาที่กระสุนจะมีชีวิต
    private Transform target;    // ตัวแปรที่เก็บตำแหน่งของเป้าหมาย

    void Start()
    {
        // หาตำแหน่งของ Ranger หรือ TowerRanger ที่มี tag "Ranger" หรือ "TowerRanger"
        GameObject[] rangerObjects = GameObject.FindGameObjectsWithTag("Ranger");
        GameObject[] towerRangerObjects = GameObject.FindGameObjectsWithTag("TowerRanger");

        // รวมทั้งสองกลุ่มและเลือกเป้าหมาย (ใกล้ที่สุดหรืออื่นๆ)
        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var obj in rangerObjects)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = obj;
            }
        }

        foreach (var obj in towerRangerObjects)
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = obj;
            }
        }

        // หากพบเป้าหมาย, กำหนด target
        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }

        Destroy(gameObject, lifetime); // ทำลายกระสุนหลังจาก 3 วินาที
    }

    void Update()
    {
        if (target != null)
        {
            // คำนวณทิศทางไปยัง target
            Vector2 direction = (target.position - transform.position).normalized;

            // เคลื่อนที่กระสุนไปในทิศทางของ target
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ranger") || collision.CompareTag("TowerRanger")) // โจมตี Ranger หรือ TowerRanger
        {
            // ใช้ฟังก์ชัน TakeDamage() ของ Ranger หรือ TowerRanger
            if (collision.CompareTag("Ranger"))
            {
                Ranger ranger = collision.GetComponent<Ranger>();
                if (ranger != null)
                {
                    ranger.TakeDamage(damage);
                }
            }
            else if (collision.CompareTag("TowerRanger"))
            {
                TowerRanger towerRanger = collision.GetComponent<TowerRanger>();
                if (towerRanger != null)
                {
                    towerRanger.TakeDamage(damage);
                }
            }

            Destroy(gameObject); // ทำลายกระสุน
        }
    }
}

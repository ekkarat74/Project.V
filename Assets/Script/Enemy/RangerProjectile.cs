using UnityEngine;
using System.Linq;

public class RangerProjectile : MonoBehaviour
{
    public float speed = 5f;     // ความเร็วกระสุน
    public int damage = 10;      // ความเสียหายของกระสุน
    public float lifetime = 3f;  // ระยะเวลาที่กระสุนจะมีชีวิต
    private Transform target;    // ตัวแปรที่เก็บตำแหน่งของเป้าหมาย

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<CircleCollider2D>(); // หรือ BoxCollider2D ตามต้องการ
        }
        col.isTrigger = true; // ต้องเปิดให้เป็น Trigger

        FindNewTarget();
        Destroy(gameObject, lifetime); // ทำลายกระสุนหลังจาก 3 วินาที
    }

    void Update()
    {
        if (target == null || !target.gameObject.activeInHierarchy) 
        {
            FindNewTarget();
        }

        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void FindNewTarget()
    {
        GameObject[] rangerObjects = GameObject.FindGameObjectsWithTag("Ranger");
        GameObject[] towerRangerObjects = GameObject.FindGameObjectsWithTag("TowerRanger");

        GameObject closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var obj in rangerObjects.Concat(towerRangerObjects))
        {
            float distance = Vector2.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = obj;
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ranger") || collision.CompareTag("TowerRanger")) 
        {
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

            Destroy(gameObject); // ทำลายกระสุนเมื่อชนเป้าหมาย
        }
    }
}
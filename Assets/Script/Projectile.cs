using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 5f;     // ความเร็วกระสุน
    public int damage = 10;      // ความเสียหายของกระสุน

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime); // กระสุนเคลื่อนที่
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ranger")) // โจมตี Ranger
        {
            collision.GetComponent<Ranger>().TakeDamage(damage);
            Destroy(gameObject); // ทำลายกระสุน
        }
        else if (collision.CompareTag("Enemy")) // โจมตี Enemy
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            Destroy(gameObject); // ทำลายกระสุน
        }
    }
}
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // อาเรย์สำหรับเก็บ Prefab ของ Enemy
    public Transform spawnPoint;      // ตำแหน่งที่ใช้ Spawn
    public float spawnInterval = 3f;  // เวลาระหว่างการ Spawn
    private float timer = 0f;        // ตัวจับเวลา

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, enemyPrefabs.Length); // เลือก Prefab แบบสุ่ม
            Instantiate(enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }
}
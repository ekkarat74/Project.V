using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class WaveSettings
    {
        public GameObject[] enemyPrefabs; // อาเรย์สำหรับเก็บ Prefab ของ Enemy
        public float spawnInterval = 1f;  // เวลาระหว่างการ Spawn ศัตรูในแต่ละ Wave
        public int enemiesPerWave = 5;    // จำนวนศัตรูในแต่ละ Wave
    }

    public WaveSettings[] waves;         // การตั้งค่าของแต่ละ Wave
    public Transform spawnPoint;         // ตำแหน่งที่ใช้ Spawn
    public float waveInterval = 5f;      // เวลาพักระหว่าง Wave

    private float spawnTimer = 0f;       // ตัวจับเวลาสำหรับการ Spawn
    private float waveTimer = 0f;        // ตัวจับเวลาสำหรับ Wave
    private int enemiesSpawned = 0;     // จำนวนศัตรูที่ Spawn แล้วใน Wave ปัจจุบัน
    private int currentWaveIndex = 0;   // ดัชนีของ Wave ปัจจุบัน
    private bool isWaveActive = true;   // สถานะว่า Wave กำลังทำงานอยู่หรือไม่

    void Update()
    {
        if (currentWaveIndex >= waves.Length)
            return; // หากหมด Wave ทั้งหมด หยุดการทำงาน

        if (isWaveActive)
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= waves[currentWaveIndex].spawnInterval && enemiesSpawned < waves[currentWaveIndex].enemiesPerWave)
            {
                SpawnEnemy();
                spawnTimer = 0f;
                enemiesSpawned++;
            }

            if (enemiesSpawned >= waves[currentWaveIndex].enemiesPerWave)
            {
                isWaveActive = false;
                waveTimer = 0f; // รีเซ็ตตัวจับเวลา Wave
            }
        }
        else
        {
            waveTimer += Time.deltaTime;

            if (waveTimer >= waveInterval)
            {
                StartNewWave();
            }
        }
    }

    void SpawnEnemy()
    {
        if (waves[currentWaveIndex].enemyPrefabs.Length > 0)
        {
            int randomIndex = Random.Range(0, waves[currentWaveIndex].enemyPrefabs.Length); // เลือก Prefab แบบสุ่ม
            Instantiate(waves[currentWaveIndex].enemyPrefabs[randomIndex], spawnPoint.position, spawnPoint.rotation);
        }
    }

    void StartNewWave()
    {
        enemiesSpawned = 0; // รีเซ็ตจำนวนศัตรูที่ Spawn แล้ว
        isWaveActive = true; // เปิดใช้งาน Wave ใหม่
        currentWaveIndex++; // ไปยัง Wave ถัดไป
    }
}

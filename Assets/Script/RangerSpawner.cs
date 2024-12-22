using UnityEngine;

public class RangerSpawner : MonoBehaviour
{
    public GameObject[] rangerPrefabs; // อาเรย์สำหรับเก็บ Prefab ของ Ranger
    public Transform spawnPoint;      // ตำแหน่งที่ใช้ Spawn
    public MineralSystem mineralSystem; // อ้างอิงระบบแร่

    public void SpawnRanger(int index)
    {
        if (index >= 0 && index < rangerPrefabs.Length)
        {
            Ranger ranger = rangerPrefabs[index].GetComponent<Ranger>();
            if (ranger != null && mineralSystem.SpendMinerals(ranger.mineralCost))
            {
                Instantiate(rangerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"สร้าง Ranger (ใช้แร่ {ranger.mineralCost})");
            }
        }
        else
        {
            Debug.LogWarning("Invalid Ranger index!");
        }
    }
}
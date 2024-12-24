using UnityEngine;
using UnityEngine.UI;

public class RangerSpawner : MonoBehaviour
{
    public GameObject[] rangerPrefabs;   // อาเรย์สำหรับเก็บ Prefab ของ Ranger
    public Transform spawnPoint;        // ตำแหน่งที่ใช้ Spawn
    public MineralSystem mineralSystem; // อ้างอิงระบบแร่

    public Button[] spawnButtons;       // ปุ่ม UI สำหรับ Spawn Ranger
    private float[] cooldownTimers;     // ตัวจับเวลาสำหรับคูลดาวน์ของแต่ละปุ่ม

    void Start()
    {
        cooldownTimers = new float[rangerPrefabs.Length];
    }

    void Update()
    {
        // อัปเดตคูลดาวน์
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;
                spawnButtons[i].interactable = cooldownTimers[i] <= 0; // ปิดปุ่มถ้ายังไม่หมดคูลดาวน์
            }
        }
    }

    public void SpawnRanger(int index)
    {
        if (index >= 0 && index < rangerPrefabs.Length && cooldownTimers[index] <= 0)
        {
            Ranger ranger = rangerPrefabs[index].GetComponent<Ranger>();
            if (ranger != null && mineralSystem.SpendMinerals(ranger.mineralCost))
            {
                Instantiate(rangerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"สร้าง Ranger (ใช้แร่ {ranger.mineralCost})");
                cooldownTimers[index] = ranger.spawnCooldown; // ใช้คูลดาวน์จาก Ranger
            }
        }
        else
        {
            Debug.Log("ยังอยู่ในคูลดาวน์ หรือค่าแร่ไม่พอ!");
        }
    }
}
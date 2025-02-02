using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RangerSpawner : MonoBehaviour
{
    public GameObject[] rangerPrefabs;    // อาเรย์สำหรับเก็บ Prefab ของ Ranger
    public Button[] spawnButtons;        // ปุ่ม UI สำหรับ Spawn Ranger
    public TextMeshProUGUI[] cooldownTexts; // TMP UI สำหรับแสดงเวลาคูลดาวน์ของแต่ละปุ่ม
    public TextMeshProUGUI[] mineralCostTexts; // TMP UI สำหรับแสดงค่าแร่ที่ต้องใช้ของแต่ละปุ่ม
    public Transform spawnPoint;         // ตำแหน่งที่ใช้ Spawn
    public MineralSystem mineralSystem;  // อ้างอิงระบบแร่
    
    private float[] cooldownTimers;      // ตัวจับเวลาสำหรับคูลดาวน์ของแต่ละปุ่ม
    public RangerUI rangerUI;            // อ้างอิงไปยัง Ranger UI

    void Start()
    {
        cooldownTimers = new float[rangerPrefabs.Length];

        // เริ่มต้นข้อความคูลดาวน์และค่าแร่ที่ต้องใช้
        for (int i = 0; i < cooldownTexts.Length; i++)
        {
            cooldownTexts[i].text = ""; // เริ่มต้นข้อความเป็นว่าง

            // อัปเดตค่าแร่ที่ต้องใช้
            Ranger ranger = rangerPrefabs[i].GetComponent<Ranger>();
            if (ranger != null && i < mineralCostTexts.Length)
            {
                mineralCostTexts[i].text = $"{ranger.mineralCost}";
            }
        }
    }

    void Update()
    {
        // อัปเดตคูลดาวน์
        for (int i = 0; i < cooldownTimers.Length; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;

                // ปิดปุ่มถ้ายังไม่หมดคูลดาวน์
                spawnButtons[i].interactable = cooldownTimers[i] <= 0;

                // อัปเดตข้อความคูลดาวน์
                cooldownTexts[i].text = cooldownTimers[i] > 0
                    ? $"{Mathf.Ceil(cooldownTimers[i])}s" // แสดงเวลาคูลดาวน์เป็นวินาที
                    : "";
            }
            else
            {
                // ลบข้อความคูลดาวน์เมื่อหมดเวลา
                cooldownTexts[i].text = "";
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
                GameObject rangerObject = Instantiate(rangerPrefabs[index], spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"สร้าง Ranger (ใช้แร่ {ranger.mineralCost})");
                cooldownTimers[index] = ranger.spawnCooldown; // ใช้คูลดาวน์จาก Ranger

                // อัปเดต UI
                rangerUI.UpdateRangerStats(ranger);
            }
        }
        else
        {
            Debug.Log("ยังอยู่ในคูลดาวน์ หรือค่าแร่ไม่พอ!");
        }
    }
    
    public void UpdateMineralCostText(int index)
    {
        if (index >= 0 && index < rangerPrefabs.Length && index < mineralCostTexts.Length)
        {
            Ranger ranger = rangerPrefabs[index].GetComponent<Ranger>();
            if (ranger != null)
            {
                mineralCostTexts[index].text = $"{ranger.mineralCost} Minerals";
            }
        }
    }
}
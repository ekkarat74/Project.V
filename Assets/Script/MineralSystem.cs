using UnityEngine;
using TMPro;

public class MineralSystem : MonoBehaviour
{
    public int currentMinerals = 0;       // จำนวนแร่ที่มีในปัจจุบัน
    public int mineralsPerSecond = 10;    // แร่ที่ผลิตได้ต่อวินาที
    public int maxMinerals = 100;         // จำนวนแร่สูงสุดที่สามารถเก็บได้
    public float timer = 0f;             // ตัวจับเวลาสำหรับผลิตแร่

    public TextMeshProUGUI mineralsText;  // TMP UI สำหรับแสดงจำนวนแร่

    void Start()
    {
        UpdateMineralsUI(); // อัปเดต UI เมื่อเริ่มต้นเกม
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            ProduceMinerals();
            timer = 0f;
        }
    }

    void ProduceMinerals()
    {
        if (currentMinerals < maxMinerals) // ตรวจสอบว่าแร่ไม่เกินค่าที่กำหนด
        {
            currentMinerals += mineralsPerSecond;
            if (currentMinerals > maxMinerals) // ถ้าแร่เกินกำหนดให้ตั้งค่ากลับเป็น maxMinerals
            {
                currentMinerals = maxMinerals;
            }
            UpdateMineralsUI(); // อัปเดต UI เมื่อจำนวนแร่เปลี่ยน
        }
    }

    public bool SpendMinerals(int amount)
    {
        if (currentMinerals >= amount)
        {
            currentMinerals -= amount;
            UpdateMineralsUI(); // อัปเดต UI เมื่อใช้แร่
            return true; // แร่เพียงพอ
        }
        else
        {
            Debug.LogWarning("แร่ไม่เพียงพอ!");
            return false; // แร่ไม่พอ
        }
    }

    void UpdateMineralsUI()
    {
        if (mineralsText != null)
        {
            mineralsText.text = $"Minerals : {currentMinerals} / {maxMinerals}"; // อัปเดตข้อความแสดงจำนวนแร่
        }
    }
}
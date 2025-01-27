using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MineralSystem : MonoBehaviour
{
    // ตัวแปรที่เกี่ยวข้องกับระบบการผลิตแร่
    [Header("Mineral Settings")]
    public int currentMinerals = 0;   // จำนวนแร่ในปัจจุบัน
    private float timer = 0f;         // ตัวจับเวลาสำหรับการผลิตแร่

    // ตัวแปรที่เกี่ยวข้องกับระบบอัปเกรด
    [Header("Upgrade Settings")]
    public int[] upgradeMaxValues = { 50, 100, 150, 200, 300, 400, 500 }; // ความจุสูงสุดของแร่ในแต่ละระดับ
    public int[] upgradeCosts = { 50, 100, 150, 200, 250, 300, 350 };     // ค่าใช้จ่ายสำหรับการอัปเกรดในแต่ละระดับ
    public float[] timeMineralValues = { 2.0f, 1.8f, 1.6f, 1.4f, 1.2f, 1.0f, 0.8f }; // เวลาที่ใช้ในการผลิตแร่หนึ่งรอบ
    public int[] mineralsPerSecondValues = { 10, 15, 20, 25, 30, 35, 40 }; // จำนวนแร่ที่ผลิตต่อรอบในแต่ละระดับ
    public int currentUpgradeLevel = 0; // ระดับการอัปเกรดปัจจุบัน (เริ่มต้นที่ 0)

    // ตัวแปรที่เกี่ยวข้องกับ UI
    [Header("UI Elements")]
    public TextMeshProUGUI mineralsText;       // แสดงจำนวนแร่ในหน้าจอ
    public Button upgradeButton;               // ปุ่มสำหรับอัปเกรด
    public TextMeshProUGUI upgradeCostText;    // แสดงค่าใช้จ่ายในการอัปเกรด

    // ฟังก์ชันที่เรียกเมื่อเริ่มต้นเกม
    void Start()
    {
        // กำหนดจำนวนแร่เริ่มต้นให้เท่ากับความจุสูงสุดของระดับปัจจุบัน
        currentMinerals = upgradeMaxValues[currentUpgradeLevel];

        // อัปเดต UI เพื่อแสดงข้อมูลเริ่มต้น
        UpdateUI();

        // เช็คว่าปุ่มอัปเกรดไม่เป็น null และผูกฟังก์ชันอัปเกรดกับปุ่ม
        upgradeButton?.onClick.AddListener(UpgradeMaxMinerals);
    }

    // ฟังก์ชันที่เรียกในทุกเฟรม
    void Update()
    {
        // เพิ่มตัวจับเวลาตามเวลาที่ผ่านไปในแต่ละเฟรม
        timer += Time.deltaTime;

        // ถ้าครบเวลาที่กำหนดสำหรับการผลิตแร่ ให้เพิ่มจำนวนแร่
        if (timer >= timeMineralValues[currentUpgradeLevel])
        {
            ProduceMinerals(); // เรียกฟังก์ชันผลิตแร่
            timer = 0f;        // รีเซ็ตตัวจับเวลา
        }
    }

    // ฟังก์ชันสำหรับใช้แร่
    public bool SpendMinerals(int amount)
    {
        // ถ้าจำนวนแร่ไม่เพียงพอ ให้คืนค่า false
        if (currentMinerals < amount) return false;

        // ถ้าจำนวนแร่เพียงพอ ให้หักจำนวนแร่ที่ใช้ และอัปเดต UI
        currentMinerals -= amount;
        UpdateUI();
        return true; // คืนค่า true เพื่อบอกว่าใช้แร่สำเร็จ
    }

    // ฟังก์ชันสำหรับผลิตแร่
    void ProduceMinerals()
    {
        // เช็คว่าจำนวนแร่ยังไม่ถึงความจุสูงสุด
        if (currentMinerals < upgradeMaxValues[currentUpgradeLevel])
        {
            // เพิ่มแร่ และใช้ Mathf.Min เพื่อป้องกันจำนวนแร่เกินความจุสูงสุด
            currentMinerals = Mathf.Min(
                currentMinerals + mineralsPerSecondValues[currentUpgradeLevel],
                upgradeMaxValues[currentUpgradeLevel]
            );
            UpdateUI(); // อัปเดต UI เพื่อแสดงจำนวนแร่ใหม่
        }
    }

    // ฟังก์ชันสำหรับอัปเดต UI
    void UpdateUI()
    {
        // แสดงจำนวนแร่ปัจจุบันและความจุสูงสุดในข้อความ
        mineralsText.text = $"{currentMinerals} / {upgradeMaxValues[currentUpgradeLevel]}";

        // ถ้าระดับการอัปเกรดยังไม่ถึงขั้นสูงสุด
        if (currentUpgradeLevel < upgradeMaxValues.Length - 1)
        {
            // ปุ่มอัปเกรดจะเปิดใช้งานเมื่อแร่เพียงพอต่อค่าใช้จ่าย
            upgradeButton.interactable = currentMinerals >= upgradeCosts[currentUpgradeLevel];
            // แสดงค่าใช้จ่ายในการอัปเกรด
            upgradeCostText.text = $"Cost: {upgradeCosts[currentUpgradeLevel]}";
        }
        else
        {
            // ถ้าถึงระดับสูงสุดแล้ว ให้ปิดปุ่มอัปเกรดและแสดงข้อความ "Max Level"
            upgradeButton.interactable = false;
            upgradeCostText.text = "Max Level";
        }
    }

    // ฟังก์ชันสำหรับอัปเกรดความจุแร่
    public void UpgradeMaxMinerals()
    {
        // เช็คว่าระดับการอัปเกรดยังไม่ถึงขั้นสูงสุด และแร่เพียงพอสำหรับการอัปเกรด
        if (currentUpgradeLevel < upgradeMaxValues.Length - 1 && SpendMinerals(upgradeCosts[currentUpgradeLevel]))
        {
            // เพิ่มระดับการอัปเกรด
            currentUpgradeLevel++;
            // แสดงข้อความใน Debug Console เพื่อแจ้งว่าการอัปเกรดสำเร็จ
            Debug.Log($"Upgraded to Level {currentUpgradeLevel + 1}: Max {upgradeMaxValues[currentUpgradeLevel]}, Rate {mineralsPerSecondValues[currentUpgradeLevel]} minerals/sec");
            UpdateUI(); // อัปเดต UI หลังการอัปเกรด
        }
    }
}
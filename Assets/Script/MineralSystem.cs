using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MineralSystem : MonoBehaviour
{
    public int currentMinerals = 0;         // จำนวนแร่ที่มีในปัจจุบัน
    private float timer = 0f;              // ตัวจับเวลาสำหรับการผลิตแร่

    [Header("Upgrade Settings")]
    public int[] upgradeMaxValues = { 50, 100, 150, 200, 300, 400, 500 }; // ค่า maxMinerals ในแต่ละขั้น
    public int[] upgradeCosts = { 50, 100, 150, 200, 250, 300, 350 };     // ค่าใช้จ่ายสำหรับอัปเกรดในแต่ละขั้น
    public float[] timeMineralValues = { 2.0f, 1.8f, 1.6f, 1.4f, 1.2f, 1.0f, 0.8f }; // เวลาในการผลิตแร่ต่อขั้น
    public int[] mineralsPerSecondValues = { 10, 15, 20, 25, 30, 35, 40 }; // ค่า mineralsPerSecond ในแต่ละขั้น
    public int currentUpgradeLevel = 0;    // ขั้นอัปเกรดปัจจุบัน (เริ่มที่ 0)

    [Header("UI Elements")]
    public TextMeshProUGUI mineralsText;       // TMP UI สำหรับแสดงจำนวนแร่
    public Button upgradeButton;               // ปุ่มสำหรับอัปเกรดแร่
    public TextMeshProUGUI upgradeCostText;    // TMP UI สำหรับแสดงค่าใช้จ่ายในการอัปเกรด

    void Start()
    {
        // ตั้งค่า currentMinerals ให้เท่ากับค่า maxMinerals ของระดับปัจจุบัน
        currentMinerals = upgradeMaxValues[currentUpgradeLevel];

        // อัปเดต UI
        UpdateMineralsUI();
        UpdateUpgradeButton();

        // เพิ่ม Listener ให้ปุ่มอัปเกรด
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(UpgradeMaxMinerals);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= GetCurrentTimeMineral())
        {
            ProduceMinerals();
            timer = 0f;
        }
    }

    public bool SpendMinerals(int amount)
    {
        if (currentMinerals >= amount)
        {
            currentMinerals -= amount;
            UpdateMineralsUI();
            UpdateUpgradeButton();
            return true;
        }
        else
        {
            Debug.LogWarning("แร่ไม่เพียงพอ!");
            return false;
        }
    }

    void ProduceMinerals()
    {
        int maxMinerals = upgradeMaxValues[currentUpgradeLevel]; // ดึงค่า maxMinerals ตามระดับขั้น
        if (currentMinerals < maxMinerals)
        {
            currentMinerals += GetCurrentMineralsPerSecond();
            if (currentMinerals > maxMinerals)
            {
                currentMinerals = maxMinerals;
            }
            UpdateMineralsUI();
            UpdateUpgradeButton();
        }
    }

    void UpdateMineralsUI()
    {
        int maxMinerals = upgradeMaxValues[currentUpgradeLevel]; // ดึงค่า maxMinerals ตามระดับขั้น
        if (mineralsText != null)
        {
            mineralsText.text = $"{currentMinerals} / {maxMinerals}";
        }
    }

    void UpdateUpgradeButton()
    {
        if (upgradeButton != null && upgradeCostText != null)
        {
            if (currentUpgradeLevel < upgradeMaxValues.Length - 1)
            {
                upgradeButton.interactable = currentMinerals >= upgradeCosts[currentUpgradeLevel];
                upgradeCostText.text = $"Cost: {upgradeCosts[currentUpgradeLevel]}";
            }
            else
            {
                upgradeButton.interactable = false;
                upgradeCostText.text = "Max Level";
            }
        }
    }

    public void UpgradeMaxMinerals()
    {
        if (currentUpgradeLevel < upgradeMaxValues.Length - 1 && SpendMinerals(upgradeCosts[currentUpgradeLevel]))
        {
            currentUpgradeLevel++;
            Debug.Log($"Max Minerals upgraded to: {upgradeMaxValues[currentUpgradeLevel]}, Minerals/Second: {GetCurrentMineralsPerSecond()}");
            UpdateMineralsUI();
            UpdateUpgradeButton();
        }
        else
        {
            Debug.LogWarning("ไม่สามารถอัปเกรดได้!");
        }
    }

    float GetCurrentTimeMineral()
    {
        return timeMineralValues[currentUpgradeLevel]; // ดึงค่า TimeMineral ตามระดับขั้น
    }

    int GetCurrentMineralsPerSecond()
    {
        return mineralsPerSecondValues[currentUpgradeLevel]; // ดึงค่า mineralsPerSecond ตามระดับขั้น
    }
}

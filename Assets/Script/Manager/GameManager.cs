using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ทำให้เป็น Singleton เพื่อให้เรียกใช้งานได้ง่าย

    [Header("UI Elements")]
    public GameObject gameOverPanel; // Panel ที่จะแสดงเมื่อเกมจบ
    public GameObject winPanel;      // Panel ที่จะแสดงเมื่อผู้เล่นชนะ
    public TextMeshProUGUI countdownText;       // Text UI สำหรับแสดงเวลาถอยหลัง
    public Button mainMenuButton;    // ปุ่มกลับไปที่เมนูหลัก
    public Button speedUpButton;     // ปุ่มเร่งความเร็วเกม

    [Header("Game Settings")]
    public float countdownTime = 60f; // เวลาถอยหลังเริ่มต้น (หน่วย: วินาที)
    public Button pauseButton;
    private bool isPaused = false;
    private float currentTime;        // เวลาปัจจุบันที่เหลือ
    private bool isGameOver = false;  // สถานะเกมจบ

    [Header("Skill System")]
    public Button skillButton; // ปุ่มสำหรับใช้งานสกิล
    public float skillDuration = 5f; // ระยะเวลาของสกิล
    private bool isSkillActive = false; // สถานะของสกิล
    
    private bool isSpeedUp = false;  // ตัวบ่งชี้ว่าเกมกำลังเร่งความเร็วหรือไม่

    [Header("Difficulty Scaling")]
    public float difficultyScaleRate = 0.1f; // อัตราการเพิ่มความยากต่อวินาที
    public float currentDifficulty = 1f; // ค่าความยากปัจจุบัน
    public float maxDifficulty = 5f; // ค่าความยากสูงสุด
    public float enemySpawnRate = 2f; // อัตราการสปอว์นศัตรู
    public float enemySpeedMultiplier = 1f; // ตัวคูณความเร็วของศัตรู
    public float enemyDamageMultiplier = 1f; // ตัวคูณดาเมจของศัตรู

    private void Awake()
    {
        // ตรวจสอบว่า Instance มีอยู่แล้วหรือไม่
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ทำให้ GameManager อยู่ถาวรระหว่างการเปลี่ยนฉาก
        }
        else
        {
            Destroy(gameObject); // ทำลาย GameManager ซ้ำ
        }
    }

    private void Start()
    {
        // เริ่มต้นเวลาถอยหลัง
        currentTime = countdownTime;

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        // ซ่อน Game Over และ Win Panel เริ่มต้น
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // ผูกฟังก์ชันกับปุ่ม Main Menu
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // ผูกฟังก์ชันกับปุ่มเร่งความเร็ว
        if (speedUpButton != null)
        {
            speedUpButton.onClick.AddListener(ToggleSpeedUp);
        }
        
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(ActivateSkill);
        }
    }

    private void Update()
    {
        if (!isGameOver)
        {
            // อัปเดตเวลาถอยหลัง
            currentTime -= Time.deltaTime;

            // แสดงเวลาถอยหลังใน Text UI
            if (countdownText != null)
            {
                countdownText.text = "Time: " + Mathf.Max(0, Mathf.FloorToInt(currentTime)).ToString();
            }

            // ตรวจสอบว่าเวลาหมดหรือไม่
            if (currentTime <= 0)
            {
                GameOver();
            }

            // อัปเดตความยาก
            UpdateDifficulty();
        }
    }
    
    void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        Debug.Log(isPaused ? "เกมถูกหยุด" : "เกมเล่นต่อ");
    }

    // ฟังก์ชันเรียกเมื่อ TowerEnemy หรือ TowerRanger ถูกทำลาย
    public void EndGame(string losingSide)
    {
        if (losingSide == "TowerEnemy")
        {
            WinGame();
        }
        else
        {
            GameOver();
        }
    }

    // ฟังก์ชันสำหรับแสดงหน้า Win
    private void WinGame()
    {
        isGameOver = true;
        Time.timeScale = 0; // หยุดเวลา
        if (winPanel != null)
        {
            winPanel.SetActive(true); // แสดง Win Panel
        }
    }

    // ฟังก์ชันสำหรับแสดงหน้า Game Over
    private void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0; // หยุดเวลา
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // แสดง Game Over Panel
        }
    }

    // ฟังก์ชันสำหรับกลับไปที่เมนูหลัก
    public void GoToMainMenu()
    {
        Time.timeScale = 1; // รีเซ็ตเวลาให้ปกติ
        SceneManager.LoadScene("MainMenu"); // โหลดฉากเมนูหลัก
    }

    // ฟังก์ชันสำหรับเปิด/ปิดการเร่งความเร็ว
    public void ToggleSpeedUp()
    {
        isSpeedUp = !isSpeedUp;
        Time.timeScale = isSpeedUp ? 2f : 1f; // ถ้าเร่งความเร็วจะใช้ 2 เท่า ถ้าไม่เร่งจะใช้ 1 เท่า
        Debug.Log(isSpeedUp ? "เกมกำลังเร่งความเร็ว" : "เกมกลับสู่ความเร็วปกติ");
    }
    
    void ActivateSkill()
    {
        if (!isSkillActive)
        {
            isSkillActive = true;
            Debug.Log("สกิลทำงาน: หยุดศัตรูทั้งหมด");

            // ปิดการใช้งานปุ่มหลังจากกดใช้สกิล
            if (skillButton != null)
            {
                skillButton.interactable = false;
            }

            // หยุดการเคลื่อนที่ของศัตรู
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                enemy.StopEnemy(skillDuration);
            }

            // เรียกคืนสถานะหลังจากระยะเวลาสกิลสิ้นสุด
            Invoke(nameof(ResetSkill), skillDuration);
        }
    }

    void ResetSkill()
    {
        isSkillActive = false;
        Debug.Log("สกิลสิ้นสุด: ศัตรูกลับมาเคลื่อนที่");
    }

    // ฟังก์ชันสำหรับอัปเดตความยาก
    void UpdateDifficulty()
    {
        if (currentDifficulty < maxDifficulty)
        {
            currentDifficulty += difficultyScaleRate * Time.deltaTime;
            currentDifficulty = Mathf.Min(currentDifficulty, maxDifficulty);

            // ปรับค่าต่าง ๆ ตามความยาก
            enemySpeedMultiplier = 1 + (currentDifficulty * 0.2f); // เพิ่มความเร็วศัตรู 20% ต่อระดับความยาก
            enemyDamageMultiplier = 1 + (currentDifficulty * 0.1f); // เพิ่มดาเมจศัตรู 10% ต่อระดับความยาก
            enemySpawnRate = 2 + (currentDifficulty * 0.5f); // เพิ่มอัตราการสปอว์นศัตรู 0.5 ต่อระดับความยาก

            Debug.Log($"Difficulty: {currentDifficulty}, Enemy Speed: {enemySpeedMultiplier}, Enemy Damage: {enemyDamageMultiplier}, Spawn Rate: {enemySpawnRate}");
        }
    }
}
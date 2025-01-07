using UnityEngine;
using UnityEngine.SceneManagement; // สำหรับการเปลี่ยนฉาก
using UnityEngine.UI;              // สำหรับจัดการ UI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // ทำให้เป็น Singleton เพื่อให้เรียกใช้งานได้ง่าย

    [Header("UI Elements")]
    public GameObject gameOverPanel; // Panel ที่จะแสดงเมื่อเกมจบ
    public Button mainMenuButton;    // ปุ่มกลับไปที่เมนูหลัก
    public Button speedUpButton;     // ปุ่มเร่งความเร็วเกม

    private bool isSpeedUp = false;  // ตัวบ่งชี้ว่าเกมกำลังเร่งความเร็วหรือไม่

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
        // ซ่อน Game Over Panel เริ่มต้น
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
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
    }

    // ฟังก์ชันเรียกเมื่อ TowerEnemy หรือ TowerRanger ถูกทำลาย
    public void EndGame(string losingSide)
    {
        Debug.Log($"{losingSide} ถูกทำลาย! เกมจบแล้ว!");
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
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button playButton;    // ปุ่มเริ่มเกม
    public Button quitButton;    // ปุ่มออกจากเกม

    public string gameSceneName = "GameScene"; // ชื่อฉากเกมที่ต้องการโหลด

    private void Start()
    {
        // กำหนด Event ให้กับปุ่ม
        playButton.onClick.AddListener(PlayGame);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(gameSceneName); // โหลดฉากเกม
    }


    public void QuitGame()
    {
        Debug.Log("ออกจากเกม");
        Application.Quit(); // ใช้งานได้จริงเมื่อ Build เกม
    }
}
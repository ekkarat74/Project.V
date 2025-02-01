using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RangerUI : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;

    private Ranger currentRanger; 

    public void UpdateRangerStats(Ranger ranger)
    {
        currentRanger = ranger;

        // อัปเดต UI ด้วยข้อมูลจาก Ranger
        healthText.text = "Health: " + currentRanger.health;
        levelText.text = "Level: " + currentRanger.level;
        expText.text = "Exp: " + currentRanger.currentExp + " / " + currentRanger.expToNextLevel;
    }
}
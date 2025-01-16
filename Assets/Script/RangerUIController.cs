using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RangerUIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject[] rangerUIPrefabs;       // Array ของ Prefab UI สำหรับ Ranger ประเภทต่าง ๆ
    public Transform[] rangerUIContainers;    // Array ของ Container สำหรับจัดเรียง UI ของแต่ละประเภท
    public TextMeshProUGUI[] rangerCountTexts; // Text TMP แสดงจำนวนของ Ranger แต่ละประเภท

    private List<RangerUIState> rangerUIList = new List<RangerUIState>();
    private Dictionary<int, int> rangerCountByType = new Dictionary<int, int>();

    public void AddRangerUI(Ranger ranger, int rangerTypeIndex)
    {
        if (rangerTypeIndex < 0 || rangerTypeIndex >= rangerUIPrefabs.Length || rangerTypeIndex >= rangerUIContainers.Length)
        {
            Debug.LogError("Invalid ranger type index.");
            return;
        }

        // สร้าง UI ของ Ranger
        GameObject uiInstance = Instantiate(rangerUIPrefabs[rangerTypeIndex], rangerUIContainers[rangerTypeIndex]);
        RangerUIState rangerUI = uiInstance.GetComponent<RangerUIState>();
        rangerUI.Initialize(ranger);
        rangerUIList.Add(rangerUI);

        // อัปเดตจำนวน Ranger ต่อประเภท
        if (!rangerCountByType.ContainsKey(rangerTypeIndex))
        {
            rangerCountByType[rangerTypeIndex] = 0;
        }
        rangerCountByType[rangerTypeIndex]++;
        UpdateRangerCountUI(rangerTypeIndex);
    }

    public void RemoveRangerUI(Ranger ranger)
    {
        RangerUIState uiToRemove = rangerUIList.Find(ui => ui.AssociatedRanger == ranger);

        if (uiToRemove != null)
        {
            int rangerTypeIndex = ranger.rangerTypeIndex;

            rangerUIList.Remove(uiToRemove);
            Destroy(uiToRemove.gameObject);

            // ลดจำนวน Ranger ต่อประเภท
            if (rangerCountByType.ContainsKey(rangerTypeIndex))
            {
                rangerCountByType[rangerTypeIndex]--;
                UpdateRangerCountUI(rangerTypeIndex);
            }
        }
    }

    private void UpdateRangerCountUI(int rangerTypeIndex)
    {
        if (rangerTypeIndex < 0 || rangerTypeIndex >= rangerCountTexts.Length)
        {
            Debug.LogError("Invalid ranger type index for updating count.");
            return;
        }

        int count = rangerCountByType.ContainsKey(rangerTypeIndex) ? rangerCountByType[rangerTypeIndex] : 0;
        rangerCountTexts[rangerTypeIndex].text = $"Count: {count}";
    }
}
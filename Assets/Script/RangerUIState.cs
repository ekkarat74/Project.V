using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RangerUIState : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI mineralText;
    public TextMeshProUGUI projectileDamageText;

    private Ranger associatedRanger;

    public Ranger AssociatedRanger => associatedRanger;

    public void Initialize(Ranger ranger)
    {
        associatedRanger = ranger;
        UpdateUI();
    }

    void Update()
    {
        if (associatedRanger != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        mineralText.text = $"Mineral: {associatedRanger.mineralCost}";
        projectileDamageText.text = $"Damage: {associatedRanger.projectileDamage}";
    }
}

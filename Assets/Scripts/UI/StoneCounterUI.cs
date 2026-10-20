using TMPro;
using UnityEngine;

public class StoneCounterUI : MonoBehaviour
{
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        if (label == null)
        {
            label = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        if (inventory == null || label == null)
        {
            Debug.LogWarning("StoneCounterUI: preencha Inventory (arraste o Player) e Label no Inspector.");
            return;
        }

        inventory.StoneCountChanged += UpdateLabel;
        UpdateLabel(inventory.StoneCount);
    }

    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.StoneCountChanged -= UpdateLabel;
        }
    }

    private void UpdateLabel(int count)
    {
        label.text = "Pedras: " + count;
    }
}
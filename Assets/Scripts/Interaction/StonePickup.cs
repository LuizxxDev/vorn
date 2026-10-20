using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class StonePickup : MonoBehaviour
{
    [SerializeField] private int amount = 1;

    private PlayerInventory playerInRange;

    // Reset roda uma vez, quando o componente é adicionado no Editor.
    // Aqui ele já deixa o collider como trigger, para você não esquecer.
    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null)
        {
            playerInRange = inventory;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>();

        if (inventory != null && inventory == playerInRange)
        {
            playerInRange = null;
        }
    }

    private void Update()
    {
        if (playerInRange == null) return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        // wasPressedThisFrame é verdadeiro só no quadro em que a tecla foi apertada.
        if (keyboard.eKey.wasPressedThisFrame)
        {
            playerInRange.AddStones(amount);
            Debug.Log("Pedras no inventário: " + playerInRange.StoneCount);
            Destroy(gameObject);
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInventory))]
[RequireComponent(typeof(PlayerMovement))]
public class PlayerThrow : MonoBehaviour
{
    [SerializeField] private StoneProjectile projectilePrefab;
    [SerializeField] private float cooldown = 0.4f;
    [SerializeField] private float spawnDistance = 0.5f;
    [SerializeField] private float spawnHeight = 0.5f;

    private PlayerInventory inventory;
    private PlayerMovement movement;
    private float nextThrowTime;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (!keyboard.spaceKey.wasPressedThisFrame) return;
        if (Time.time < nextThrowTime) return;

        if (projectilePrefab == null)
        {
            Debug.LogWarning("PlayerThrow: arraste o prefab StoneProjectile no Inspector.");
            return;
        }

        if (!inventory.TrySpendStone()) return;

        Vector2 direction = movement.FacingVector;

        // O pivô do personagem fica nos pés, então subimos a saída da pedra até a altura das mãos.
        Vector2 spawnPosition = (Vector2)transform.position
                                + Vector2.up * spawnHeight
                                + direction * spawnDistance;

        StoneProjectile stone = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        stone.Launch(direction);

        nextThrowTime = Time.time + cooldown;
    }
}
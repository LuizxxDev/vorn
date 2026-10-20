using System.Collections.Generic;
using UnityEngine;

public class ForestScatter : MonoBehaviour
{
    [Header("Sprites a espalhar")]
    [SerializeField] private Sprite[] sprites;

    [Header("Área (centrada neste objeto)")]
    [SerializeField] private Vector2 areaSize = new Vector2(50f, 30f);
    [SerializeField] private int count = 60;
    [SerializeField] private float minSpacing = 3f;
    [SerializeField] private int seed = 1;

    [Header("Zonas livres (X,Y = canto inferior esquerdo, em coordenadas do mundo)")]
    [SerializeField] private Rect[] clearZones;

    [Header("Aparência")]
    [SerializeField] private int sortingOrder = 0;
    [SerializeField] private Vector2 scaleRange = new Vector2(0.9f, 1.1f);
    [SerializeField] private bool randomFlip = true;

    [Header("Colisão na base (pivô embaixo)")]
    [SerializeField] private bool blocksMovement = true;
    [SerializeField, Range(0.05f, 1f)] private float colliderWidthPercent = 0.25f;
    [SerializeField] private float colliderHeight = 0.3f;

    [ContextMenu("Scatter")]
    public void Scatter()
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning("ForestScatter: coloque pelo menos um sprite na lista.");
            return;
        }

        Clear();

        System.Random random = new System.Random(seed);
        List<Vector2> placed = new List<Vector2>();
        int attempts = 0;
        int maxAttempts = count * 40;

        while (placed.Count < count && attempts < maxAttempts)
        {
            attempts++;

            float x = transform.position.x + ((float)random.NextDouble() - 0.5f) * areaSize.x;
            float y = transform.position.y + ((float)random.NextDouble() - 0.5f) * areaSize.y;
            Vector2 position = new Vector2(x, y);

            if (IsInClearZone(position)) continue;
            if (IsTooClose(position, placed)) continue;

            placed.Add(position);
            Spawn(position, random);
        }

        Debug.Log("ForestScatter: " + placed.Count + " de " + count + " objetos criados.");
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = transform.GetChild(i).gameObject;

#if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(child);
#else
            Destroy(child);
#endif
        }
    }

    private bool IsInClearZone(Vector2 position)
    {
        if (clearZones == null) return false;

        foreach (Rect zone in clearZones)
        {
            if (zone.Contains(position)) return true;
        }

        return false;
    }

    private bool IsTooClose(Vector2 position, List<Vector2> placed)
    {
        foreach (Vector2 other in placed)
        {
            if (Vector2.Distance(position, other) < minSpacing) return true;
        }

        return false;
    }

    private void Spawn(Vector2 position, System.Random random)
    {
        Sprite sprite = sprites[random.Next(sprites.Length)];

        GameObject go = new GameObject(sprite.name);
        go.transform.SetParent(transform);
        go.transform.position = position;

        float scale = Mathf.Lerp(scaleRange.x, scaleRange.y, (float)random.NextDouble());
        go.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = sortingOrder;
        spriteRenderer.spriteSortPoint = SpriteSortPoint.Pivot;
        spriteRenderer.flipX = randomFlip && random.Next(2) == 0;

        if (blocksMovement)
        {
            Bounds bounds = sprite.bounds; // espaço local; com pivô embaixo, min.y vale 0
            BoxCollider2D box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(bounds.size.x * colliderWidthPercent, colliderHeight);
            box.offset = new Vector2(bounds.center.x, bounds.min.y + colliderHeight * 0.5f);
        }

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Scatter forest");
#endif
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, areaSize.y, 0f));

        Gizmos.color = Color.yellow;

        if (clearZones == null) return;

        foreach (Rect zone in clearZones)
        {
            Gizmos.DrawWireCube(zone.center, new Vector3(zone.width, zone.height, 0f));
        }
    }
}
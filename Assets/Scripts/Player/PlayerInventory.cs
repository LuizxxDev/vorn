using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public event Action<int> StoneCountChanged;

    public int StoneCount { get; private set; }

    public void AddStones(int amount)
    {
        if (amount <= 0) return;

        StoneCount += amount;
        StoneCountChanged?.Invoke(StoneCount);
    }

    public bool TrySpendStone()
    {
        if (StoneCount <= 0) return false;

        StoneCount--;
        StoneCountChanged?.Invoke(StoneCount);
        return true;
    }
}
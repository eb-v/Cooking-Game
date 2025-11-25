using UnityEngine;

public static class FreezeManager
{
    // If true, the slot machine is freezing gameplay
    public static bool GameFrozenBySlotMachine = false;

    // If true, the pause menu is forcing a freeze override
    public static bool PauseMenuOverride = false;

    /// <summary>
    /// Main logic: applies the correct timeScale based on priority.
    /// </summary>
    public static void ApplyState()
    {
        // 1️⃣ Pause menu ALWAYS wins
        if (PauseMenuOverride)
        {
            Time.timeScale = 0;
            return;
        }

        // 2️⃣ Slot machine freeze
        if (GameFrozenBySlotMachine)
        {
            Time.timeScale = 0;
            return;
        }

        // 3️⃣ Normal gameplay
        Time.timeScale = 1;
    }

    // ------------------------------------------------------------------
    // ⭐ NEW: Backward-compatible methods for slot machine scripts
    // ------------------------------------------------------------------

    public static void FreezeGameplay()
    {
        // Slot machine freeze
        GameFrozenBySlotMachine = true;
        ApplyState();
    }

    public static void UnfreezeGameplay()
    {
        // Stop slot machine freeze
        GameFrozenBySlotMachine = false;
        ApplyState();
    }

    // ------------------------------------------------------------------
    // Optional helpers used by updated SlotMachineManager:
    // ------------------------------------------------------------------

    public static void FreezeBySlotMachine()
    {
        FreezeGameplay();
    }

    public static void UnfreezeBySlotMachine()
    {
        UnfreezeGameplay();
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class RedTeamKeyboard : MonoBehaviour
{
    public FoosballRod[] redRods; // 0: GK, 1: Def, 2: Mid, 3: Att
    public int activeRod = 0;

    public float slideSpeed = 2.5f;
    public float maxAngle = 80f;
    public float snapSpeed = 30f;

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null || redRods == null || redRods.Length == 0) return;

        // 1. SWITCH RODS: Q increases index, E decreases index (locked between 0 and 3)
        if (kb.qKey.wasPressedThisFrame)
            activeRod = Mathf.Clamp(activeRod + 1, 0, redRods.Length - 1);
        if (kb.eKey.wasPressedThisFrame)
            activeRod = Mathf.Clamp(activeRod - 1, 0, redRods.Length - 1);

        if (kb.digit1Key.wasPressedThisFrame) activeRod = 0;
        if (kb.digit2Key.wasPressedThisFrame && redRods.Length > 1) activeRod = 1;
        if (kb.digit3Key.wasPressedThisFrame && redRods.Length > 2) activeRod = 2;
        if (kb.digit4Key.wasPressedThisFrame && redRods.Length > 3) activeRod = 3;

        // 2. SLIDE: W / S or Up / Down Arrows
        float slide = 0f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) slide = 1f;
        else if (kb.sKey.isPressed || kb.downArrowKey.isPressed) slide = -1f;

        // 3. KICK: A / D or Left / Right Arrows
        float kick = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) kick = -1f;
        else if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) kick = 1f;

        // 4. APPLY: Move active rod, reset unselected rods to upright (0, 0)
        for (int i = 0; i < redRods.Length; i++)
        {
            if (redRods[i] == null) continue;

            if (i == activeRod)
                redRods[i].MoveAndRotate(slide, kick, slideSpeed, maxAngle, snapSpeed);
            else
                redRods[i].MoveAndRotate(0f, 0f, slideSpeed, maxAngle, snapSpeed);
        }
    }
}
using UnityEngine;
using CardGame;
using System;


public class PlayerBridge : Player
{
    private PlayerHP hpUI;
    private PlayerCurse curseUI;

    public PlayerBridge(PlayerHP hpUI, PlayerCurse curseUI)
    {
        this.hpUI = hpUI;
        this.curseUI = curseUI;

        // 초기 상태 UI에도 반영
        this.Hp = 10;
        this.Curse = 0;
    }

    public override int Hp
    {
        get => base.Hp;
        set
        {
            int clamped = Math.Max(0, value);
            base.Hp = clamped;
            Debug.Log($"[PlayerBridge] Hp 변경됨: {clamped}"); 
            hpUI.SetHP(clamped);  // UI에도 반영
        }
    }

    public override int Curse
    {
        get => base.Curse;
        set
        {
            int clamped = Math.Max(0, value);
            base.Curse = clamped;
            Debug.Log($"[PlayerBridge] Curse 변경됨: {clamped}");
            curseUI.SetCurse(clamped);  // UI에도 반영
        }
    }
}

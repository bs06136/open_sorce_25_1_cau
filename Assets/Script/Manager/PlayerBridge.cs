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
            if (hpUI != null)
                hpUI.SetHP(clamped);  // ✅ Null 체크
            else
                Debug.LogWarning("[PlayerBridge] hpUI가 null입니다. SetHP 호출 실패");
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
            if (curseUI != null)
                curseUI.SetCurse(clamped);  // ✅ Null 체크
            else
                Debug.LogWarning("[PlayerBridge] curseUI가 null입니다. SetCurse 호출 실패");
        }
    }
}

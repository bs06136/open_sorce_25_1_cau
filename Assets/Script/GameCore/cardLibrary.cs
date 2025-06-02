using System.Collections.Generic;

namespace CardGame
{
    public static class CardLibrary
    {
        public static List<Card> AllCards = new List<Card>
{
    // 문서 순서대로 (0~40)
    new Card("바보", 0, 0, "체력을 10으로 초기화", CardEffect.FoolEffect),
    new Card("마법사", 0, 1, "3턴 후 저주 3 감소", CardEffect.MagicianEffect),
    new Card("여교황", 4, 0, "3턴간 체력 증가 불가", CardEffect.HighPriestessEffect),
    new Card("여제", 1, 0, "죽음 카드 5장 제거", CardEffect.EmpressEffect),
    new Card("황제", 0, 0, "체력 & 저주 두 배", CardEffect.EmperorEffect),
    new Card("교황", 4, 0, "3턴간 저주 감소 불가", CardEffect.HierophantEffect),
    new Card("연인", 1, 1, "리롤 기회 +1", CardEffect.LoversEffect),
    new Card("전차", 4, 0, "다음 턴 2장 선택", CardEffect.ChariotEffect),
    new Card("힘", 1, 0, ""),
    new Card("은둔자", -3, 0, "5턴 후 체력 7 증가", CardEffect.HermitEffect),
    new Card("운명의 수레바퀴", 0, 0, "체력/저주 교환", CardEffect.FortuneWheelEffect),
    new Card("정의", 0, 0, "체력 & 저주 재분배", CardEffect.JusticeEffect),
    new Card("매달린 남자", 5, 0, "다음 턴 2장만 뽑음", CardEffect.HangedManEffect),
    new Card("죽음", 0, 0, "사망한다", CardEffect.DeathEffect),
    new Card("절제", -5, 0, "3턴간 저주 피해 무효", CardEffect.TemperanceEffect),
    new Card("악마", 20, 5, ""),
    new Card("탑", -1, 0, "턴 스킵", CardEffect.TowerEffect),
    new Card("별", 2, 0, ""),
    new Card("달", -10, -2, ""),
    new Card("태양", 10, 2, ""),
    new Card("심판", -3, -1, "덱에서 5장 제거", CardEffect.JudgementEffect),
    new Card("세계", 7, -3, "모든 카드 효과 적용", CardEffect.WorldEffect),
    new Card("부활", -1, 0, "죽음 카드 제거", CardEffect.ReviveEffect),
    new Card("생명", 0, 0, "덱에 카드 20장 추가", CardEffect.LifeEffect),
    new Card("광대", 0, 1, "체력 무작위 증가", CardEffect.ScampEffect),
    new Card("거울", 0, 1, "마지막 카드 효과 재발동", CardEffect.MirrorEffect),
    new Card("연기", 0, 0, ""),
    new Card("일식", 8, 0, "2턴 후 저주 +2", CardEffect.EclipseEffect),
    new Card("암거래", 0, 2, "5턴간 죽음 카드 추가 금지", CardEffect.BlackMarketEffect),
    new Card("불씨", -1, 1, "체력 1 이하 시 생존", CardEffect.EmberEffect),
    new Card("저주받은 책", 0, 1, ""),
    new Card("예언자", -3, 1, "다음 턴 페널티 무효", CardEffect.ProphetEffect),
    new Card("종말의 경전", 0, 3, "덱 리셋", CardEffect.ApocalypseScriptureEffect),
    new Card("강탈자", -3, 0, ""),
    new Card("대천사", 1, -1, "죽음 카드 교체 효과", CardEffect.ArchangelEffect),
    new Card("영혼의 초", 5, 2, "3턴 후 저주 2 감소", CardEffect.SoulCandleEffect),
    new Card("그림자의 균열", -1, 1, ""),
    new Card("영혼 결혼식", 6, 3, "리롤 기회 +1", CardEffect.SoulWeddingEffect),
    new Card("피의 서약", -10, 0, "2턴간 저주 증가 무효", CardEffect.BloodPactEffect),
    new Card("운명의 유희", 5, 0, "다음 턴 무작위 선택", CardEffect.GambleOfFateEffect),
    new Card("꿈", -4, -1, "")
};
    }
}

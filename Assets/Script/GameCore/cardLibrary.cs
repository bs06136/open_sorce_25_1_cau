using System.Collections.Generic;

namespace CardGame
{
    public static class CardLibrary
    {
        public static List<Card> AllCards = new List<Card>
        {
            new Card("바보", 0, 0, "체력을 10으로 초기화한다.", CardEffect.FoolEffect),
            new Card("마법사", 0, 1, "3턴 뒤 저주를 3 감소시킨다.", CardEffect.MagicianEffect),
            new Card("여교황", 4, 0, "다음 3턴 동안 일반 효과로 체력을 증가시킬 수 없다.", CardEffect.HighPriestessEffect),
            new Card("여제", 1, 0, "덱 내 죽음 카드 5장을 제거한다.", CardEffect.EmpressEffect),
            new Card("황제", 0, 0, "체력과 저주를 모두 두 배로 만든다.", CardEffect.EmperorEffect),
            new Card("교황", 4, 0, "다음 3턴 동안 일반 효과로 저주를 감소시킬 수 없다.", CardEffect.HierophantEffect),
            new Card("연인", 1, 1, "리롤 기회를 1회 얻는다.", CardEffect.LoversEffect),
            new Card("전차", 4, 0, "다음 턴에는 두장의 카드를 선택해 모두 적용한다.", CardEffect.ChariotEffect),
            new Card("힘", 1, 0, " "),
            new Card("은둔자", -3, 0, "5턴 뒤 체력을 7 증가시킨다.", CardEffect.HermitEffect),
            new Card("운명의 수레바퀴", 0, 0, "체력과 저주를 5:1 비율로 바꾼다.", CardEffect.FortuneWheelEffect),
            new Card("정의", 0, 0, "체력과 저주를 합하여 5:1 비율로 나눈다.", CardEffect.JusticeEffect),
            new Card("매달린 남자", 5, 0, "다음 턴에는 2장의 카드만 뽑는다.", CardEffect.HangedManEffect),
            new Card("죽음", 0, 0, "사망한다…\n\"죽음은 모든 것의 끝이다.\"", CardEffect.DeathEffect),
            new Card("절제", -5, 0, "다음 2턴간 저주로 체력이 감소하지 않는다.", CardEffect.TemperanceEffect),
            new Card("악마", 20, 5, " "),
            new Card("탑", -1, 0, "다음 턴은 카드를 뽑지 않고 저주로 인한 체력감소가 이루어지지 않는다.", CardEffect.TowerEffect),
            new Card("별", 2, 0, " "),
            new Card("달", -10, -2, " "),
            new Card("태양", 10, 2, " "),
            new Card("심판", -3, -1, "덱에서 무작위로 카드 5장을 제거한다.", CardEffect.JudgementEffect),
            new Card("세계", 7, -3, "뽑은 모든 카드를 적용한다.\n\"통합 · 공존 · 번영\"", CardEffect.WorldEffect),
            new Card("부활", -1, 0, "덱 내 모든 죽음 카드를 삭제한다.", CardEffect.ReviveEffect),
            new Card("생명", 0, 0, "덱에 죽음을 제외한 각기 다른 무작위 카드 20장을 추가한다.", CardEffect.LifeEffect),
            new Card("악동", 0, 1, "체력을 1~10 중 무작위로 증가시킨다.", CardEffect.ScampEffect),
            new Card("거울", 0, 1, "마지막에 사용한 카드를 다시 발동시킨다.", CardEffect.MirrorEffect),
            new Card("연기", 0, 0, "\"연기는 그저 흩어질 뿐\""),
            new Card("일식", 8, 0, "2턴 뒤 저주를 2 증가시킨다.", CardEffect.EclipseEffect),
            new Card("암거래", 0, 2, "다음 5턴 동안 죽음이 덱에 추가되지 않는다.", CardEffect.BlackMarketEffect),
            new Card("불씨", -1, 1, "다음 한 번 체력이 1 이하로 떨어지게 될 경우, 체력을 1로 저주를 0으로 변경시킨다.", CardEffect.EmberEffect),
            new Card("저주받은 책", 0, 1, " "),
            new Card("예언자", -3, 1, "다음 턴에는 일반 효과로 체력은 감소시키지 않고 저주는 증가시키지 않는다.", CardEffect.ProphetEffect),
            new Card("종말의 경전", 0, 3, "덱을 리셋 시킨다. ", CardEffect.ApocalypseScriptureEffect),
            new Card("강탈자", -3, 0, " "),
            new Card("대천사", 1, -1, "다음 턴에 죽음을 뽑을 시, 해당 카드를 랜덤한 카드로 교체한다.", CardEffect.ArchangelEffect),
            new Card("영혼의 초", 5, 2, "3턴 뒤, 저주를 2 감소시킨다.", CardEffect.SoulCandleEffect),
            new Card("그림자의 균열", -1, 1, " "),
            new Card("영혼 결혼식", 6, 3, "리롤 기회를 1회 얻는다.", CardEffect.SoulWeddingEffect),
            new Card("피의 서약", -10, 0, "다음 2턴간 일반 효과로 저주를 증가시키지 않는다.", CardEffect.BloodPactEffect),
            new Card("운명의 유희", 5, 0, "다음 턴, 카드가 무작위로 선택 된다.", CardEffect.GambleOfFateEffect),
            new Card("꿈", -4, -1, " ")
        };
    }
}
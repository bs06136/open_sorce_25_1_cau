using System.Collections.Generic;

public static class CardStoryMap
{
    public static readonly Dictionary<string, string> Story = new Dictionary<string, string>
    {
        { "바보",       "This card symbolizes a pure beginning and adventure ㅁㅁ." },
        { "마법사",     "It represents creative power and self-confidence." },
        { "여교황",     "A symbol of intuition and hidden wisdom." },
        { "여제",       "Represents abundance, motherhood, and creativity." },
        { "황제",       "Symbolizes order, stability, and fatherly authority." },
        { "교황",       "Embodies spiritual guidance and moral authority." },
        { "연인",       "Signifies love, harmony, and partnership." },
        { "전차",       "Denotes willpower, control, and victory." },
        { "힘",         "Represents inner strength and courage." },
        { "은둔자",     "Symbolizes introspection and solitude." },
        { "운명의 수레바퀴", "Stands for fate, cycles, and change." },
        { "정의",       "Embodies fairness, truth, and balance." },
        { "매달린 남자",   "Represents suspension and new perspectives." },
        { "죽음",       "Signifies endings and transformative rebirth." },
        { "절제",       "Denotes balance, moderation, and healing." },
        { "악마",       "Represents temptation and material bondage." },
        { "탑",         "Symbolizes sudden upheaval and revelation." },
        { "별",         "Denotes hope, inspiration, and serenity." },
        { "달",         "Stands for illusion, fear, and intuition." },
        { "태양",       "Embodies joy, vitality, and success." },
        { "심판",       "Represents judgment, awakening, and renewal." },
        { "세계",       "Denotes completion, wholeness, and achievement." },
        { "부활",       "Signifies renewal and forgiveness." },
        { "생명",       "Represents growth, vitality, and abundance." },
        { "악동",       "Denotes mischief and playful spontaneity." },
        { "거울",       "Reflects on memory, repetition, and cycles." },
        { "연기",       "Symbolizes illusion and transience." },
        { "일식",       "Represents sudden change and shadow." },
        { "암거래",     "Denotes secret dealings and hidden exchanges." },
        { "불씨",       "Symbolizes spark, protection, and renewal." },
        { "저주받은 책", "Stands for hidden knowledge and peril." },
        { "예언자",     "Denotes foresight and spiritual messages." },
        { "종말의 경전", "Represents apocalypse and cosmic transformation." },
        { "강탈자",     "Symbolizes aggression and taking by force." },
        { "대천사",     "Denotes divine protection and intervention." },
        { "영혼의 초",   "Represents spiritual light and purification." },
        { "그림자의 균열","Symbolizes hidden fractures and secrets." },
        { "영혼 결혼식", "Denotes union, partnership, and spiritual bonds." },
        { "피의 서약",   "Embodies sacrifice and binding promises." },
        { "운명의 유희", "Represents chance, risk, and play." },
        { "꿈",         "Denotes visions, subconscious, and rest." },
    };

    public static string GetStory(string cardName)
    {
        return Story.TryGetValue(cardName, out var story) ? story : ".";
    }
}

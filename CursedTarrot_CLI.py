import random
import time
from itertools import chain


#---------------------------------------------------------------------------------------------------------------------
# GAME VALANCE ADJUSTMENT

REGULAR_CURSE_INCREASING_FREQUENCY = 5 # 정기적 저주 증가 주기
REGULAR_CURSE_INCREASING_INIT = 1 # 정기적 저주 증가량 초기값
REGULAR_CURSE_INCREASING_CUMULATE = 1 # 정기적 저주 증가량 누적값

#---------------------------------------------------------------------------------------------------------------------

class Card:
    def __init__(self, name, hp_change=0, curse_change=0, description="", special=None):
        self.name = name
        self.hp_change = hp_change
        self.curse_change = curse_change
        self.description = description
        self.special = special

    def apply(self, player, game, cards):
        if self.name == "세계": # 세계 카드의 경우, 모든 카드의 효과의 일반효과는 합산해서 적용하기 위해 구분
            self.special(player, game, cards)  

        else:
            if self.hp_change != 0:
                if self.hp_change > 0:
                    print(f"체력이 {self.hp_change} 증가합니다. : {player.hp} -> ", end="")
                else:
                    print(f"체력이 {-self.hp_change} 감소합니다. : {player.hp} -> ", end="")
                player.hp += self.hp_change
                print(f"{player.hp}")

            if self.curse_change != 0:
                if self.curse_change > 0:
                    print(f"저주가 {self.curse_change} 증가합니다. : {player.curse} -> ", end="")
                else:
                    print(f"저주가 {-self.curse_change} 감소합니다. : {player.curse} -> ", end="")
                player.curse += self.curse_change
                print(f"{player.curse}")

            if self.special:
                self.special(player, game, cards)
    
    def copy(self, **kwargs):
        new_card = Card(self.name, self.hp_change, self.curse_change, self.description, self.special)
        for key, value in kwargs.items():
            setattr(new_card, key, value)
        return new_card

class Player:
    def __init__(self):
        self._hp = 10
        self._curse = 0
        self.next_draw_num = 3
        self.skip_next_turn = False
        self.reroll_available = 0
        self.non_curse_damage_turn = 0
        self.non_curse_increase_turn = 0
        self.non_hp_increase_turn = 0
        self.non_curse_decrease_turn = 0
        self.non_hp_decrease_turn = 0
        self.delayed_effect_list = []
        self.next_pick_num = 1
        self.last_card = None
        self.not_add_death = 0
        self.ember = False
        self.archangel = False
        self.random_choice = False
    
    @property
    def curse(self):
        return self._curse

    @curse.setter
    def curse(self, value):
        self._curse = max(0, value)  # curse는 0 미만 불가

    @property
    def hp(self):
        return self._hp

    @hp.setter
    def hp(self, value):
        self._hp = max(0, value)  # hp도 0 미만 불가

class Game:
    def __init__(self):
        self.turn = 1
        self.player = Player()
        self.deck = self.generate_deck()

    def generate_deck(self):
        base_deck = [card for card in all_cards if card.name != "죽음"]
        random.shuffle(base_deck)
        return base_deck 

    def draw_cards(self, count):
        if len(self.deck) < count:
            self.deck += self.generate_deck()
        return [self.deck.pop(0) for _ in range(count)]

    def insert_death_cards(self, count):
        for _ in range(count):
            pos = random.randint(0, len(self.deck))
            self.deck.insert(pos, death)

#---------------------------------------------------------------------------------------------------------------------
# 카드 특수효과

def death_effect(player, game, other_cards):
    print("[죽음 효과] 사망합니다...")
    display_score(player, game)

def fool_effect(player, game, other_cards):
    print("[바보 효과] 체력을 10으로 초기화합니다!")
    player.hp = 10

def tower_effect(player, game, other_cards):
    print("[탑 효과] 다음 턴은 카드를 뽑지 않고 저주로 인한 체력감소가 이루어지지 않습니다!")
    player.skip_next_turn = True

def lovers_effect(player, game, other_cards):
    print("[연인 효과] 다음 턴에 리롤 가능!")
    player.reroll_available = player.reroll_available + 1

def revive_effect(player, game, other_cards):
    print("[부활 효과] 덱에서 죽음 카드 제거!")
    game.deck = [card for card in game.deck if card.name != "죽음"]

def life_effect(player, game, other_cards): 
    print("[생명 효과] 덱의 랜덤한 위치에 무작위 카드 20장 추가!")
    new_cards = game.generate_deck()[:20]
    random.shuffle(new_cards)
    for _ in range(20):
        pos = random.randint(0, len(game.deck))
        game.deck.insert(pos, new_cards.pop())

def fortune_wheel_effect(player, game, other_cards):
    print(f"[운명의 수레바퀴 효과] 체력과 저주를 5:1 비율로 맞바꿉니다! : {player.hp}, {player.curse} -> ", end="")
    player.hp, player.curse = player.curse * 5, player.hp // 5
    print(f"{player.hp}, {player.curse}")

def hanged_man_effect(player, game, other_cards):
    print("[매달린 남자 효과] 다음 턴에 2장만 뽑습니다!")
    player.next_draw_num = 2

def judgement_effect(player, game, other_cards):
    print("[심판 효과] 덱에서 무작위 카드 5장 제거!")
    if len(game.deck) > 5:
        for _ in range(5):
            pos = random.randint(0, len(game.deck) - 1)
            game.deck.pop(pos)
    else:
        game.deck = []

def temperance_effect(player, game, other_cards):
    print("[절제 효과] 이번을 포함해 3턴 동안 저주로 인한 체력 감소가 이루어지지 않습니다!")
    player.non_curse_damage_turn = 3

def scamp_effect(player, game, other_cards):
    print("[악동 효과] 체력을 1~10 중 무작위로 증가시킵니다!")
    random_value = random.randint(1, 10)
    print(f"체력이 {random_value} 증가합니다.")
    player.hp += random_value

def hierophant_effect(player, game, other_cards):
    print("[교황 효과] 다음 3턴 동안 일반효과로 저주 감소 불가!")
    player.non_curse_decrease_turn = 3

def hermit_effect(player, game, other_cards):
    player.delayed_effect_list.append((5, "은둔자", lambda p: setattr(p, 'hp', p.hp + 7)))

def magician_effect(player, game, other_cards):
    player.delayed_effect_list.append((3, "마법사", lambda p: setattr(p, 'curse', p.curse - 3)))

def high_priestess_effect(player, game, other_cards):
    print("[여교황 효과] 다음 3턴 동안 일반효과로 체력 증가 불가!")
    player.non_hp_increase_turn = 3

def empress_effect(player, game, other_cards):
    print("[여제 효과] 덱에서 죽음 카드 5장 제거!")
    death_count = game.deck.count(death)
    if death_count > 5:
        for _ in range(5):
            pos = random.choice([i for i, card in enumerate(game.deck) if card.name == "죽음"])
            game.deck.pop(pos)
    else:
        game.deck = [card for card in game.deck if card.name != "죽음"]

def emperor_effect(player, game, other_cards):
    print("[황제 효과] 체력과 저주를 모두 두 배로 만듭니다!")
    player.hp *= 2
    player.curse *= 2

def chariot_effect(player, game, other_cards):
    print("[전차 효과] 다음 턴에 두 장의 카드를 고릅니다!")
    player.next_pick_num = 2

def justice_effect(player, game, other_cards):
    print(f"[정의 효과] 체력과 저주를 합해 5:1 비율로 나눕니다! : {player.hp}, {player.curse} -> ", end="")
    total = player.hp + player.curse
    player.curse = total // 6
    player.hp = total - player.curse
    print(f"{player.hp}, {player.curse}")

def world_effect(player, game, other_cards):
    print("[세계 효과] 이번 턴에 뽑은 모든 카드의 효과를 적용합니다!")

    hp_change = 0
    curse_change = 0

    for selected in chain([world], other_cards):

        # 일반 효과의 체력 및 저주 변경 제한
        if player.non_curse_increase_turn > 0:
            if selected.curse_change > 0:
                selected = selected.copy(curse_change=0)
        if player.non_hp_increase_turn > 0:
            if selected.hp_change > 0:
                selected = selected.copy(hp_change=0)
        if player.non_curse_decrease_turn > 0:
            if selected.curse_change < 0:
                selected = selected.copy(curse_change=0)
        if player.non_hp_decrease_turn > 0:
            if selected.hp_change < 0:
                selected = selected.copy(hp_change=0)

        hp_change += selected.hp_change
        curse_change += selected.curse_change

    print(f"카드들의 체력과 저주 변화를 합산하여 적용합니다! : {player.hp}, {player.curse} -> ", end="")
    player.hp += hp_change
    player.curse += curse_change
    print(f"{player.hp}, {player.curse}")

    for card in other_cards:
        if card.special:
            print(f"'{card.name}' 카드의 특수 효과를 적용합니다.")
            card.special(player, game, other_cards)

def mirror_effect(player, game, other_cards):
    if player.last_card is None:
        print("[거울 효과] 마지막에 사용한 카드가 없습니다.")
    else:
        print(f"[거울 효과] 마지막에 사용한 카드 '{player.last_card.name}'의 효과를 다시 발동합니다!")
        player.last_card.apply(player, game, other_cards)

def eclipse_effect(player, game, other_cards):
    player.delayed_effect_list.append((2, "일식", lambda p: setattr(p, 'curse', p.curse + 2)))

def black_market_effect(player, game, other_cards):
    print("[암거래 효과] 이번을 포함해 5턴 동안 죽음 카드가 덱에 추가되지 않습니다!")
    player.not_add_death = 5

def ember_effect(player, game, other_cards):
    print("[불씨 효과] 다음 한 번 턴 종료시 체력이 1 이하로 떨어지게 될 경우 체력을 1로 고정하고 저주를 0으로 변경합니다!")
    player.ember = True

def prophet_effect(player, game, other_cards):
    print("[예언자 효과] 다음 턴에 선택한 카드는 일반 효과로 체력은 감소시키지 않고 저주는 증가시키지 않습니다!")
    player.non_hp_decrease_turn = 1
    player.non_curse_increase_turn = 1

def apocalypse_scripture_effect(player, game, other_cards):
    print("[종말의 경전 효과] 덱을 리셋 시킵니다!")
    game.deck = game.generate_deck()

def archangel_effect(player, game, other_cards):
    print("[대천사 효과] 다음 턴에 죽음 카드가 뽑힐 시, 해당 카드를 랜덤한 카드로 교체합니다!")
    player.archangel = True

def soul_candle_effect(player, game, other_cards):
    player.delayed_effect_list.append((3, "영혼의 초", lambda p: setattr(p, 'curse', p.curse - 2)))

def soul_wedding_effect(player, game, other_cards):
    print("[영혼 결혼식 효과] 다시 뽑기 기회를 1회 얻습니다!")
    player.reroll_available += 1

def blood_pact_effect(player, game, other_cards):
    print("[피의 서약 효과] 다음 2턴간 일반 효과로 저주가 증가하지 않습니다!")
    player.non_curse_increase_turn = 2

def gamble_of_fate_effect(player, game, other_cards):
    print("[운명의 유희 효과] 다음 턴, 카드가 무작위로 선택됩니다!")
    player.random_choice = True



#---------------------------------------------------------------------------------------------------------------------
# 카드 목록

#0
fool = Card("바보", 0, 0, "체력을 초기 값인 10으로 만든다.", special=fool_effect)
magician = Card("마법사", 0, 1, "3턴 뒤 저주를 3 감소시킨다.", special=magician_effect)
high_priestess = Card("여교황", 4, 0, "다음 3턴동안 일반 효과로 체력을 증가시킬 수 없다.", special=high_priestess_effect)
empress = Card("여제", 1, 0, "덱에 있는 죽음 카드 5장을 제거한다.", special=empress_effect)
emperor = Card("황제", 0, 0, "체력과 저주를 모두 두 배로 만든다.", special=emperor_effect)

#5
hierophant = Card("교황", 4, 0, "다음 3턴 동안은 일반 효과로 저주를 감소시킬 수 없다.", special=hierophant_effect)
lovers = Card("연인", 1, 1, "다시 뽑기 기회를 1회 얻는다. 다시 뽑기는 3장을 모두 버리고 새롭게 3장을 뽑는다.", special=lovers_effect)
chariot = Card("전차", 4, 0, "다음 턴에는 두 장의 카드를 고른다.(두장 모두 효과 적용)", special=chariot_effect)
strength = Card("힘", 1, 0, "")
hermit = Card("은둔자", -3, 0, "5턴 뒤 체력을 7 증가시킨다.", special=hermit_effect)

#10
fortune_wheel = Card("운명의 수레바퀴", 0, 0, "체력과 저주를 5:1 비율로 맞바꾼다.", special=fortune_wheel_effect)
justice = Card("정의", 0, 0, "체력과 저주를 합해 5:1 비율로 나눈다.", special=justice_effect)
hanged_man = Card("매달린 남자", 5, 0, "다음 턴에는 3장이 아닌 2장의 카드 만을 뽑는다.", special=hanged_man_effect)
death = Card("죽음", 0, 0, "사망한다.", special=death_effect)
temperance = Card("절제", -5, 0, "이번을 포함해 3턴 동안 저주로 인한 체력 감소가 이루어지지 않습니다.", special=temperance_effect)

#15
devil = Card("악마", 20, 5, "")
tower = Card("탑", -1, 0, "다음 턴은 카드를 뽑지 않고 저주로 인한 체력감소가 이루어지지 않습니다.", special=tower_effect)
star = Card("별", 2, 0, "")
moon = Card("달", -10, -2, "")
sun = Card("태양", 10, 2, "")

#20
judgement = Card("심판", -3, -1, "덱에 있는 무작위 카드 5장을 제거한다.", special=judgement_effect)
world = Card("세계", 7, -3, "이번 턴에 뽑은 모든 카드의 효과를 적용한다.", special=world_effect)
revive = Card("부활", -1, 0, "덱에 있는 모든 죽음 카드를 삭제한다.", special=revive_effect)
life = Card("생명", 0, 0, "덱에 죽음을 제외한 각기 다른 무작위 카드 20장을 추가한다.", special=life_effect)
scamp = Card("악동", 0, 1, "체력을 1~10 중 무작위로 증가시킨다", special=scamp_effect)

#25
mirror = Card("거울", 0, 1, "마지막에 사용한 카드의 효과를 다시 발동한다.", special=mirror_effect)
smoke = Card("연기", 0, 0, "")
eclipse = Card("일식", 8, 0, "2턴 뒤에 저주를 2 증가시킨다.", special=eclipse_effect)
black_market = Card("암거래", 0, 2, "이번을 포함하여 5턴 동안 죽음 카드가 덱에 추가되지 않는다.", special=black_market_effect)
ember = Card("불씨", -1, 1, "다음 한 번 턴 종료시 체력이 1 이하로 떨어지게 될 경우 체력을 1로 고정하고 저주를 0으로 변경한다.", special=ember_effect)

#30
cursed_book = Card("저주받은 책", 0, 1, "")
prophet = Card("예언자", -3, 1, "다음 턴에 선택한 카드는 일반 효과로 체력은 감소시키지 않고 저주는 증가시키지 않는다. (특수효과 제외)", special=prophet_effect)
apocalypse_scripture = Card("종말의 경전", 0, 3, "덱을 리셋 시킨다. ", special=apocalypse_scripture_effect)
plunderer = Card("강탈자", -3, 0, "")
archangel = Card("대천사", 1, -1, "다음 턴에 죽음 카드가 뽑힐 시, 해당 카드를 랜덤한 카드로 교체한다.", special=archangel_effect)

#35
soul_candle = Card("영혼의 초", 5, 2, "3턴 뒤, 저주를 2 감소시킨다.", special=soul_candle_effect)
crack_of_shadow = Card("그림자의 균열", -1, 1, "")
soul_wedding = Card("영혼 결혼식", 6, 3, "다시 뽑기 기회를 1회 얻는다. 다시 뽑기는 3장을 모두 버리고 새롭게 3장을 뽑는다.", special=soul_wedding_effect)
blood_pact = Card("피의 서약", -10, 0, "다음 2턴간 일반 효과로 저주가 증가하지 않는다.", special=blood_pact_effect)
gamble_of_fate = Card("운명의 유희", 5, 0, "다음 턴, 카드가 무작위로 선택 된다.", special=gamble_of_fate_effect)

#40
dream = Card("꿈", -4, -1, "")


#---------------------------------------------------------------------------------------------------------------------

all_cards = [death, 
            fool, tower, lovers, sun, moon, star, strength, devil, revive, life, 
            fortune_wheel, hanged_man, judgement, temperance, scamp, hierophant, hermit, magician, high_priestess, empress, 
            emperor, chariot, justice, world, mirror, smoke, eclipse, black_market, ember, cursed_book, 
            prophet, apocalypse_scripture, plunderer, archangel, soul_candle, crack_of_shadow, soul_wedding, blood_pact, gamble_of_fate, dream]

#---------------------------------------------------------------------------------------------------------------------

def play_game():
    game = Game()
    player = game.player

    while game.turn <= 40 and player.hp > 0:

        if game.turn != 1:
            time.sleep(2)

        isSkipped = False

        print(f"\n=== Turn {game.turn} ===")
        print(f"HP: {player.hp} | Curse: {player.curse} | Deck: {len(game.deck)} cards left | ember: {player.ember}")

        if player.skip_next_turn:
            print("이번 턴은 스킵됩니다.")
            player.skip_next_turn = False
            isSkipped = True
        else:
            draw_count = player.next_draw_num
            player.next_draw_num = 3  # Reset to default for next turn
            cards = game.draw_cards(draw_count)

            print("카드를 뽑습니다", end="")
            time.sleep(1) # 1초 대기
            print(". ", end="")
            time.sleep(1) # 1초 대기
            print(". ", end="")
            time.sleep(1) # 1초 대기
            print(".")
            time.sleep(1) # 1초 대기
            while True:
                for i, card in enumerate(cards):
                    if player.archangel and card.name == "죽음": # 대천사 효과 검토
                        card = random.choice([c for c in all_cards if c.name != "죽음"])
                        print(f"[대천사 효과] {i+1}번째 카드가 '{card.name}' 카드로 교체되었습니다.")
                        cards[i] = card # 대천사 효과 적용
                    print(f"{i+1}. {card.name} (HP {card.hp_change}, Curse {card.curse_change}) - {card.description}")
                player.archangel = False # 대천사 효과 초기화

                time.sleep(1) # 1초 대기

                if player.reroll_available>0:
                    print(f"리롤 기회가 {player.reroll_available}회 남았습니다.")
                    reroll = input("리롤하시겠습니까? (y/n): ").lower()
                    if reroll == 'y':
                        cards = game.draw_cards(draw_count)
                        print("카드를 다시 뽑습니다:")
                        player.reroll_available -= 1
                        continue 
                break
            
            pick_count = player.next_pick_num
            player.next_pick_num = 1  # Reset to default for next turn
            selected_cards = []

            #랜덤 카드 선택 검토
            if player.random_choice:
                time.sleep(2) # 2초 대기
                selected = random.choice(cards)
                selected_cards.append(selected)
                cards.remove(selected)
                print(f"{selected.name} 카드가 무작위로 선택됨!")
                player.random_choice = False

            else:
                if pick_count == 1:
                    while True:
                        try:
                            choice = int(input(f"카드를 선택하세요 (1~{len(cards)}): ")) - 1
                            if 0 <= choice < len(cards):
                                break
                            else:
                                print("유효하지 않은 선택입니다. 다시 입력하세요.")
                        except ValueError:
                            print("숫자를 입력하세요.")
                    selected = cards.pop(choice)
                    selected_cards.append(selected)
                    print(f"'{selected.name}' 선택됨!")

                elif pick_count == 2:
                    print("이번 턴에는 카드를 2장 선택합니다.")
                    for _ in range(2):
                        if _ == 0:
                            print("첫번째 ", end="")
                        if _ == 1:
                            for i, card in enumerate(cards):
                                print(f"{i+1}. {card.name} (HP {card.hp_change}, Curse {card.curse_change}) - {card.description}")
                            print("두번째 ", end="")
                        while True:
                            try:
                                choice = int(input(f"카드를 선택하세요 (1~{len(cards)}): ")) - 1
                                if 0 <= choice < len(cards):
                                    break
                                else:
                                    print("유효하지 않은 선택입니다. 다시 입력하세요.")
                            except ValueError:
                                print("숫자를 입력하세요.")
                        selected = cards.pop(choice)
                        selected_cards.append(selected)
                        print(f"'{selected.name}' 선택됨!")

            for selected in selected_cards:

                # 일반 효과의 체력 및 저주 변경 제한
                if player.non_curse_increase_turn > 0:
                    if selected.curse_change > 0:
                        selected = selected.copy(curse_change=0)
                    player.non_curse_increase_turn -= 1
                if player.non_hp_increase_turn > 0:
                    if selected.hp_change > 0:
                        selected = selected.copy(hp_change=0)
                    player.non_hp_increase_turn -= 1
                if player.non_curse_decrease_turn > 0:
                    if selected.curse_change < 0:
                        selected = selected.copy(curse_change=0)
                    player.non_curse_decrease_turn -= 1    
                if player.non_hp_decrease_turn > 0:
                    if selected.hp_change < 0:
                        selected = selected.copy(hp_change=0)
                    player.non_hp_decrease_turn -= 1

                # Apply the selected card's effect
                selected.apply(player, game, cards) #cards: not chosen cards
                
                player.last_card = selected # Save the last card used

        # Apply delayed effects
        if player.delayed_effect_list: 
            new_delayed_effect_list = []
            for delay, name, effect in player.delayed_effect_list:
                delay -= 1
                if delay >= 0:
                    new_delayed_effect_list.append((delay, name, effect))
                else:
                    print(f"[지연 효과 발동] {name} 효과가 발동합니다. : {player.hp}, {player.curse} -> ", end="")
                    effect(player)
                    print(f"{player.hp}, {player.curse}")
            player.delayed_effect_list = new_delayed_effect_list

        # Apply curse damage
        if player.non_curse_damage_turn > 0:
            player.non_curse_damage_turn -= 1
        else:
            if isSkipped:
                print("이번 턴은 스킵되었습니다. 저주 피해 없음.")
            else:
                if player.curse > 0:
                    print(f"저주로 인해 체력이 {player.curse} 감소합니다. : {player.hp} -> ", end="")
                    player.hp -= player.curse
                    print(f"{player.hp}")

        # Insert death card to deck if curse >= 6
        if player.not_add_death > 0:
            player.not_add_death -= 1
        else:
            if player.curse >= 6:
                print(f"저주가 {player.curse}이므로 죽음 카드 {player.curse - 5}장이 덱에 추가됩니다.")
                game.insert_death_cards(player.curse - 5)

        # 정기적으로 일정 턴마다 저주 증가
        if game.turn % REGULAR_CURSE_INCREASING_FREQUENCY == 0:
            increase_amount = REGULAR_CURSE_INCREASING_INIT + (game.turn // REGULAR_CURSE_INCREASING_FREQUENCY - 1) * REGULAR_CURSE_INCREASING_CUMULATE            
            print(f"{game.turn}턴 종료로 저주가 {increase_amount} 증가합니다. : {player.curse} -> ", end="")
            player.curse += increase_amount
            print(f"{player.curse}")

        # Check ember effect
        if player.ember and player.hp <= 1:
            print("[불씨 효과 발동] 체력이 1, 저주가 0으로 변경됩니다.")
            player.hp = 1
            player.curse = 0
            player.ember = False

        game.turn += 1

    if player.hp <= 0:
        print("체력이 0이 되어 사망합니다.")
        display_score(player, game)
    else:
        print("생존! 승리!")
        display_score(player, game)

def display_score(player, game):
    print("\n=== 최종 점수 ===")
    print(f"최종 체력: {player.hp}")
    print(f"최종 저주: {player.curse}")
    print(f"최종 층수: {game.turn - 1}층")
    exit()

#---------------------------------------------------------------------------------------------------------------------

if __name__ == "__main__":
    play_game()

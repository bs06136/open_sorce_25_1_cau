import pygame
import sys

pygame.init()

# 화면 설정
screen_width, screen_height = 768, 1152
screen = pygame.display.set_mode((screen_width, screen_height))
pygame.display.set_caption("저주의 꿈")

# 배경 이미지 로드
background = pygame.image.load("title_screen.png")
background = pygame.transform.scale(background, (screen_width, screen_height))

# 폰트 설정
pygame.font.init()
title_font = pygame.font.SysFont("malgungothic", 60)  # 한글 지원
sub_font = pygame.font.SysFont("malgungothic", 40)

# 버튼 위치 (이미지 기준 정확히 조정된 값)
buttons = {
    "start": pygame.Rect(210, 590, 350, 120),
    "card": pygame.Rect(210, 735, 350, 120),
    "exit": pygame.Rect(210, 880, 350, 120),
    "gear": pygame.Rect(610, 919, 80, 80)
}

# 상태 변수: 메인 메뉴인지, 게임 화면인지
game_state = "main_menu"

def draw_button_highlight(rect, hover):
    if hover:
        glow = pygame.Surface((rect.width, rect.height), pygame.SRCALPHA)
        glow.fill((255, 255, 255, 60))
        screen.blit(glow, rect.topleft)

# 게임 루프
running = True
while running:
    mouse_pos = pygame.mouse.get_pos()

    if game_state == "main_menu":
        # 메인 메뉴 화면
        screen.blit(background, (0, 0))

        for key, rect in buttons.items():
            draw_button_highlight(rect, rect.collidepoint(mouse_pos))

    elif game_state == "in_game":
        # 검은 배경 + 문구 출력
        screen.fill((0, 0, 0))  # 검은색
        title_text = title_font.render("탑에 들어갑니다", True, (255, 255, 255))
        sub_text = sub_font.render("1 turn", True, (200, 200, 200))
        screen.blit(title_text, (screen_width // 2 - title_text.get_width() // 2, 450))
        screen.blit(sub_text, (screen_width // 2 - sub_text.get_width() // 2, 550))

    pygame.display.flip()

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

        elif event.type == pygame.MOUSEBUTTONDOWN and game_state == "main_menu":
            if buttons["start"].collidepoint(mouse_pos):
                print("✅ 게임 시작!")
                game_state = "in_game"  # 상태 변경
            elif buttons["card"].collidepoint(mouse_pos):
                print("📜 카드 종류 보기")
            elif buttons["exit"].collidepoint(mouse_pos):
                print("❌ 게임 종료")
                running = False
            elif buttons["gear"].collidepoint(mouse_pos):
                print("⚙ 설정 열기")

pygame.quit()
sys.exit()

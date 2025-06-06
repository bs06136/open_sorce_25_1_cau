# 🌌 저주받은 타로: 타워 오브 페이트

> 깊고 깊은 숲 속, 잊혀진 검은 탑의 시련이 당신을 기다립니다.

## 👥 팀 정보

| 항목     | 내용 |
|----------|------|
| **팀원**   |배태현 (20185412)<br> 김태우 (20192317)<br>송민석 (20194526)<br>정어진 (20185810)<br>가오즈항 (20196837) |


## 🧠 게임 소개

인간의 발길이 닿지 않은 숲 한가운데, 고요히 솟아오른 검은 탑이 있습니다. 안개 속에 숨겨진 이 탑 안에는, 금단의 힘 **“저주받은 타로”**가 잠들어 있습니다.  
플레이어는 탑의 시련 속에서 **타로 카드를 통해 생존을 도모하고, 저주를 관리하며**, 점차 무너지는 정신과 육체를 붙잡아야 합니다.  
**40턴 동안 생존**하는 것이 유일한 목표이자, 유일한 희망입니다.

---

## 🖼️ 게임 UI 미리보기

> 메인 타이틀 화면 – "저주의 꿈"

<img src="asset/title_screen.png" alt="저주의 꿈 메인 UI" width="500"/>


---

## 🎯 게임 목표

- **목표 턴(40턴)** 동안 생존
- 타로 카드를 이용해 **체력과 저주**를 관리
- **덱 조작 및 전략적 선택**으로 점점 강화되는 시련을 극복

---

## ⚙️ 게임 기본 자원

| 자원명    | 초기값 | 설명                                     |
|---------|------|----------------------------------------|
| 체력 (HP) | 10   | 매 턴 저주 수치만큼 피해. 0이 되면 즉시 게임 종료     |
| 저주 (Curse) | 0   | 5턴마다 자동 +1. 카드 효과에 따라 증감 가능          |
| 카드 덱    | 40장  | 매 턴 3장 랜덤 등장. 카드 효과로 추가/삭제 가능       |

---

## 🔁 턴 진행 방식

1. **카드 뽑기**: 기본 3장의 카드가 랜덤 등장  
2. **카드 선택**: 1장을 선택  
3. **효과 실행**: 카드의 효과 즉시 적용 (HP/저주/덱 변화)  
4. **저주 피해**: 턴 종료 시 HP -= 현재 저주 수치  
5. **저주 증가**: 매 5턴마다 자동으로 +1  
6. **죽음 카드 삽입**: 저주 ≥ 6 → 매 턴 `저주 - 5` 장의 죽음 카드가 덱에 추가됨  

---

## 🃏 카드 유형

### ① 생존형 카드
- 예시: 태양, 성배, 부활, 불씨  
- **효과**: 체력 회복, 죽음 카드 제거 등  

### ② 정화형 카드
- 예시: 고탑, 심판, 여제, 마법사  
- **효과**: 저주 수치 감소, 죽음 카드 제거  

### ③ 조작형 카드
- 예시: 별, 연인, 세계, 거울  
- **효과**: 카드 수 변화, 다음 턴 영향, 효과 복제 등  

### ④ 고위험 고보상 카드
- 예시: 악마, 황제, 운명의 수레바퀴  
- **효과**: 체력 대량 회복 vs 저주 대폭 증가  

### ⑤ 덱 조정형 카드
- 예시: 지팡이, 생명, 봉인 등  
- **효과**: 카드 추가/삭제/덱 재편  

---

## ⛓️ 저주 압박 시스템

- **5턴마다 저주 +1** (회피 불가)
- **저주 ≥ 6일 때**, 매 턴 [저주 - 5]장의 죽음 카드 삽입
- 시간이 지날수록 **덱이 오염**되고, 생존 난이도 급상승

---

## ⏳ 카드 효과 타입

| 타입     | 설명                                               |
|--------|--------------------------------------------------|
| 즉시형   | 선택 시 즉시 발동                                     |
| 지연형   | 일정 턴 후 발동 (예: 3턴 후 저주 -3)                   |
| 조건형   | 특정 조건 만족 시 발동 (예: HP 1 이하일 때 부활)        |

---

## 🧠 핵심 전략 포인트

- **저주 6 이상 절대 금지**: 죽음 카드가 쌓이면 치명적  
- **덱 정리 우선**: 죽음 카드 삽입 전 불필요 카드 정리  
- **카드 연계 사용**: `거울 + 세계` 조합 등 폭발적 효과 가능  
- **장기 플랜 필수**: 지연형 카드 활용 위한 턴별 추적 필요  
- **고위험 카드는 타이밍 중요**: 체력 여유 있을 때 `악마`, `황제` 활용

---

## 🏁 결말 시스템

| 생존 턴        | 저주 조건     | 결말 유형             |
|--------------|------------|---------------------|
| 1~9턴         | 무관         | 통제불능의 영혼          |
| 10~19턴       | 저주 ≥ 10   | 심연으로 추락           |
| 10~19턴       | 저주 < 10   | 생존자                |
| 20턴 이상      | 무관         | 운명 파괴자            |

---

## 📜 라이선스



## 카드 이미지

| 0 | 1 | 2 | 3 | 4 |
|--|--|--|--|--|
| [![](https://github.com/user-attachments/assets/7f6b6203-f196-4873-9412-d0ea26d19c62)](https://github.com/user-attachments/assets/7f6b6203-f196-4873-9412-d0ea26d19c62) | [![](https://github.com/user-attachments/assets/83a06330-bda9-4c69-aaf6-8a77b29979dc)](https://github.com/user-attachments/assets/83a06330-bda9-4c69-aaf6-8a77b29979dc) | [![](https://github.com/user-attachments/assets/4f52e768-5b23-48c5-aec3-09695b829b6e)](https://github.com/user-attachments/assets/4f52e768-5b23-48c5-aec3-09695b829b6e) | [![](https://github.com/user-attachments/assets/35ca76fa-acc6-4a83-b79d-5c8f55955872)](https://github.com/user-attachments/assets/35ca76fa-acc6-4a83-b79d-5c8f55955872) | [![](https://github.com/user-attachments/assets/f3d5bce4-fc58-4b49-8adc-7bd704ef7eda)](https://github.com/user-attachments/assets/f3d5bce4-fc58-4b49-8adc-7bd704ef7eda) |

| 5 | 6 | 7 | 8 | 9 |
|--|--|--|--|--|
| [![](https://github.com/user-attachments/assets/87a3607c-ea96-4d83-9f29-f2694440e993)](https://github.com/user-attachments/assets/87a3607c-ea96-4d83-9f29-f2694440e993) | [![](https://github.com/user-attachments/assets/eb4a6ec6-084e-41bb-944a-830e3756974a)](https://github.com/user-attachments/assets/eb4a6ec6-084e-41bb-944a-830e3756974a) | [![](https://github.com/user-attachments/assets/1a850165-939c-4d3e-b1c4-a949f28a4bb5)](https://github.com/user-attachments/assets/1a850165-939c-4d3e-b1c4-a949f28a4bb5) | [![](https://github.com/user-attachments/assets/4226c93d-d92a-4638-8ce7-39376b57d26a)](https://github.com/user-attachments/assets/4226c93d-d92a-4638-8ce7-39376b57d26a) | [![](https://github.com/user-attachments/assets/82dfaa82-0507-41e4-b6c3-e1dc021ead6e)](https://github.com/user-attachments/assets/82dfaa82-0507-41e4-b6c3-e1dc021ead6e) |

| 10 | 11 | 12 | 13 | 14 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/486aca67-1785-460d-abc0-23fba7c50a35)](https://github.com/user-attachments/assets/486aca67-1785-460d-abc0-23fba7c50a35) | [![](https://github.com/user-attachments/assets/d5c9195c-af8a-48ae-80bb-56ebc83671cf)](https://github.com/user-attachments/assets/d5c9195c-af8a-48ae-80bb-56ebc83671cf) | [![](https://github.com/user-attachments/assets/24c6581b-33fc-42ea-a635-f8c0f6071060)](https://github.com/user-attachments/assets/24c6581b-33fc-42ea-a635-f8c0f6071060) | [![](https://github.com/user-attachments/assets/9cf0b1d3-5c33-4150-85c8-9cade5dfc900)](https://github.com/user-attachments/assets/9cf0b1d3-5c33-4150-85c8-9cade5dfc900) | [![](https://github.com/user-attachments/assets/0c15de86-bfae-4dfc-ad78-882a342df3aa)](https://github.com/user-attachments/assets/0c15de86-bfae-4dfc-ad78-882a342df3aa) |

| 15 | 16 | 17 | 18 | 19 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/7079d653-0dbc-440d-a3df-6b3b51b2ce02)](https://github.com/user-attachments/assets/7079d653-0dbc-440d-a3df-6b3b51b2ce02) | [![](https://github.com/user-attachments/assets/369fd101-d3f7-4799-98f2-3b93298f7158)](https://github.com/user-attachments/assets/369fd101-d3f7-4799-98f2-3b93298f7158) | [![](https://github.com/user-attachments/assets/0fef02b3-4fd4-46f7-8a7a-809c03423c67)](https://github.com/user-attachments/assets/0fef02b3-4fd4-46f7-8a7a-809c03423c67) | [![](https://github.com/user-attachments/assets/09f4cad1-1c91-46b9-b4c3-2cb80cd043e1)](https://github.com/user-attachments/assets/09f4cad1-1c91-46b9-b4c3-2cb80cd043e1) | [![](https://github.com/user-attachments/assets/6e7d67b1-cb1f-4f7e-98d1-50930fa23455)](https://github.com/user-attachments/assets/6e7d67b1-cb1f-4f7e-98d1-50930fa23455) |

| 20 | 21 | 22 | 23 | 24 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/acb37763-fa63-4270-9f1a-9bac05bc4815)](https://github.com/user-attachments/assets/acb37763-fa63-4270-9f1a-9bac05bc4815) | [![](https://github.com/user-attachments/assets/4c40e63b-a728-4196-9310-122559c5a8ae)](https://github.com/user-attachments/assets/4c40e63b-a728-4196-9310-122559c5a8ae) | [![](https://github.com/user-attachments/assets/8479def0-d267-4dc4-9de4-d285a4e52e52)](https://github.com/user-attachments/assets/8479def0-d267-4dc4-9de4-d285a4e52e52) | [![](https://github.com/user-attachments/assets/df4682b5-abf1-4fe5-acca-acba0b1ae483)](https://github.com/user-attachments/assets/df4682b5-abf1-4fe5-acca-acba0b1ae483) | [![](https://github.com/user-attachments/assets/39e754cc-7903-4837-aad1-e463093ea075)](https://github.com/user-attachments/assets/39e754cc-7903-4837-aad1-e463093ea075) |

| 25 | 26 | 27 | 28 | 29 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/e2f21330-4eb1-4a15-97ab-84e255fc4a5c)](https://github.com/user-attachments/assets/e2f21330-4eb1-4a15-97ab-84e255fc4a5c) | [![](https://github.com/user-attachments/assets/7a899356-fd4f-4bf0-999c-7931aa61e8b7)](https://github.com/user-attachments/assets/7a899356-fd4f-4bf0-999c-7931aa61e8b7) | [![](https://github.com/user-attachments/assets/c58c38bc-4f6a-4b4b-b6ba-3444ad6270d9)](https://github.com/user-attachments/assets/c58c38bc-4f6a-4b4b-b6ba-3444ad6270d9) | [![](https://github.com/user-attachments/assets/dd9807fe-12d9-4236-8018-f989a75929da)](https://github.com/user-attachments/assets/dd9807fe-12d9-4236-8018-f989a75929da) | [![](https://github.com/user-attachments/assets/36c76d4d-8feb-4c9c-8e3d-a3dfbcf25557)](https://github.com/user-attachments/assets/36c76d4d-8feb-4c9c-8e3d-a3dfbcf25557) |

| 30 | 31 | 32 | 33 | 34 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/9e818ac6-f520-44e7-927b-25fa51ee067e)](https://github.com/user-attachments/assets/9e818ac6-f520-44e7-927b-25fa51ee067e) | [![](https://github.com/user-attachments/assets/32dc6d70-23cc-40c5-bc79-fe8e4710646d)](https://github.com/user-attachments/assets/32dc6d70-23cc-40c5-bc79-fe8e4710646d) | [![](https://github.com/user-attachments/assets/206fc37f-f139-4955-b46c-a031b99fcd67)](https://github.com/user-attachments/assets/206fc37f-f139-4955-b46c-a031b99fcd67) | [![](https://github.com/user-attachments/assets/5c7601f5-4b4b-446c-adad-1aefa84a0aef)](https://github.com/user-attachments/assets/5c7601f5-4b4b-446c-adad-1aefa84a0aef) | [![](https://github.com/user-attachments/assets/4119860d-5362-41e4-ad5d-b96bf27a3a5e)](https://github.com/user-attachments/assets/4119860d-5362-41e4-ad5d-b96bf27a3a5e) |

| 35 | 36 | 37 | 38 | 39 |
|----|----|----|----|----|
| [![](https://github.com/user-attachments/assets/0557ad70-443f-4766-99cf-6f19150f4ae7)](https://github.com/user-attachments/assets/0557ad70-443f-4766-99cf-6f19150f4ae7) | [![](https://github.com/user-attachments/assets/3c693073-9d12-4b8b-aa6f-59de5b7dbd8e)](https://github.com/user-attachments/assets/3c693073-9d12-4b8b-aa6f-59de5b7dbd8e) | [![](https://github.com/user-attachments/assets/8283bb89-0bdf-4521-aece-999a49b95361)](https://github.com/user-attachments/assets/8283bb89-0bdf-4521-aece-999a49b95361) | [![](https://github.com/user-attachments/assets/d32e3f0e-abaf-44aa-910d-bf932e33fcdd)](https://github.com/user-attachments/assets/d32e3f0e-abaf-44aa-910d-bf932e33fcdd) | [![](https://github.com/user-attachments/assets/04b2928e-442a-4f8d-9bcf-49c515158545)](https://github.com/user-attachments/assets/04b2928e-442a-4f8d-9bcf-49c515158545) |

| 40 |   |   |   |   |
|----|---|---|---|---|
| [![](https://github.com/user-attachments/assets/74d2f6ff-9cf1-4ec1-80ad-b55c3c47c181)](https://github.com/user-attachments/assets/74d2f6ff-9cf1-4ec1-80ad-b55c3c47c181) |  |  |  |  |



# 🃏 OSSW 팀 프로젝트 기획안 - 저주받은 타로

---

## 📌 프로젝트 개요

- **프로젝트 분류**: 게임 개발
- **게임 이름**: 저주받은 타로
- **장르**: 싱글플레이 전략 턴제 카드게임
- **플랫폼**: 모바일 Android
- **팀원 수**: 5명
- **개발 기간**: 약 10주

---

## 🌌 저주받은 타로


### 🧠 게임 배경 스토리

깊고 깊은 곳, 인간의 발길이 닿지 않은 울창한 숲 한가운데.
하늘 끝까지 솟은 검은 탑이 고요하게 홀로 서 있다.

그 주위엔 안개가 짙게 깔려 있으며, 나뭇잎 사이로 스며드는 빛조차 꿈결처럼 흐릿하다.
어딘가에서 들려오는 작고 낮은 속삭임과 숲의 숨결은, 이 곳을 한없이 몽환적이고 스산한 기운으로 물들인다.
탑은 시간조차 잊은 채 존재해왔으며, 그 안에는 **저주받은 타로**라 불리는 금단의 힘이 잠들어 있다.
그리고 탑 끝에 다다르는 자는, 그 금단의 힘을 자신의 것으로 만들 것이라 전해진다.
탑의 각 층은 현실과 꿈의 경계가 일그러진 세계로 이루어져 있고, 그곳에 발을 들인 자는 탑이 내리는 시련을 통해 저주받은 타로의 정체를 마주하게 될 것이다.
시련은 그에서 선택을 강요하며 탑을 오를수록 점점 더 깊고, 농밀하게 이들을 갉아먹고 잠식시킨다.
그리고 마침내, 선택의 무게가 그의 운명을 결정짓게 될 것이다.
이제, 당신은 천천히, 조심스럽게 탑의 문을 밀고 들어선다.
심장은 조용히 두근대고, 손끝은 미세하게 떨린다.
이 곳에서의 모든 선택은 오직 당신의 것이며, 모든 대가는 반드시 돌아온다.

### 🎯 게임 목표

- **목표 턴(40턴)** 동안 생존

### ⚙️ 게임 기본 자원

| 자원명 | 초기값 | 설명 |
| --- | --- | --- |
| 체력 (HP) | 10 | 매 턴 저주 수치만큼 피해. 0이 되면 즉시 게임 종료 |
| 저주 (Curse) | 0 | 5턴마다 자동 +1. 카드 효과에 따라 증감 가능 |
| 카드 덱 | 40장 | 매 턴 3장 랜덤 등장. 카드 효과로 추가/삭제 가능 |

### 🔁 턴 진행 방식

매 턴마다 다음의 순서로 진행된다.

1. **카드 뽑기**: 기본 3장의 카드를 덱에서 드로우
2. **카드 선택**: 1장을 선택
3. **효과 실행**: 일반 효과 즉시 실행, 특수 효과 존재 시 문구에 맞게 실행
4. **저주 피해**: 턴 종료 시 체력 -= 현재 저주
5. **저주 증가**: 매 5턴마다 `턴/5` 만큼 저주 증가
6. **죽음 카드 삽입**: 저주 ≥ 6 → 매 턴 `저주 - 5` 장의 죽음 카드가 덱에 추가됨

### 🃏 카드 효과 예시

| 카드 | 일반 효과 | 특수 효과 |
| --- | --- | --- |
| 🌞 태양 | 체력 +10 / 저주 +2 |  |
| 🌙 달 | 체력 -10 / 저주 -2 |  |
| ⭐ 별 | 체력 +2 |  |
| ☠ 죽음 |  | 사망한다. |
| 🗼 탑 | 체력 -1 | 다음 턴은 카드를 뽑지 않고 저주로 인한 체력감소가 이루어지지 않는다. |
| 👥 연인 | 체력 +1 / 저주 +1 | 리롤 기회를 1회 얻는다. |
| 🌍 세계 | 체력 +7/ 저주 -3 | 뽑은 모든 카드를 적용한다. |

  **☑️카드 이미지는 본 문서 하단 참고**

### 🖼️ 게임 UI 미리보기

<img src="asset/ex_gametitle.PNG" alt="메인화면 UI" width="200"/> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <img src="asset/ex_gameplay.PNG" alt="게임화면 UI" width="200"/>

---

### 👥 팀 정보

| **팀명** | 청룡오형제 |
| --- | --- |
| **팀원** | 배태현 <br> 김태우 <br>송민석 <br>정어진 <br>가오즈항  |

---

## 🛠️ 개발 환경

- **게임 엔진**: Unity 6000.1.2f1
- **IDE**: Visual Studio Code
- **개발 언어**: C#
- **버전 관리**: Git & GitHub
- **협업 도구**: Discord, Notion
- **AI 도구**: ChatGPT-4

---

## 🗃️ 카드 및 기타 이미지

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

| 40 | main | Start | Floor |   |
|----|---|---|---|---|
| [![](https://github.com/user-attachments/assets/74d2f6ff-9cf1-4ec1-80ad-b55c3c47c181)](https://github.com/user-attachments/assets/74d2f6ff-9cf1-4ec1-80ad-b55c3c47c181) | ![1main](https://github.com/user-attachments/assets/f62aca8b-4c46-417a-9c52-ef3088376968) | ![start1](https://github.com/user-attachments/assets/cb7809fb-4427-4c6c-91ac-43bbc9efe355) | ![Floor_1to10](https://github.com/user-attachments/assets/787aac0b-5765-4409-97f6-54714e52c1a9) |  |

---

### 개발 담당

---

## 🎮 게임 로직

**배태현**  
- 카드별 체력, 저주 수치 적용  
- 카드별 특수 효과 적용  
- 카드 드로우 기능  
- 직업 기능  
- 카드 덱 기능  

**가오즈항**  
- 카드 자동 추출 시스템 구현  
- 게임 로직 설계 및 최적화  
- 무한모드 추가  
- 카드별 스토리 추가  

---

## 🎨 UI/UX

**김태우**  
- 카드 이미지(40장) 제작  
- UI 연결(카드 이미지, 카드 효과, 카드 스토리)
- 카드 선택(직접,랜덤)시 강조 애니메이션
- 메인 메뉴 구성(텍스트, 버튼)  
- 인게임 화면 구성(HP, 저주 표시, 카드 위치, 턴 표시)  

**송민석**  
- 게임 UI 이미지 제작 및 비율 조정  
- 카드 드로우 애니메이션 추가  
- 게임 엔딩 장면 연동  
- 크레딧 화면  
- 카드 종류 탭(초기 설정)  

---

## 📝 설계 및 문서화

**정어진**  
- 게임 시스템 설계 및 기획  
- 게임 초기 CLI 버전 개발  
- 문서화  
- 발표  
- UI 개선  
- 게임 로직 버그 수정  
- 카드 효과 제작

---

## 설치 방법

### Android (APK)

1. [최신 릴리즈 페이지](https://github.com/bs06136/open_sorce_25_1_cau/releases/latest)에서 `CursedTarot.apk` 파일을 다운로드합니다.  
2. 다운로드한 APK 파일을 Android 기기로 복사합니다.  
3. 기기 설정 → 보안 → ‘알 수 없는 출처(Unknown sources)’ 설치를 허용합니다.  
4. APK 파일을 실행하여 설치를 완료합니다.  
5. 설치가 완료되면 홈 화면 또는 앱 서랍에서 앱 아이콘을 눌러 실행하세요.  

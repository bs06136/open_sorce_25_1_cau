# 💠 Contributing Guide: 저주받은 타로 (Cursed Tarot)

코드 스타일, 협업 프로세스, 브랜치 전략, PR 가이드 등을 반드시 준수해 주세요.

---

## 1. 개발 환경

![PawsFurryGIF.gif](attachment:a5835e1f-b7f7-483a-8b19-1f40ae29b1f5:PawsFurryGIF.gif)

### 1.1 필수 도구

- Unity Hub
- Git
- VS Code (권장)

### 1.2 저장소 복제

```
bash
복사편집
git clone https://github.com/bs06136/open_sorce_25_1_cau.git
cd open_sorce_25_1_cau

```

### 1.3 Unity 설치

- [Unity 다운로드 페이지](https://unity.com/kr/download)에서 에디터 설치
- Android 빌드 지원 모듈 설치:
    - Android Build Support
    - Android SDK & NDK Tools
    - OpenJDK

### 1.4 프로젝트 연결

- Unity Hub → **Add** → 클론한 폴더 선택 → 프로젝트 로드

---

## 2. 협업 규칙 및 워크플로우

### 🔐 메인 브랜치 보호

- `main` 브랜치는 항상 **빌드 가능한 상태** 유지
- 직접 푸시 금지
- PR 생성 → **Collaborator에 의한 PR 승인 및 CI 통과 후 머지**

### 🌿 브랜치 네이밍

```
<type>/<ISSUE번호>-<간단설명>
```

예시:

- `feature/23-add-card-effects`
- `bugfix/45-fix-draw-bu`
- `hotfix/67-critical-patch`

### 💬 커밋 메시지

```
<type>: <설명>
```

타입 예시:

| **유형** | **패턴** |
| --- | --- |
| 버그 수정 | `bugfix` |
| 문서 작업 | `doc` |
| 카드·로직 추가 | `expand` |
| 게임 시스템 변경 | `refactor` |

커밋 예시:

```
bugfix   : 체력이 줄어들지 않는 문제 해결
doc      : 문서 내용 수정
expand   : 새로운 카드 확장팩( 추가
refactor : 카드 4장 선택되게 변경
```

### 🔄 동기화 & 푸시

```bash
git fetch origin
git rebase origin/main
git push origin <branch>
```

---

## 3. Pull Request 절차

1. GitHub에서 PR 생성 (`main` 브랜치 대상으로)
2. **제목**: `[ISSUE번호] <type>: <간단설명>`
3. **본문** 포함 항목:
    - **개요**: 작업 목적
    - **변경 사항**:
        - `feat`: 기능 추가
        - `fix`: 버그 수정
        - `refactor`: 리팩터링
        - `docs`: 문서 수정
    - **테스트**: 검증 방법
    - **관련 이슈**: `closes #23`
4. 리뷰어 지정 및 최소 1명 승인
5. CI 성공 → 머지
6. 브랜치 삭제:

```bash
git branch -d <branch>
git push origin --delete <branch>
```

---

## 4. 코드 스타일 & 품질

### 📐 C# 코드 스타일

- 클래스/메서드: `PascalCase`
- 변수: `camelCase`
- 상수: `UPPER_SNAKE_CASE`
- 들여쓰기: 스페이스 4칸
- 최대 라인 길이: 120자

### 🧹 Lint & 포맷

- `.editorconfig` 및 StyleCop 적용
- `dotnet format` 사용 권장

### ✅ 테스트

- NUnit 기반 단위 테스트
- 커버리지 80% 목표
- `Tests/` 폴더 내 구성

### 🔁 CI 순서

빌드 → 린트 → 테스트

실패 시 머지 금지

---

## 5. 커뮤니케이션 & 지원

- **Slack**: `#cursed-tarot` 채널
- **이슈 관리**: GitHub Issues
- **문서 변경**: 반드시 Wiki에 반영

---

## 📌 참고

- 문서 갱신 시 본 가이드도 함께 업데이트할 것
- 기여 전에는 꼭 이 문서를 숙지해 주세요

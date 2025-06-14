# 💠 Contributing Guide: 저주받은 타로 (Cursed Tarot)

코드 스타일, 협업 프로세스, 브랜치 전략, PR 가이드 등을 반드시 준수해 주세요.

---

## 1. 개발 환경


### 🔧 필수 도구

- Unity Hub
- Git
- VS Code (권장)

### 💾 저장소 복제

```
bash
복사편집
git clone https://github.com/bs06136/open_sorce_25_1_cau.git
cd open_sorce_25_1_cau

```

### 🕹 Unity 설치

- [Unity 다운로드 페이지](https://unity.com/kr/download)에서 에디터 설치
- Android 빌드 지원 모듈 설치:
    - Android Build Support
    - Android SDK & NDK Tools
    - OpenJDK

### 🔗 프로젝트 연결

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
expand   : 새로운 카드 확장팩(빛나는 유물) 추가
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
        - `fix`: 버그 수정
        - `docs`: 문서 수정
        - `expand`: 신규 카드, 로직 추가
        - `refactor`: 기존 시스템, 로직 변경
        - 
    - **테스트**: 검증 방법
    - **관련 이슈**: `closes #23`
4. PR 요청 → collaborator 확인 후 승인
5. CI 성공 → 머지
6. 브랜치 삭제:

```bash
git branch -d <branch>
git push origin --delete <branch>
```

---


## 4. 커뮤니케이션 & 지원

- **이슈 관리**: GitHub Issues
- **문서 변경**: 반드시 Wiki에 반영

---

## 📌 참고

- 문서 갱신 시 본 가이드도 함께 업데이트할 것
- 기여 전에는 꼭 이 문서를 숙지해 주세요

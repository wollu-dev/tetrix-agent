# Contributing Guide

## 1. 작업 흐름

```
메인 레포 Fork → 로컬 Clone → 브랜치 생성 → 작업 → PR
```

1. 메인 레포를 **Fork**한다.
2. Fork한 레포를 로컬에 Clone한다.
3. 작업 단위에 맞는 브랜치를 생성한다. ([브랜치 네이밍 규칙](#4-브랜치-네이밍-규칙) 참고)
4. 작업 완료 후 Fork한 레포에 Push한다.
5. 메인 레포의 `main` 브랜치로 **Pull Request**를 생성한다.
6. 상대방의 리뷰 및 승인 후 Merge한다.

---

## 2. 브랜치 보호 규칙

`main` 브랜치에는 아래 보호 규칙이 적용되어 있다.

- **직접 Push 불가** — 반드시 PR을 통해서만 Merge 가능
- **1명 이상의 리뷰 승인 필수** — 상대방의 Approve 없이 Merge 불가
- **CI 통과 필수** — 모든 상태 체크가 통과되어야 Merge 가능
- **최신 상태 유지 필수** — main 브랜치 기준으로 최신 상태인 브랜치만 Merge 가능

---

## 3. 커밋 메시지 규칙

### 형식

```
<type>: <subject>

[body]
```

### Type 목록

| Type | 설명 |
|------|------|
| `feat` | 새로운 기능 추가 |
| `fix` | 버그 수정 |
| `docs` | 문서 수정 |
| `style` | 코드 포맷, 세미콜론 등 로직 변경 없는 수정 |
| `refactor` | 리팩토링 (기능 변경 없음) |
| `test` | 테스트 코드 추가 및 수정 |
| `chore` | 빌드, 패키지, 설정 파일 수정 |

### 규칙

- 제목은 **50자 이내**, 마침표 없이 작성
- 제목은 **명령형**으로 작성 (e.g. `Add` / `Fix` / `Update`)
- 본문이 필요한 경우 제목과 한 줄 띄고 작성

### 예시

```
feat: 로그인 기능 추가

소셜 로그인(Google, Kakao) 연동 및 JWT 토큰 발급 처리
```

```
fix: 메인 페이지 레이아웃 깨짐 수정
```

---

## 4. 브랜치 네이밍 규칙

### 형식

```
<type>/<간단한-설명>
```

### Type 목록

| Type | 설명 |
|------|------|
| `feature` | 새로운 기능 개발 |
| `fix` | 버그 수정 |
| `hotfix` | 긴급 수정 |
| `docs` | 문서 작업 |
| `chore` | 설정, 환경 관련 작업 |
| `refactor` | 리팩토링 |

### 규칙

- 소문자와 하이픈(`-`)만 사용
- 간결하고 작업 내용을 명확히 표현

### 예시

```
feature/login
feature/social-login
fix/main-layout
hotfix/auth-token-expired
docs/update-readme
chore/eslint-setup
```

---

## 5. Pull Request 규칙

- PR 제목은 커밋 메시지 규칙과 동일하게 작성
- PR 본문은 템플릿에 따라 작성 ([PR 템플릿](.github/pull_request_template.md) 참고)
- 리뷰어를 반드시 지정할 것
- Merge 방식은 **Squash and Merge** 사용 (커밋 히스토리 정리)
- Merge 후 작업 브랜치는 삭제

---

## 6. 이슈 관리

- 작업 시작 전 이슈를 먼저 생성하는 것을 권장
- 이슈 제목은 브랜치 네이밍과 동일한 형식으로 작성
- PR 본문에 `Closes #이슈번호`를 명시하여 자동 Close 처리

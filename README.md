# Space fireduck

Unity로 제작한 **2D 교육형 프로젝트 기반 게임**입니다.  
기본 학습 프로젝트에 우주를 나는 닭 콘셉트와 아이템, 슬로우 모드 등의 요소를 추가했습니다.

## Project Info

- Engine: Unity 6 `6000.5.0f1`
- Language: C#
- Type: Educational Project / 2D Game
- Repository: Space-fireduck

## Controls

- **좌클릭 홀드**: 마우스 방향으로 이동
- **우클릭 홀드**: 슬로우 모드 사용
  - 슬로우 게이지를 소모합니다.
  - 사용하지 않을 때 게이지가 회복됩니다.

## Main Features

### 불닭 소스
불닭 소스를 획득하면 일정 시간 동안 강화 상태가 됩니다.

- **3초간 무적**
- 이동 추진력 증가
- 강화 상태 전용 캐릭터 스프라이트 적용

### 슬로우 모드
우클릭을 누르고 있는 동안 게임 시간이 느려집니다.

- 기본 Time Scale: `1.0`
- Slow Mode Time Scale: `0.3`
- 게이지 소모 및 자동 회복

### 장애물 / 운석
장애물과 운석을 피하며 점수를 획득합니다.

- 일반 장애물
- Cold Meteor
- 충돌 시 게임 오버
- 무적 상태에서는 일부 위험 요소를 파괴할 수 있습니다.

### Score / High Score
플레이 중 점수를 기록하고, 최고 점수는 `PlayerPrefs`를 이용해 저장합니다.

## Project Structure

주요 스크립트는 `Assets/Scipts` 폴더에서 확인할 수 있습니다.

- `PlayerController.cs` — 이동, 슬로우 모드, 강화 상태, 점수 및 게임 오버
- `ObstacleSpawner.cs` — 장애물 생성
- `ColdMeteorSpawner.cs` — 운석 생성
- `FireSauceSpawner.cs` — 불닭 소스 생성
- `SideFoodSpawner.cs` — 사이드 음식 생성

## Purpose

Unity 2D의 물리, 입력 처리, UI, 오브젝트 생성, 아이템 효과와 게임 진행 구조를 학습하면서  
기본 프로젝트를 개인 콘셉트로 변형해본 작업입니다.

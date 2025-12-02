# viewXR-Unity: Project\_VR

## 프로젝트 개요 (Overview)

이 프로젝트는 **Unity** 엔진과 **OpenXR 표준**을 기반으로 개발된 몰입형 **가상현실(VR) 환경**입니다. `viewXR` 프레임워크(또는 솔루션)를 활용하여 사용자에게 \*\*[주제 예: 3D 모델, 데이터 시각화, 건축 탐색 등]\*\*을 직관적이고 현실적으로 탐색하고 상호작용할 수 있는 경험을 제공하는 **VR 기반 시각화 및 상호작용 솔루션**입니다.

## 주요 기능 (Features)

### 🔹 OpenXR 호환성

**Unity OpenXR 플러그인**을 사용하여 다음과 같은 다양한 XR 장치와 높은 호환성을 제공합니다:

  * **Meta Quest** 시리즈
  * **HTC VIVE**
  * 기타 **OpenXR 지원 장치**

### 🔹 몰입형 3D 환경

  * **[구체적 환경 설명 예: 고해상도 실내/실외 맵]**
  * 이동, 탐색, 회전 등 자연스러운 **VR 내비게이션** 지원

### 🔹 직관적인 상호작용

**Unity XR Interaction Toolkit**을 기반으로 다음과 같은 상호작용을 지원합니다:

  * 객체 **선택 / 집기 / 회전**
  * **VR UI 조작**
  * 컨트롤러 및 **핸드 트래킹** 지원

### 🔹 [핵심 기능 1]

  * 예: 3D 모델의 부품 **분해 및 조립 시뮬레이션**

### 🔹 [핵심 기능 2]

  * 예: **실시간 사용자 위치 공유** 및 **협업 기능**

## 필수 요구 사항 (Prerequisites)

### 소프트웨어

| 항목 | 상세 내용 | 비고 |
| :--- | :--- | :--- |
| **Unity Editor** | Unity **2021.3 LTS 이상** |  |
| **IDE** | Visual Studio 또는 기타 C\# IDE | 선택 사항 |

### 하드웨어

  * **VR 헤드셋**: **OpenXR 런타임 지원 장치**
      * Meta Quest 2 / Meta Quest 3
      * HTC VIVE / VIVE Pro
  * **운영체제**
      * **Windows 10/11** (PCVR)
      * **Android** (Standalone Quest 빌드)

## 설치 및 실행 (Installation)

### 1\. 저장소 클론

```bash
git clone https://github.com/bluerrinng/viewXR-Unity.git
```

### 2\. Unity 프로젝트 열기

1.  **Unity Hub**를 실행합니다.
2.  `열기(Open)` 버튼을 클릭합니다.
3.  클론한 프로젝트 폴더 내의 **`Project_VR`** 폴더를 선택합니다.
4.  Unity가 필요한 패키지를 로딩하고 설치할 때까지 대기합니다.

### 3\. VR 환경 설정 확인

1.  상단 메뉴에서 `Edit` → `Project Settings`를 엽니다.
2.  좌측에서 \*\*`XR Plug-in Management`\*\*를 선택합니다.
3.  `Plug-in Providers`에서 \*\*`OpenXR`\*\*을 활성화합니다.
4.  `OpenXR` 설정을 확인하고 다음 Feature Groups가 추가되었는지 확인합니다:
      * Meta Quest Support
      * VIVE Cosmos Support
      * 기타 필요한 Feature Groups

### 4\. 프로젝트 실행

1.  `Assets/Scenes` 폴더에서 메인 VR 씬(예: `MainScene.unity`)을 엽니다.
2.  VR 헤드셋을 PC에 연결하고, 관련 런타임(SteamVR, Oculus PC App 등)을 실행합니다.
3.  Unity Editor에서 **Play 버튼**을 클릭하면 VR 환경에서 프로젝트가 실행됩니다.

-----

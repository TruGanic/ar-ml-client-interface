# OrganicTraceAR

Unity-based AR mobile application for QR-driven organic produce traceability.

## Overview
OrganicTraceAR is a Unity 6 mobile project that combines Vuforia QRbarcode scanning with a lightweight AR overlay to present produce traceability insights in a user-friendly format. After a QR code is scanned, the app extracts the batch ID, sends it to the Insight Engine API, and displays a persistent world-space overlay with batch-level quality and trust information.

This repository contains the Unity project scaffold, auth flow, AR overlay logic, scene generator, and supporting scripts required to run the application.

## Tech Stack
- Unity 6000.3.2f1
- UI System uGUI + TextMeshPro
- Render Pipeline URP
- Target Platform Android (portrait)
- AR  Scanning Vuforia Engine
- Networking UnityWebRequest
- Backend Integration Insight Engine REST API

## Core Features
- Splash, Auth, and AR scenes
- Login, Signup, and Forgot Password screens
- Direct Login  Signup navigation to `ARScene`
- QR  barcode scanning through Vuforia
- Insight Engine API call using scanned batch ID
- Persistent world-space AR summary overlay
- Mock auth API service for rapid UI testing
- Editor tool to generate all scenes from code

## Repository Structure
```text
OrganicTraceAR_UnityProject
├── Assets
│   └── OrganicTraceAR
│       ├── Docs
│       ├── Editor
│       ├── Scenes
│       └── Scripts
├── Packages
├── ProjectSettings
└── README_GIT_REPO.md
```

### Key folders
- `AssetsOrganicTraceAREditor` - scene generator tools
- `AssetsOrganicTraceARScriptsAR` - scanner, overlay, and AR data presentation
- `AssetsOrganicTraceARScriptsUIAuth` - login, signup, and forgot-password controllers
- `AssetsOrganicTraceARScriptsManagers` - navigation, panel switching, session handling
- `AssetsOrganicTraceARDocs` - quick setup documentation

## Scene Flow
1. `SplashScene`
2. `AuthScene`
3. `ARScene`

### Auth flow
- Login success - `ARScene`
- Signup success - `ARScene`
- Forgot Password - mock success  status message

### AR flow
1. User scans a QR code
2. Batch ID is extracted from the QR content
3. App calls the Insight Engine API
4. Response is parsed into app models
5. AR overlay is populated and displayed in world space
6. Overlay remains visible until a new QR is scanned

## Getting Started
### 1. Open the project
Open this folder in Unity 6000.3.2f1.

### 2. Let Unity resolve packages
Wait until Unity finishes importing packages and scripts.

### 3. Generate scenes
Run

`Tools  OrganicTraceAR  Generate All Scenes`

This generates
- `SplashScene`
- `AuthScene`
- `ARScene`

### 4. Open a scene
Open
- `AssetsOrganicTraceARScenesAuthScene.unity`
- or `AssetsOrganicTraceARScenesARScene.unity`

## Vuforia Setup
Vuforia is not bundled in this repo because installation and licensing depend on the local Unity environment.

### After opening the project
1. Install Vuforia Engine through Unity Package Manager or your preferred package workflow
2. Enter the Vuforia license key in
   - `Project Settings  Vuforia Engine`
3. Add  confirm the required Vuforia barcode component on the `Barcode` GameObject in `ARScene`
4. Ensure the AR camera has
   - `VuforiaBehaviour`
   - tag set to `MainCamera`

## API Configuration
In `ARScene`, select the `Barcode` object and configure the scanner fields in the Inspector

- Insight Engine Base Url
- Endpoint Prefix

Example
- Base URL `httpYOUR_SERVER_IP8000`
- Endpoint Prefix `apiretailerhistory`

The final request format is

```text
{BaseUrl}apiretailerhistory{batchId}
```

## Important Scripts
### AR
- `BarcodeInsightScanner.cs`
  - reads scanned barcode data
  - calls the Insight Engine API
  - passes results to the overlay presenter
- `ARInsightOverlayPresenter.cs`
  - binds API response fields to the AR UI
- `PersistentAROverlayAnchor.cs`
  - places the overlay in world space and keeps it visible after the QR leaves view
- `BatchInsightModels.cs`
  - response models for the Insight Engine payload

### Auth
- `LoginFormController.cs`
- `SignupFormController.cs`
- `ForgotPasswordFormController.cs`
- `AuthViewController.cs`
- `MockApiService.cs`

### Scene  Navigation
- `SceneNavigator.cs`
- `PanelManager.cs`
- `SplashController.cs`
- `OrganicTraceARSceneGenerator.cs`

## Editor Testing
If Vuforia is not fully configured yet, the project includes a debug-friendly workflow for scanner testing.

Use the `BarcodeInsightScanner` debug options if present in the Inspector or component context menu to simulate a scan with a known batch ID.

## Current UI  AR Design
### Auth Scene
- Clean portrait mobile layout
- Login, Signup, Forgot Password panels
- Panel-based navigation using `PanelManager`

### AR Scene
- World-space summary card
- Screen-space statusdebug text
- Persistent overlay root separated from the tracked barcode object
- Overlay updates when a new batch is scanned

## Known Notes
- Vuforia initialization in Unity Editor can be sensitive to webcam availability and local setup
- If Vuforia fails to initialize after scene or hierarchy changes, try
  - reopening Unity
  - deleting the `Library` folder and reopening the project
  - reimporting the Vuforia package
- `MainScene` is intentionally skipped in the current fast-path flow; Login and Signup go directly to `ARScene`

## Recommended Git Ignore
For Unity repos, make sure `.gitignore` excludes generated folders such as
- `Library`
- `Temp`
- `Logs`
- `obj`
- `Build`
- `.vs`
- `UserSettings`

Use Unity's standard `.gitignore` template for best results.

## Quick Start Summary
1. Open project in Unity 6000.3.2f1
2. Generate scenes from `Tools  OrganicTraceAR  Generate All Scenes`
3. Install and configure Vuforia
4. Set the Insight Engine base URL on the `Barcode` object
5. Open `AuthScene`
6. Login or Signup
7. Continue directly to `ARScene`
8. Scan QR and verify the overlay response

## License  Project Use
This repository is intended as the project codebase for the OrganicTraceAR research and prototype implementation.
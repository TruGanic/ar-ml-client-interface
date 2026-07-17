using System.Collections.Generic;
using System.IO;
using OrganicTraceAR.AR;
using OrganicTraceAR.Core;
using OrganicTraceAR.Managers;
using OrganicTraceAR.Mock;
using OrganicTraceAR.UI.Auth;
using OrganicTraceAR.UI.Common;
using TMPro;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OrganicTraceAR.Editor
{
    public static class OrganicTraceARSceneGenerator
    {
        private static readonly Color BackgroundColor = Hex("F7F8FA");
        private static readonly Color CardColor = Hex("FFFFFF");
        private static readonly Color Primary = Hex("2E7D32");
        private static readonly Color Secondary = Hex("66BB6A");
        private static readonly Color Muted = Hex("6B7280");
        private static readonly Color Dark = Hex("1F2937");
        private static readonly Color LightGray = Hex("E5E7EB");

        [MenuItem("Tools/OrganicTraceAR/Generate All Scenes")]
        public static void GenerateAllScenes()
        {
            TMP_PackageResourceImporter importer = new TMP_PackageResourceImporter();
            importer.ImportProjectResources();

            EnsureFolders();
            GenerateSplashScene();
            GenerateAuthScene();
            GenerateARScene();
            UpdateBuildSettings();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("OrganicTraceAR", "Scenes generated successfully.", "OK");
        }

        private static void EnsureFolders()
        {
            if (!AssetDatabase.IsValidFolder("Assets/OrganicTraceAR")) AssetDatabase.CreateFolder("Assets", "OrganicTraceAR");
            if (!AssetDatabase.IsValidFolder("Assets/OrganicTraceAR/Scenes")) AssetDatabase.CreateFolder("Assets/OrganicTraceAR", "Scenes");
        }

        private static void UpdateBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene($"Assets/OrganicTraceAR/Scenes/{AppScenes.SplashScene}.unity", true),
                new EditorBuildSettingsScene($"Assets/OrganicTraceAR/Scenes/{AppScenes.AuthScene}.unity", true),
                new EditorBuildSettingsScene($"Assets/OrganicTraceAR/Scenes/{AppScenes.ARScene}.unity", true),
            };
        }

        private static void GenerateSplashScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = AppScenes.SplashScene;

            var light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;

            var sceneRoot = new GameObject("SceneRoot");
            var canvas = CreateCanvas("Canvas", null, RenderMode.ScreenSpaceOverlay);
            canvas.transform.SetParent(sceneRoot.transform, false);
            var safeArea = CreateUIObject("SafeArea", canvas.transform);
            StretchFull(safeArea.RectTransform);
            safeArea.Image.color = BackgroundColor;

            var logo = CreateText("TitleText", safeArea.Transform, "OrganicTraceAR", 72, FontStyles.Bold, TextAlignmentOptions.Center, Dark);
            SetRect(logo.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(900, 120));
            var subtitle = CreateText("SubtitleText", safeArea.Transform, "Organic produce traceability in AR", 30, FontStyles.Normal, TextAlignmentOptions.Center, Muted);
            SetRect(subtitle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(900, 80));

            var host = new GameObject("SplashControllerHost");
            host.transform.SetParent(sceneRoot.transform, false);
            var splash = host.AddComponent<SplashController>();
            SetBoolSerialized(splash, "goToARIfLoggedIn", true);
            SetFloatSerialized(splash, "splashDurationSeconds", 2f);

            EnsureEventSystem();
            EditorSceneManager.SaveScene(scene, $"Assets/OrganicTraceAR/Scenes/{AppScenes.SplashScene}.unity");
        }

        private static void GenerateAuthScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = AppScenes.AuthScene;

            var light = new GameObject("Directional Light");
            light.AddComponent<Light>().type = LightType.Directional;

            var sceneRoot = new GameObject("SceneRoot");
            var services = new GameObject("Services");
            services.transform.SetParent(sceneRoot.transform, false);

            var mockApiGo = new GameObject("MockApiService");
            mockApiGo.transform.SetParent(services.transform, false);
            var mockApi = mockApiGo.AddComponent<MockApiService>();

            var navGo = new GameObject("SceneNavigator");
            navGo.transform.SetParent(services.transform, false);
            var nav = navGo.AddComponent<SceneNavigator>();

            var panelManagerGo = new GameObject("PanelManager");
            panelManagerGo.transform.SetParent(services.transform, false);
            var panelManager = panelManagerGo.AddComponent<PanelManager>();

            var authViewGo = new GameObject("AuthViewController");
            authViewGo.transform.SetParent(services.transform, false);
            var authView = authViewGo.AddComponent<AuthViewController>();

            var canvas = CreateCanvas("Canvas", sceneRoot.transform, RenderMode.ScreenSpaceOverlay);
            var safeArea = CreateUIObject("SafeArea", canvas.transform);
            StretchFull(safeArea.RectTransform);
            safeArea.Image.color = BackgroundColor;
            var safeLayout = safeArea.GameObject.AddComponent<VerticalLayoutGroup>();
            safeLayout.padding = new RectOffset(48, 48, 72, 40);
            safeLayout.spacing = 28;
            safeLayout.childAlignment = TextAnchor.UpperCenter;
            safeLayout.childControlWidth = true;
            safeLayout.childControlHeight = false;
            safeLayout.childForceExpandWidth = true;
            safeLayout.childForceExpandHeight = false;

            var header = CreateUIObject("AuthHeader", safeArea.Transform);
            var headerLE = header.GameObject.AddComponent<LayoutElement>();
            headerLE.preferredHeight = 180;
            var headerLayout = header.GameObject.AddComponent<VerticalLayoutGroup>();
            headerLayout.spacing = 10;
            headerLayout.childAlignment = TextAnchor.MiddleCenter;
            headerLayout.childControlWidth = true;
            headerLayout.childControlHeight = false;
            headerLayout.childForceExpandWidth = true;
            headerLayout.childForceExpandHeight = false;
            CreateText("TitleText", header.Transform, "OrganicTraceAR", 64, FontStyles.Bold, TextAlignmentOptions.Center, Dark);
            CreateText("SubtitleText", header.Transform, "Track organic produce with confidence", 30, FontStyles.Normal, TextAlignmentOptions.Center, Muted);

            var loginPanel = CreateAuthPanel("LoginPanel", safeArea.Transform, 720);
            var signupPanel = CreateAuthPanel("SignupPanel", safeArea.Transform, 780);
            var forgotPanel = CreateAuthPanel("ForgotPasswordPanel", safeArea.Transform, 520);
            signupPanel.GameObject.SetActive(false);
            forgotPanel.GameObject.SetActive(false);

            var loginEmail = CreateTMPInputField("EmailInput", loginPanel.Transform, "Enter your email", false);
            var loginPassword = CreateTMPInputField("PasswordInput", loginPanel.Transform, "Enter your password", true);
            var loginStatus = CreateStatusMessage("StatusText", loginPanel.Transform);
            var loginButton = CreateButton("LoginButton", loginPanel.Transform, "Login", Primary, Color.white);
            var signupNavButton = CreateButton("GoToSignupButton", loginPanel.Transform, "Create an Account", Secondary, Color.white);
            var forgotButton = CreateButton("ForgotPasswordButton", loginPanel.Transform, "Forgot Password", LightGray, Dark);
            var loginController = loginPanel.GameObject.AddComponent<LoginFormController>();
            SetObjectRefSerialized(loginController, "emailInput", loginEmail.inputField);
            SetObjectRefSerialized(loginController, "passwordInput", loginPassword.inputField);
            SetObjectRefSerialized(loginController, "statusMessageView", loginStatus.statusView);
            SetObjectRefSerialized(loginController, "mockApiService", mockApi);
            SetObjectRefSerialized(loginController, "sceneNavigator", nav);
            AddButtonCall(loginButton.button, loginController, nameof(LoginFormController.Submit));
            AddButtonCall(signupNavButton.button, authView, nameof(AuthViewController.ShowSignup));
            AddButtonCall(forgotButton.button, authView, nameof(AuthViewController.ShowForgotPassword));

            var signupName = CreateTMPInputField("NameInput", signupPanel.Transform, "Enter your name", false);
            var signupEmail = CreateTMPInputField("EmailInput", signupPanel.Transform, "Enter your email", false);
            var signupPassword = CreateTMPInputField("PasswordInput", signupPanel.Transform, "Create a password", true);
            var signupStatus = CreateStatusMessage("StatusText", signupPanel.Transform);
            var signupButton = CreateButton("SignupButton", signupPanel.Transform, "Sign Up", Primary, Color.white);
            var backToLoginFromSignup = CreateButton("BackToLoginButton", signupPanel.Transform, "Back to Login", LightGray, Dark);
            var signupController = signupPanel.GameObject.AddComponent<SignupFormController>();
            SetObjectRefSerialized(signupController, "nameInput", signupName.inputField);
            SetObjectRefSerialized(signupController, "emailInput", signupEmail.inputField);
            SetObjectRefSerialized(signupController, "passwordInput", signupPassword.inputField);
            SetObjectRefSerialized(signupController, "statusMessageView", signupStatus.statusView);
            SetObjectRefSerialized(signupController, "mockApiService", mockApi);
            SetObjectRefSerialized(signupController, "sceneNavigator", nav);
            AddButtonCall(signupButton.button, signupController, nameof(SignupFormController.Submit));
            AddButtonCall(backToLoginFromSignup.button, authView, nameof(AuthViewController.ShowLogin));

            var forgotEmail = CreateTMPInputField("EmailInput", forgotPanel.Transform, "Enter your email", false);
            var forgotStatus = CreateStatusMessage("StatusText", forgotPanel.Transform);
            var sendResetButton = CreateButton("SendResetLinkButton", forgotPanel.Transform, "Send Reset Link", Primary, Color.white);
            var backToLoginFromForgot = CreateButton("BackToLoginButton", forgotPanel.Transform, "Back to Login", LightGray, Dark);
            var forgotController = forgotPanel.GameObject.AddComponent<ForgotPasswordFormController>();
            SetObjectRefSerialized(forgotController, "emailInput", forgotEmail.inputField);
            SetObjectRefSerialized(forgotController, "statusMessageView", forgotStatus.statusView);
            SetObjectRefSerialized(forgotController, "mockApiService", mockApi);
            AddButtonCall(sendResetButton.button, forgotController, nameof(ForgotPasswordFormController.Submit));
            AddButtonCall(backToLoginFromForgot.button, authView, nameof(AuthViewController.ShowLogin));

            SetPanelManagerPanels(panelManager, new Dictionary<string, GameObject>
            {
                { AppPanels.Login, loginPanel.GameObject },
                { AppPanels.Signup, signupPanel.GameObject },
                { AppPanels.ForgotPassword, forgotPanel.GameObject },
            }, AppPanels.Login);
            SetObjectRefSerialized(authView, "panelManager", panelManager);

            EnsureEventSystem();
            EditorSceneManager.SaveScene(scene, $"Assets/OrganicTraceAR/Scenes/{AppScenes.AuthScene}.unity");
        }

        private static void GenerateARScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = AppScenes.ARScene;

            var lightGo = new GameObject("Directional Light");
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;

            var arCamera = new GameObject("ARCamera");
            arCamera.tag = "MainCamera";
            var camera = arCamera.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.black;
            arCamera.AddComponent<AudioListener>();

            var barcode = new GameObject("Barcode");
            var scanner = barcode.AddComponent<BarcodeInsightScanner>();
            var trackedContentRoot = new GameObject("TrackedContentRoot");
            trackedContentRoot.transform.SetParent(barcode.transform, false);

            var overlayRoot = new GameObject("PersistentOverlayRoot");
            var overlayAnchor = overlayRoot.AddComponent<PersistentAROverlayAnchor>();
            var presenter = overlayRoot.AddComponent<ARInsightOverlayPresenter>();

            var worldCanvas = CreateCanvas("WorldOverlayCanvas", overlayRoot.transform, RenderMode.WorldSpace);
            var wcRt = worldCanvas.GetComponent<RectTransform>();
            wcRt.sizeDelta = new Vector2(800f, 500f);
            worldCanvas.transform.localScale = new Vector3(0.0015f, 0.0015f, 0.0015f);
            worldCanvas.transform.localPosition = new Vector3(0f, 0.12f, 0f);

            var summaryCard = CreateUIObject("SummaryCard", worldCanvas.transform);
            summaryCard.Image.color = new Color32(20, 24, 32, 220);
            summaryCard.GameObject.AddComponent<CanvasGroup>();
            SetRect(summaryCard.RectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 380));
            summaryCard.GameObject.SetActive(false);

            var content = CreateUIObject("ContentRoot", summaryCard.Transform);
            StretchFullWithPadding(content.RectTransform, 24, 24, 24, 24);
            var contentLayout = content.GameObject.AddComponent<VerticalLayoutGroup>();
            contentLayout.spacing = 16;
            contentLayout.childAlignment = TextAnchor.UpperLeft;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = false;
            contentLayout.childForceExpandWidth = true;
            contentLayout.childForceExpandHeight = false;

            var header = CreateUIObject("HeaderRow", content.Transform);
            header.GameObject.AddComponent<LayoutElement>().preferredHeight = 80;
            var headerLayout = header.GameObject.AddComponent<HorizontalLayoutGroup>();
            headerLayout.spacing = 12;
            headerLayout.childAlignment = TextAnchor.MiddleLeft;
            headerLayout.childControlWidth = false;
            headerLayout.childControlHeight = true;
            headerLayout.childForceExpandWidth = false;
            headerLayout.childForceExpandHeight = false;
            var produceNameText = CreateText("ProduceNameText", header.Transform, "Organic Cabbage", 44, FontStyles.Bold, TextAlignmentOptions.MidlineLeft, Color.white);
            produceNameText.gameObject.AddComponent<LayoutElement>().flexibleWidth = 1;
            var statusChip = CreateUIObject("StatusChip", header.Transform);
            statusChip.Image.color = new Color32(34, 197, 94, 255);
            SetSize(statusChip.RectTransform, 170, 54);
            var statusText = CreateText("StatusText", statusChip.Transform, "DELIVERED", 24, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            StretchFull(statusText.rectTransform);

            var metricsRow = CreateUIObject("MetricsRow", content.Transform);
            metricsRow.GameObject.AddComponent<LayoutElement>().preferredHeight = 150;
            var metricsLayout = metricsRow.GameObject.AddComponent<HorizontalLayoutGroup>();
            metricsLayout.spacing = 20;
            metricsLayout.childAlignment = TextAnchor.MiddleCenter;
            metricsLayout.childControlWidth = true;
            metricsLayout.childControlHeight = true;
            metricsLayout.childForceExpandWidth = true;
            metricsLayout.childForceExpandHeight = false;

            var gradeBadge = CreateUIObject("GradeBadge", metricsRow.Transform);
            gradeBadge.Image.color = new Color32(16, 185, 129, 255);
            SetSize(gradeBadge.RectTransform, 120, 120);
            var gradeText = CreateText("GradeText", gradeBadge.Transform, "A", 54, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            StretchFull(gradeText.rectTransform);

            var organicCard = CreateMetricCard(metricsRow.Transform, "OrganicScoreCard", "Organic Score", "95");
            var trustCard = CreateMetricCard(metricsRow.Transform, "TrustScoreCard", "Trust Score", "52");

            var flagsRow = CreateUIObject("FlagsRow", content.Transform);
            flagsRow.GameObject.AddComponent<LayoutElement>().preferredHeight = 60;
            var flagsLayout = flagsRow.GameObject.AddComponent<HorizontalLayoutGroup>();
            flagsLayout.spacing = 12;
            flagsLayout.childAlignment = TextAnchor.MiddleLeft;
            flagsLayout.childControlWidth = false;
            flagsLayout.childControlHeight = true;
            flagsLayout.childForceExpandWidth = false;
            flagsLayout.childForceExpandHeight = false;
            var tempFlag = CreateFlagChip(flagsRow.Transform, "FlagChipTemp", "High Temp");
            var humidityFlag = CreateFlagChip(flagsRow.Transform, "FlagChipHumidity", "Low Humidity");

            var hintText = CreateText("TapHintText", content.Transform, "Scan next QR to replace", 20, FontStyles.Normal, TextAlignmentOptions.Right, new Color32(180, 190, 200, 255));

            var screenCanvas = CreateCanvas("Canvas", null, RenderMode.ScreenSpaceOverlay);
            var qrText = CreateText("QRText", screenCanvas.transform, string.Empty, 26, FontStyles.Normal, TextAlignmentOptions.TopLeft, Color.white);
            SetRect(qrText.rectTransform, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(20,-20), new Vector2(650,40));
            var statusUiText = CreateText("Status", screenCanvas.transform, "Ready to scan", 26, FontStyles.Normal, TextAlignmentOptions.TopLeft, Color.white);
            SetRect(statusUiText.rectTransform, new Vector2(0,1), new Vector2(0,1), new Vector2(0,1), new Vector2(20,-60), new Vector2(900,60));

            SetObjectRefSerialized(overlayAnchor, "overlayRoot", overlayRoot.transform);
            SetObjectRefSerialized(presenter, "summaryCard", summaryCard.GameObject);
            SetObjectRefSerialized(presenter, "summaryCardCanvasGroup", summaryCard.GameObject.GetComponent<CanvasGroup>());
            SetObjectRefSerialized(presenter, "produceNameText", produceNameText);
            SetObjectRefSerialized(presenter, "statusText", statusText);
            SetObjectRefSerialized(presenter, "gradeText", gradeText);
            SetObjectRefSerialized(presenter, "organicScoreText", organicCard.valueText);
            SetObjectRefSerialized(presenter, "trustScoreText", trustCard.valueText);
            SetObjectRefSerialized(presenter, "tapHintText", hintText);
            SetObjectRefSerialized(presenter, "statusChipImage", statusChip.Image);
            SetObjectRefSerialized(presenter, "gradeBadgeImage", gradeBadge.Image);
            SetObjectRefSerialized(presenter, "flagChipTemp", tempFlag.root);
            SetObjectRefSerialized(presenter, "flagChipTempText", tempFlag.text);
            SetObjectRefSerialized(presenter, "flagChipHumidity", humidityFlag.root);
            SetObjectRefSerialized(presenter, "flagChipHumidityText", humidityFlag.text);

            SetObjectRefSerialized(scanner, "barcodeAsText", qrText);
            SetObjectRefSerialized(scanner, "statusText", statusUiText);
            SetObjectRefSerialized(scanner, "overlayPresenter", presenter);
            SetObjectRefSerialized(scanner, "persistentOverlayAnchor", overlayAnchor);
            SetObjectRefSerialized(scanner, "arCamera", camera);

            EnsureEventSystem();
            EditorSceneManager.SaveScene(scene, $"Assets/OrganicTraceAR/Scenes/{AppScenes.ARScene}.unity");
        }

        private static UIObject CreateAuthPanel(string name, Transform parent, float preferredHeight)
        {
            var panel = CreateUIObject(name, parent);
            panel.Image.color = CardColor;
            panel.GameObject.AddComponent<LayoutElement>().preferredHeight = preferredHeight;
            var layout = panel.GameObject.AddComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(32, 32, 32, 32);
            layout.spacing = 22;
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            return panel;
        }

        private static MetricCard CreateMetricCard(Transform parent, string name, string label, string value)
        {
            var root = CreateUIObject(name, parent);
            root.Image.color = new Color32(32, 40, 52, 255);
            SetSize(root.RectTransform, 220, 120);
            var labelText = CreateText(name + "Label", root.Transform, label, 22, FontStyles.Normal, TextAlignmentOptions.Top, Color.white);
            SetRect(labelText.rectTransform, new Vector2(0,0), new Vector2(1,1), new Vector2(0.5f,1), new Vector2(0,-16), new Vector2(-20,30));
            var valueText = CreateText(name + "Text", root.Transform, value, 48, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            StretchFullWithPadding(valueText.rectTransform, 0, 0, 18, 10);
            return new MetricCard { root = root.GameObject, valueText = valueText };
        }

        private static FlagChip CreateFlagChip(Transform parent, string name, string text)
        {
            var chip = CreateUIObject(name, parent);
            chip.Image.color = new Color32(185, 28, 28, 255);
            SetSize(chip.RectTransform, 180, 44);
            var chipText = CreateText(name + "Text", chip.Transform, text, 20, FontStyles.Bold, TextAlignmentOptions.Center, Color.white);
            StretchFull(chipText.rectTransform);
            return new FlagChip { root = chip.GameObject, text = chipText };
        }

        private static TMPInput CreateTMPInputField(string name, Transform parent, string placeholder, bool isPassword)
        {
            var root = CreateUIObject(name, parent);
            root.Image.color = Color.white;
            root.GameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            var input = root.GameObject.AddComponent<TMP_InputField>();
            root.RectTransform.sizeDelta = new Vector2(0, 100);

            var textArea = new GameObject("Text Area", typeof(RectTransform), typeof(RectMask2D));
            textArea.transform.SetParent(root.Transform, false);
            var textAreaRt = textArea.GetComponent<RectTransform>();
            StretchFullWithPadding(textAreaRt, 20, 20, 14, 14);

            var placeholderText = CreateText("Placeholder", textArea.transform, placeholder, 30, FontStyles.Normal, TextAlignmentOptions.Left, new Color32(156, 163, 175, 255));
            StretchFull(placeholderText.rectTransform);
            var inputText = CreateText("Text", textArea.transform, string.Empty, 32, FontStyles.Normal, TextAlignmentOptions.Left, Dark);
            StretchFull(inputText.rectTransform);

            input.textViewport = textAreaRt;
            input.textComponent = inputText;
            input.placeholder = placeholderText;
            input.contentType = isPassword ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            input.lineType = TMP_InputField.LineType.SingleLine;

            return new TMPInput { root = root.GameObject, inputField = input };
        }

        private static StatusMessage CreateStatusMessage(string name, Transform parent)
        {
            var text = CreateText(name, parent, string.Empty, 26, FontStyles.Normal, TextAlignmentOptions.Left, Muted);
            text.gameObject.AddComponent<LayoutElement>().preferredHeight = 60;
            var view = text.gameObject.AddComponent<StatusMessageView>();
            SetObjectRefSerialized(view, "messageText", text);
            return new StatusMessage { root = text.gameObject, text = text, statusView = view };
        }

        private static UIButton CreateButton(string name, Transform parent, string label, Color background, Color textColor)
        {
            var root = CreateUIObject(name, parent);
            root.Image.color = background;
            root.GameObject.AddComponent<LayoutElement>().preferredHeight = 100;
            var button = root.GameObject.AddComponent<Button>();
            var labelText = CreateText("Text", root.Transform, label, 34, FontStyles.Bold, TextAlignmentOptions.Center, textColor);
            StretchFull(labelText.rectTransform);
            return new UIButton { root = root.GameObject, button = button, label = labelText };
        }

        private static Canvas CreateCanvas(string name, Transform parent, RenderMode renderMode)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            if (parent != null) go.transform.SetParent(parent, false);
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = renderMode;
            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        private static UIObject CreateUIObject(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            return new UIObject { GameObject = go, Transform = go.transform, RectTransform = go.GetComponent<RectTransform>(), Image = go.GetComponent<Image>() };
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string text, float size, FontStyles style, TextAlignmentOptions alignment, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.fontStyle = style;
            tmp.alignment = alignment;
            tmp.color = color;
            tmp.enableWordWrapping = true;
            return tmp;
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<EventSystem>() != null)
                return;
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        private static void SetPanelManagerPanels(PanelManager panelManager, Dictionary<string, GameObject> entries, string defaultKey)
        {
            var so = new SerializedObject(panelManager);
            var panelsProp = so.FindProperty("panels");
            panelsProp.arraySize = entries.Count;
            int index = 0;
            foreach (var kvp in entries)
            {
                var element = panelsProp.GetArrayElementAtIndex(index++);
                element.FindPropertyRelative("Key").stringValue = kvp.Key;
                element.FindPropertyRelative("Panel").objectReferenceValue = kvp.Value;
            }
            so.FindProperty("defaultPanelKey").stringValue = defaultKey;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(panelManager);
        }

        private static void SetObjectRefSerialized(Object target, string propertyName, Object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.objectReferenceValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetBoolSerialized(Object target, string propertyName, bool value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.boolValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetFloatSerialized(Object target, string propertyName, float value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(propertyName);
            if (prop != null)
            {
                prop.floatValue = value;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(target);
            }
        }

        private static void AddButtonCall(Button button, Object target, string methodName)
        {
            var so = new SerializedObject(button);
            var calls = so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls");
            int index = calls.arraySize;
            calls.InsertArrayElementAtIndex(index);
            var call = calls.GetArrayElementAtIndex(index);
            call.FindPropertyRelative("m_Target").objectReferenceValue = target;
            call.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue = target.GetType().AssemblyQualifiedName;
            call.FindPropertyRelative("m_MethodName").stringValue = methodName;
            call.FindPropertyRelative("m_Mode").intValue = 1;
            call.FindPropertyRelative("m_CallState").intValue = 2;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(button);
        }

        private static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var color);
            return color;
        }

        private static void StretchFull(RectTransform rt)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void StretchFullWithPadding(RectTransform rt, float left, float right, float top, float bottom)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(-right, -top);
        }

        private static void SetRect(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = pivot;
            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
        }

        private static void SetSize(RectTransform rt, float width, float height)
        {
            rt.sizeDelta = new Vector2(width, height);
        }

        private struct UIObject
        {
            public GameObject GameObject;
            public Transform Transform;
            public RectTransform RectTransform;
            public Image Image;
        }

        private struct UIButton
        {
            public GameObject root;
            public Button button;
            public TextMeshProUGUI label;
        }

        private struct TMPInput
        {
            public GameObject root;
            public TMP_InputField inputField;
        }

        private struct StatusMessage
        {
            public GameObject root;
            public TextMeshProUGUI text;
            public StatusMessageView statusView;
        }

        private struct MetricCard
        {
            public GameObject root;
            public TextMeshProUGUI valueText;
        }

        private struct FlagChip
        {
            public GameObject root;
            public TextMeshProUGUI text;
        }
    }
}

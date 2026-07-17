using System;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace OrganicTraceAR.AR
{
    public class BarcodeInsightScanner : MonoBehaviour
    {
        [Header("Scanner UI")]
        [SerializeField] private TextMeshProUGUI barcodeAsText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("API")]
        [SerializeField] private string insightEngineBaseUrl = "http://YOUR_SERVER_IP:8000";
        [SerializeField] private string endpointPrefix = "/api/retailer/history/";
        [SerializeField] private float scanCooldownSeconds = 2f;

        [Header("Overlay")]
        [SerializeField] private ARInsightOverlayPresenter overlayPresenter;
        [SerializeField] private PersistentAROverlayAnchor persistentOverlayAnchor;
        [SerializeField] private Camera arCamera;

        [Header("Editor Debug")]
        [SerializeField] private string editorDebugBatchId = "BATCH-234560";

        private string lastProcessedBarcode = string.Empty;
        private bool isRequestInProgress;
        private float lastScanTime = -999f;

        public BatchInsightResponse LatestInsightResponse { get; private set; }

        private void Start()
        {
            if (statusText != null)
                statusText.text = "Ready to scan";

            if (arCamera == null)
                arCamera = Camera.main;
        }

        private void Update()
        {
            if (!TryGetBarcodeText(out var scannedText))
            {
                if (barcodeAsText != null)
                    barcodeAsText.text = string.Empty;
                return;
            }

            if (barcodeAsText != null)
                barcodeAsText.text = scannedText;

            if (string.IsNullOrWhiteSpace(scannedText))
                return;

            TryStartScan(scannedText.Trim());
        }

        [ContextMenu("Simulate Debug Scan")]
        public void SimulateDebugScan()
        {
            if (string.IsNullOrWhiteSpace(editorDebugBatchId))
            {
                if (statusText != null)
                    statusText.text = "Editor Debug Batch Id is empty";
                return;
            }

            TryStartScan(editorDebugBatchId.Trim(), true);
        }

        public void ResetLastScan()
        {
            lastProcessedBarcode = string.Empty;
            LatestInsightResponse = null;
            if (statusText != null)
                statusText.text = "Ready to scan again";
        }

        private void TryStartScan(string scannedText, bool force = false)
        {
            if (!force)
            {
                if (isRequestInProgress)
                    return;
                if (scannedText == lastProcessedBarcode)
                    return;
                if (Time.time - lastScanTime < scanCooldownSeconds)
                    return;
            }

            lastProcessedBarcode = scannedText;
            lastScanTime = Time.time;
            StartCoroutine(GetBatchInsights(scannedText));
        }

        private IEnumerator GetBatchInsights(string batchId)
        {
            isRequestInProgress = true;
            if (statusText != null)
                statusText.text = $"Fetching insights for {batchId}...";

            string url = BuildUrl(batchId);
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                request.SetRequestHeader("Accept", "application/json");
                yield return request.SendWebRequest();
                isRequestInProgress = false;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Insight API request failed: {request.error}\nURL: {url}");
                    if (statusText != null)
                        statusText.text = $"API error: {request.error}";
                    yield break;
                }

                string json = request.downloadHandler.text;
                try
                {
                    LatestInsightResponse = JsonUtility.FromJson<BatchInsightResponse>(json);
                    if (LatestInsightResponse != null)
                    {
                        if (statusText != null)
                            statusText.text = $"Loaded: {LatestInsightResponse.produceType} | Trust: {LatestInsightResponse.summary.overallTrustScore}";

                        overlayPresenter?.ShowInsights(LatestInsightResponse);
                        persistentOverlayAnchor?.PlaceOverlayAt(transform, arCamera != null ? arCamera : Camera.main);
                    }
                    else if (statusText != null)
                    {
                        statusText.text = "No data returned";
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"JSON parse error: {ex.Message}\nRaw JSON:\n{json}");
                    if (statusText != null)
                        statusText.text = "Failed to parse API response";
                }
            }
        }

        private string BuildUrl(string batchId)
        {
            string baseUrl = insightEngineBaseUrl.TrimEnd('/');
            string prefix = endpointPrefix.StartsWith("/") ? endpointPrefix : "/" + endpointPrefix;
            return $"{baseUrl}{prefix}{UnityWebRequest.EscapeURL(batchId)}";
        }

        private bool TryGetBarcodeText(out string scannedText)
        {
            scannedText = string.Empty;
            Component barcodeBehaviour = FindComponentByTypeName("BarcodeBehaviour");
            if (barcodeBehaviour == null)
                return false;

            PropertyInfo instanceDataProperty = barcodeBehaviour.GetType().GetProperty("InstanceData", BindingFlags.Public | BindingFlags.Instance);
            if (instanceDataProperty == null)
                return false;

            object instanceData = instanceDataProperty.GetValue(barcodeBehaviour);
            if (instanceData == null)
                return false;

            PropertyInfo textProperty = instanceData.GetType().GetProperty("Text", BindingFlags.Public | BindingFlags.Instance);
            if (textProperty == null)
                return false;

            scannedText = textProperty.GetValue(instanceData) as string;
            return !string.IsNullOrWhiteSpace(scannedText);
        }

        private Component FindComponentByTypeName(string typeName)
        {
            var components = GetComponents<Component>();
            foreach (var component in components)
            {
                if (component != null && component.GetType().Name == typeName)
                    return component;
            }
            return null;
        }
    }
}

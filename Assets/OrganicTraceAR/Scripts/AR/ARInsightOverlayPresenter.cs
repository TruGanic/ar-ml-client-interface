using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrganicTraceAR.AR
{
    public class ARInsightOverlayPresenter : MonoBehaviour
    {
        [Header("Card Root")]
        [SerializeField] private GameObject summaryCard;
        [SerializeField] private CanvasGroup summaryCardCanvasGroup;

        [Header("Main Fields")]
        [SerializeField] private TextMeshProUGUI produceNameText;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI organicScoreText;
        [SerializeField] private TextMeshProUGUI trustScoreText;
        [SerializeField] private TextMeshProUGUI tapHintText;

        [Header("Status / Grade Visuals")]
        [SerializeField] private Image statusChipImage;
        [SerializeField] private Image gradeBadgeImage;

        [Header("Flags")]
        [SerializeField] private GameObject flagChipTemp;
        [SerializeField] private TextMeshProUGUI flagChipTempText;
        [SerializeField] private GameObject flagChipHumidity;
        [SerializeField] private TextMeshProUGUI flagChipHumidityText;

        [Header("Behaviour")]
        [SerializeField] private bool hideCardOnStart = true;

        private void Start()
        {
            if (hideCardOnStart)
                HideSummaryCard();
        }

        public void ShowInsights(BatchInsightResponse data)
        {
            if (data == null)
            {
                HideSummaryCard();
                return;
            }

            SetText(produceNameText, Safe(data.produceType, "Unknown Produce"));
            SetText(statusText, Safe(data.status, "UNKNOWN"));
            SetText(gradeText, Safe(data.summary != null ? data.summary.organicGrade : null, "-"));
            SetText(organicScoreText, data.summary != null ? data.summary.organicScore.ToString() : "-");
            SetText(trustScoreText, data.summary != null ? data.summary.overallTrustScore.ToString() : "-");
            SetText(tapHintText, "Scan next QR to replace");

            ApplyStatusStyle(data.status);
            ApplyGradeStyle(data.summary != null ? data.summary.organicGrade : null);
            ApplyFlags(data.transport != null ? data.transport.flags : null);
            ShowSummaryCard();
        }

        public void HideSummaryCard()
        {
            if (summaryCard != null)
                summaryCard.SetActive(false);

            if (summaryCardCanvasGroup != null)
            {
                summaryCardCanvasGroup.alpha = 0f;
                summaryCardCanvasGroup.interactable = false;
                summaryCardCanvasGroup.blocksRaycasts = false;
            }
        }

        public void ShowSummaryCard()
        {
            if (summaryCard != null)
                summaryCard.SetActive(true);

            if (summaryCardCanvasGroup != null)
            {
                summaryCardCanvasGroup.alpha = 1f;
                summaryCardCanvasGroup.interactable = true;
                summaryCardCanvasGroup.blocksRaycasts = true;
            }
        }

        private void ApplyFlags(string[] flags)
        {
            bool hasTempFlag = ContainsFlag(flags, "TEMP");
            bool hasHumidityFlag = ContainsFlag(flags, "HUMIDITY");

            if (flagChipTemp != null)
                flagChipTemp.SetActive(hasTempFlag);
            if (flagChipHumidity != null)
                flagChipHumidity.SetActive(hasHumidityFlag);

            if (hasTempFlag && flagChipTempText != null)
                flagChipTempText.text = "High Temp";
            if (hasHumidityFlag && flagChipHumidityText != null)
                flagChipHumidityText.text = "Low Humidity";
        }

        private void ApplyStatusStyle(string status)
        {
            if (statusChipImage == null)
                return;

            string normalized = Safe(status, string.Empty).ToUpperInvariant();
            if (normalized == "DELIVERED")
                statusChipImage.color = new Color32(34, 197, 94, 255);
            else if (normalized.Contains("TRANSIT"))
                statusChipImage.color = new Color32(245, 158, 11, 255);
            else
                statusChipImage.color = new Color32(107, 114, 128, 255);
        }

        private void ApplyGradeStyle(string grade)
        {
            if (gradeBadgeImage == null)
                return;

            string normalized = Safe(grade, string.Empty).ToUpperInvariant();
            if (normalized == "A")
                gradeBadgeImage.color = new Color32(16, 185, 129, 255);
            else if (normalized == "B")
                gradeBadgeImage.color = new Color32(59, 130, 246, 255);
            else if (normalized == "C")
                gradeBadgeImage.color = new Color32(245, 158, 11, 255);
            else
                gradeBadgeImage.color = new Color32(156, 163, 175, 255);
        }

        private static bool ContainsFlag(string[] flags, string keyword)
        {
            if (flags == null || flags.Length == 0)
                return false;

            foreach (string flag in flags)
            {
                if (!string.IsNullOrEmpty(flag) && flag.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }

        private static string Safe(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static void SetText(TextMeshProUGUI target, string value)
        {
            if (target != null)
                target.text = value;
        }
    }
}

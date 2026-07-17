using TMPro;
using UnityEngine;

namespace OrganicTraceAR.UI.Common
{
    public class StatusMessageView : MonoBehaviour
    {
        [SerializeField] private TMP_Text messageText;

        public void ShowMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }

        public void Clear()
        {
            ShowMessage(string.Empty);
        }
    }
}

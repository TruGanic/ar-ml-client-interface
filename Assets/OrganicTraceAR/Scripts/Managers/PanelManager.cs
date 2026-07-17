using System.Collections.Generic;
using UnityEngine;

namespace OrganicTraceAR.Managers
{
    public class PanelManager : MonoBehaviour
    {
        [SerializeField] private List<PanelDefinition> panels = new();
        [SerializeField] private string defaultPanelKey;

        private readonly Dictionary<string, GameObject> panelLookup = new();
        public string CurrentPanelKey { get; private set; }

        private void Awake()
        {
            BuildLookup();
            if (!string.IsNullOrWhiteSpace(defaultPanelKey))
            {
                Show(defaultPanelKey);
            }
        }

        private void BuildLookup()
        {
            panelLookup.Clear();
            foreach (var item in panels)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.Key) || item.Panel == null)
                {
                    continue;
                }

                if (!panelLookup.ContainsKey(item.Key))
                {
                    panelLookup.Add(item.Key, item.Panel);
                }
            }
        }

        public void Show(string key)
        {
            foreach (var kvp in panelLookup)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.SetActive(kvp.Key == key);
                }
            }

            CurrentPanelKey = key;
        }
    }
}

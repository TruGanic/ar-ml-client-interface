using UnityEngine;

namespace OrganicTraceAR.AR
{
    public class PersistentAROverlayAnchor : MonoBehaviour
    {
        [SerializeField] private Transform overlayRoot;
        [SerializeField] private float yOffset = 0.12f;

        public void PlaceOverlayAt(Transform trackedTransform, Camera arCamera)
        {
            if (overlayRoot == null || trackedTransform == null)
                return;

            overlayRoot.position = trackedTransform.position + new Vector3(0f, yOffset, 0f);

            if (arCamera != null)
            {
                Vector3 direction = overlayRoot.position - arCamera.transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude > 0.0001f)
                {
                    overlayRoot.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
    }
}

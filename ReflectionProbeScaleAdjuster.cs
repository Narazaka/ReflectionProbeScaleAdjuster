using UnityEngine;

namespace Narazaka.Unity.ReflectionProbeScaleAdjuster
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ReflectionProbe))]
    /// <summary>
    /// ReflectionProbeのスケールやオフセットをlossyScaleに合わせて自動調整するコンポーネント
    /// </summary>
    public class ReflectionProbeScaleAdjuster : MonoBehaviour
#if HAS_VRCSDK
        , VRC.SDKBase.IEditorOnly
#endif
    {
        [SerializeField] Vector3 originalScale;
        [SerializeField] Vector3 originalCenter;

        ReflectionProbe probe;

        void Start()
        {
            GetOriginalValues();
        }

        void Update()
        {
            GetOriginalValues();
            var lossyScale = transform.lossyScale;
            if (lossyScale == originalScale)
            {
                probe.size = originalScale;
                probe.center = originalCenter;
                return;
            }

            var newSize = new Vector3(
                originalScale.x * lossyScale.x,
                originalScale.y * lossyScale.y,
                originalScale.z * lossyScale.z
            );
            var newCenter = new Vector3(
                originalCenter.x * lossyScale.x,
                originalCenter.y * lossyScale.y,
                originalCenter.z * lossyScale.z
            );
            if (newSize != probe.size || newCenter != probe.center)
            {
                Debug.Log($"ReflectionProbeScaleAdjuster: [{GetPathOf(transform)}] Updating probe size from {probe.size} to {newSize} and center from {probe.center} to {newCenter}");
                probe.size = newSize;
                probe.center = newCenter;
            }
        }

        void GetOriginalValues()
        {
            if (probe == null) probe = GetComponent<ReflectionProbe>();
            if (originalScale == Vector3.zero && originalCenter == Vector3.zero)
            {
#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(this, "ReflectionProbeScaleAdjuster Start");
#endif
                originalScale = probe.size;
                originalCenter = probe.center;
            }
        }

        string GetPathOf(Transform t)
        {
            if (t.parent == null) return t.name;
            return GetPathOf(t.parent) + "/" + t.name;
        }
    }
}

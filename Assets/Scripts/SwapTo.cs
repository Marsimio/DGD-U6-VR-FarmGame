using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class SwapTo : MonoBehaviour
{
    [SerializeField] GameObject replacementPrefab;

    XRSocketInteractor socket;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();

        socket.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDestroy()
    {
        socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!replacementPrefab) return;

        var oldInteractable = args.interactableObject as XRBaseInteractable;
        if (oldInteractable == null) return;

        var oldGO = oldInteractable.gameObject;

        Transform parent = oldGO.transform.parent;
        Vector3 scale = oldGO.transform.localScale;

        Transform attach = socket.attachTransform != null ? socket.attachTransform : socket.transform;

        Vector3 localPos = attach.InverseTransformPoint(oldGO.transform.position);
        Quaternion localRot = Quaternion.Inverse(attach.rotation) * oldGO.transform.rotation;

        Vector3 pos = attach.TransformPoint(localPos);
        Quaternion rot = attach.rotation * localRot;

        GameObject newer = Instantiate(replacementPrefab, pos, rot, parent);
        newer.transform.localScale = scale;

        var newInteractable = newer.GetComponent<XRBaseInteractable>();
        if (newInteractable == null)
        {
            Debug.LogWarning("Replacement prefab must have an XRGrabInteractable (or XRBaseInteractable).", newer);
            Destroy(newer);
            return;
        }

        var grow = newer.GetComponent<GrowOverTime>();
        if (grow != null) grow.StartGrowing();

        Destroy(oldGO);
    }
}
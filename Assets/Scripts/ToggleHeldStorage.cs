using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToggleHeldStorage : MonoBehaviour
{
    [SerializeField] XRDirectInteractor hand;
    [SerializeField] XRInteractionManager interactionManager;
    [SerializeField] InputActionReference toggleAction;

    [SerializeField] Transform storageAnchor;

    IXRSelectInteractable stored;
    Rigidbody storedRb;
    bool storedPrevKinematic;
    bool hasStoredState;

    void Awake()
    {
        if (!hand) hand = GetComponentInChildren<XRDirectInteractor>();
        if (!interactionManager) interactionManager = Object.FindAnyObjectByType<XRInteractionManager>();
    }

    void OnEnable()
    {
        if (!toggleAction) return;
        toggleAction.action.Enable();
        toggleAction.action.performed += OnToggle;
    }

    void OnDisable()
    {
        if (!toggleAction) return;
        toggleAction.action.performed -= OnToggle;
        toggleAction.action.Disable();
    }

    void OnToggle(InputAction.CallbackContext _)
    {
        if (!hand || !interactionManager || !storageAnchor) return;

        if (hand.hasSelection)
        {
            if (stored != null) return;
            Store(hand.firstInteractableSelected);
        }
        else if (stored != null)
        {
            StartCoroutine(RetrieveNextFrame());
        }
    }

    void Store(IXRSelectInteractable item)
    {
        interactionManager.SelectExit(hand, item);

        stored = item;

        var go = ((Component)item).gameObject;

        go.transform.SetParent(storageAnchor, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        storedRb = go.GetComponent<Rigidbody>();
        if (storedRb)
        {
            storedPrevKinematic = storedRb.isKinematic;
            hasStoredState = true;

            storedRb.linearVelocity = Vector3.zero;
            storedRb.angularVelocity = Vector3.zero;
            storedRb.isKinematic = true;
        }
    }

    IEnumerator RetrieveNextFrame()
    {
        var item = stored;
        var go = ((Component)item).gameObject;

        var attach = hand.attachTransform ? hand.attachTransform : hand.transform;
        go.transform.SetParent(null, true);
        go.transform.SetPositionAndRotation(attach.position, attach.rotation);

        if (storedRb && hasStoredState)
        {
            storedRb.isKinematic = storedPrevKinematic;
            storedRb.linearVelocity = Vector3.zero;
            storedRb.angularVelocity = Vector3.zero;
            storedRb.WakeUp();
        }

        yield return null;

        interactionManager.SelectEnter(hand, item);

        stored = null;
        storedRb = null;
        hasStoredState = false;
    }
}
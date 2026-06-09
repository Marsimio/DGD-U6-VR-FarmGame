using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


[RequireComponent(typeof(XRSocketInteractor))]
public class SwapTo : MonoBehaviour
{
    XRSocketInteractor _socket;
    XRInteractionManager _manager;

    void Awake()
    {
        _socket = GetComponent<XRSocketInteractor>();
        _manager = _socket.interactionManager;

        _socket.selectEntered.AddListener(OnSelectEntered);
    }

    void OnDestroy()
    {
        if (_socket != null)
            _socket.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (_manager == null) return;

        var oldInteractable = args.interactableObject as MonoBehaviour;
        if (oldInteractable == null) return;

        var seed = oldInteractable.GetComponent<Seed>();
        if (seed == null || seed.data == null || seed.data.cropPrefab == null) return;

        StartCoroutine(SwapRoutine(oldInteractable, seed.data.cropPrefab));
    }

    IEnumerator SwapRoutine(MonoBehaviour oldInteractableMb, GameObject prefab)
    {
        GameObject oldGO = oldInteractableMb.gameObject;
        Transform parent = oldGO.transform.parent;
        Vector3 localScale = oldGO.transform.localScale;

        Transform attach = _socket.attachTransform != null ? _socket.attachTransform : _socket.transform;

        GameObject newGO = Instantiate(prefab, attach.position, attach.rotation, parent);
        newGO.transform.localScale = localScale;

        var newInteractable = newGO.GetComponent<IXRSelectInteractable>();
        var oldInteractable = oldInteractableMb as IXRSelectInteractable;

        if (newInteractable == null || oldInteractable == null)
        {
            Destroy(newGO);
            yield break;
        }

        _manager.SelectExit(_socket, oldInteractable);
        yield return null;
        _manager.SelectEnter(_socket, newInteractable);

        var grow = newGO.GetComponent<GrowOverTime>();
        if (grow != null) grow.StartGrowing();

        Destroy(oldGO);
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FencePlacer : MonoBehaviour
{
    [SerializeField] XRRayInteractor rayInteractor;
    [SerializeField] InputActionReference placeAction;
    [SerializeField] InputActionReference rotateAction;
    [SerializeField] GameObject fencePrefab;
    [SerializeField] GameObject previewPrefab;
    [SerializeField] float yOffset = 1f;

    int rotStep;
    GameObject preview;
    bool placeArmed;

    void OnEnable()
    {
        if (placeAction)
        {
            placeAction.action.Enable();
            placeAction.action.started += OnPlaceStarted;
            placeAction.action.canceled += OnPlaceReleased;
            placeArmed = false;
        }

        if (rotateAction)
        {
            rotateAction.action.Enable();
            rotateAction.action.performed += OnRotate;
        }

        if (previewPrefab && preview == null)
        {
            preview = Instantiate(previewPrefab);
            foreach (var collision in preview.GetComponentsInChildren<Collider>(true))
                collision.enabled = false;
            preview.SetActive(false);
        }
    }

    void OnDisable()
    {
        if (placeAction)
        {
            placeAction.action.started -= OnPlaceStarted;
            placeAction.action.canceled -= OnPlaceReleased;
            placeAction.action.Disable();
        }

        if (rotateAction)
        {
            rotateAction.action.performed -= OnRotate;
            rotateAction.action.Disable();
        }

        placeArmed = false;
        if (preview) preview.SetActive(false);
    }

    void OnRotate(InputAction.CallbackContext _)
    {
        rotStep = (rotStep + 1) % 4;
    }

    void OnPlaceStarted(InputAction.CallbackContext _)
    {
        placeArmed = true;
        StartCoroutine(PreviewWhileHeld());
    }

    IEnumerator PreviewWhileHeld()
    {
        while (placeArmed)
        {
            if (rayInteractor && TryGetGroundHit(out var hit))
            {
                Vector3 pos = hit.point + Vector3.up * yOffset;
                Quaternion rot = Quaternion.Euler(0f, rotStep * 90f, 0f);

                if (preview)
                {
                    preview.SetActive(true);
                    preview.transform.SetPositionAndRotation(pos, rot);
                }
            }
            else
            {
                if (preview) preview.SetActive(false);
            }

            yield return null;
        }

        if (preview) preview.SetActive(false);
    }

    void OnPlaceReleased(InputAction.CallbackContext _)
    {
        if (!placeArmed) return;
        placeArmed = false;

        if (!rayInteractor || !fencePrefab) return;
        if (!TryGetGroundHit(out var hit)) return;

        Vector3 pos = hit.point + Vector3.up * yOffset;
        Quaternion rot = Quaternion.Euler(0f, rotStep * 90f, 0f);

        Instantiate(fencePrefab, pos, rot);
    }

    bool TryGetGroundHit(out RaycastHit hit)
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out hit))
            return hit.collider != null && hit.collider.CompareTag("Ground");

        return false;
    }
}
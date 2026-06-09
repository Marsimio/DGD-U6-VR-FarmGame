using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class SitObject : MonoBehaviour
{
    public XROrigin xrOrigin;
    public Transform sitPoint;

    public bool disableLocomotionWhileSeated = true;
    public MonoBehaviour[] locomotionToToggle;

    public InputActionReference dismountAction;
    
    private Transform _preSitParent;
    private bool _isSeated;

    void OnEnable()
    {
        if (!dismountAction) return;
        dismountAction.action.Enable();
        dismountAction.action.performed += OnDismount;
    }

    void OnDisable()
    {
        if (!dismountAction) return;
        dismountAction.action.performed -= OnDismount;
        dismountAction.action.Disable();
    }
    
    public void Sit()
    {
        if (_isSeated || xrOrigin == null || sitPoint == null) return;

        _preSitParent = xrOrigin.transform.parent;

        Transform hmd = xrOrigin.Camera.transform;
        Vector3 originToHmd = hmd.position - xrOrigin.transform.position;
        xrOrigin.transform.position = sitPoint.position - originToHmd;

        xrOrigin.transform.SetParent(sitPoint, true);

        if (disableLocomotionWhileSeated)
            SetLocomotion(false);

        _isSeated = true;
    }
    
    void OnDismount(InputAction.CallbackContext _)
    {
        if (_isSeated) Stand();
    }

    public void Stand()
    {
        if (!_isSeated || xrOrigin == null) return;

        xrOrigin.transform.SetParent(_preSitParent, true);

        if (disableLocomotionWhileSeated)
            SetLocomotion(true);

        _isSeated = false;
    }

    private void SetLocomotion(bool enabled)
    {
        if (locomotionToToggle == null) return;
        foreach (var m in locomotionToToggle)
            if (m) m.enabled = enabled;
    }
}
using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    [SerializeField] private PlayerCharacter _owner;
    [SerializeField] private float _detectorDistance;
    [SerializeField] private Vector3 _detectorBoxSize = Vector3.one;
    [SerializeField] private LayerMask _interactableLayer;

    private IInteractable _detectedInteractable;
    private bool _isInteracting;
    public bool Enabled {get; private set;} = true;

    private void Update()
    {
        UpdateDetection();
    }

    private void UpdateDetection()
    {
        if (_isInteracting)
        {
            _isInteracting = false;
            return;
        }
        if (!Enabled)
        {
            return;
        }
        Transform cameraTransform = Camera.main.transform;
        bool isDetectingInteractable = Physics.BoxCast(
            cameraTransform.position, //center of boxcast
            _detectorBoxSize*0.5f, //halfsize of boxcast
            cameraTransform.forward, //direction of boxcast
            out RaycastHit hit, // whether hit or not
            Quaternion.identity, // orientation?
            _detectorDistance, // maximum distance of boxcast
            _interactableLayer // boxcast layer mask
        ); 
        if (isDetectingInteractable)
        {
            IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _detectedInteractable = interactable;
            }
        }
    }

    public void Interact()
    {
        if (_detectedInteractable != null && Enabled)
        {
            _detectedInteractable.Interact(_owner);
            _detectedInteractable = null;
            _isInteracting = true;
        }
    }

    public void SetEnabled(bool isEnabled)
    {
        Enabled = isEnabled;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Transform cameraTransform = Camera.main.transform;

        if (!Enabled)
        {
            return;
        }

        bool isDetectingInteractable = Physics.BoxCast(
            cameraTransform.position, //center of boxcast
            _detectorBoxSize*0.5f, //halfsize of boxcast
            cameraTransform.forward, //direction of boxcast
            out RaycastHit hit, // whether hit or not
            Quaternion.identity, // orientation?
            _detectorDistance, // maximum distance of boxcast
            _interactableLayer // boxcast layer mask
        ); 
        
        if (isDetectingInteractable)
        {
            Gizmos.color = Color.green;
            // line from camera to object
            Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * hit.distance);
            // wire cube around object
            Gizmos.DrawWireCube(cameraTransform.position + cameraTransform.forward * hit.distance, _detectorBoxSize);
        }
        else
        {
            Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * _detectorDistance);
            Gizmos.DrawWireCube(cameraTransform.position + cameraTransform.forward * _detectorDistance, _detectorBoxSize);
        }
    }

}
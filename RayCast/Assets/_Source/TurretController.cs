using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private Vector3 rotationOffset = new Vector3(0, 90, 0);
    [SerializeField] private Material highlightMaterial;
    
    private Material _originalMaterial;
    private GameObject _highlightedObject;
    private Camera _mainCamera;
    private RaycastHit _lastRaycastHit;
    private bool _hasHit;

    private void Update()
    {
        _mainCamera = Camera.main;  
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        _hasHit = Physics.Raycast(ray, out _lastRaycastHit, 100f);

        RotateTurretToMouse(ray);
        HighlightTarget();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private Vector3 GetTargetPoint(Ray ray)
    {
        if (_hasHit)
        {
            return _lastRaycastHit.point;
        }
        
        Plane turretPlane = new Plane(Vector3.up, transform.position);
        if (turretPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return ray.GetPoint(100f);
    }

    private void RotateTurretToMouse(Ray ray)
    {
        Vector3 targetPoint = GetTargetPoint(ray);
        Vector3 direction = targetPoint - transform.position;
        if (direction.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
    }

    private void HighlightTarget()
    {
        if (_hasHit)
        {
            GameObject hitObject = _lastRaycastHit.collider.gameObject;
            if (hitObject.CompareTag("Cube"))
            {
                if (_highlightedObject != null && _highlightedObject != hitObject)
                {
                    ResetHighlight();
                }

                if (_highlightedObject != hitObject)
                {
                    _highlightedObject = hitObject;
                    if (hitObject.TryGetComponent(out Renderer renderer))
                    {
                        _originalMaterial = renderer.material;
                        renderer.material = highlightMaterial;
                    }
                }
                return;
            }
        }

        ResetHighlight();
    }

    private void ResetHighlight()
    {
        if (_highlightedObject != null)
        {
            if (_highlightedObject.TryGetComponent(out Renderer renderer) && _originalMaterial != null)
            {
                renderer.material = _originalMaterial;
            }
            _highlightedObject = null;
            _originalMaterial = null;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = firePoint.forward * shootForce;
        }
    }

    private void OnDrawGizmos()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        Vector3 targetPoint = GetTargetPoint(ray);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, targetPoint);
        Gizmos.DrawWireSphere(targetPoint, 0.3f);
    }
}
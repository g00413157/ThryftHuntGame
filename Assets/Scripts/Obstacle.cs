using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Obstacle : MonoBehaviour
{
    private bool wasHit;

    void Awake()
    {
        FitColliderToVisual();
    }

    public void HandlePlayerHit(GameObject playerObject)
    {
        if (wasHit || playerObject == null || GameManager.Instance == null)
            return;

        wasHit = true;
        GameManager.Instance.HitObstacle();

        Collider obstacleCollider = GetComponent<Collider>();
        if (obstacleCollider != null)
            obstacleCollider.enabled = false;
    }

    void FitColliderToVisual()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        if (boxCollider == null || renderers.Length == 0)
            return;

        Bounds combinedBounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            combinedBounds.Encapsulate(renderers[i].bounds);

        Vector3 localCenter = transform.InverseTransformPoint(combinedBounds.center);
        Vector3 lossyScale = transform.lossyScale;

        boxCollider.center = localCenter;
        boxCollider.size = new Vector3(
            SafeDivide(combinedBounds.size.x, lossyScale.x) * 0.9f,
            SafeDivide(combinedBounds.size.y, lossyScale.y) * 0.9f,
            SafeDivide(combinedBounds.size.z, lossyScale.z) * 0.9f
        );
        boxCollider.isTrigger = false;
    }

    float SafeDivide(float value, float scaleAxis)
    {
        float safeScale = Mathf.Abs(scaleAxis) < 0.0001f ? 1f : Mathf.Abs(scaleAxis);
        return value / safeScale;
    }
}

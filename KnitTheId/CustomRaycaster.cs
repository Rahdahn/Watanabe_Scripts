using UnityEngine;

public static class CustomRaycaster
{
    public static RaycastHit2D? RaycastByOrderInLayer(Vector2 origin, Vector2 direction, float distance, LayerMask layerMask)
    {
        // ‘SƒqƒbƒgŒ‹‰Ê‚ðŽæ“¾
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask);

        if (hits.Length == 0)
            return null;

        // Å‚à sortingOrder ‚ª‚‚¢‚à‚Ì‚ð’T‚·
        RaycastHit2D topHit = hits[0];
        int topPriority = GetSortingPriority(hits[0].collider);

        foreach (var hit in hits)
        {
            int order = GetSortingPriority(hit.collider);
            if (order > topPriority)
            {
                topPriority = order;
                topHit = hit;
            }
        }

        return topHit;
    }

    private static int GetSortingPriority(Collider2D collider)
    {
        Renderer renderer = collider.GetComponent<Renderer>();
        if (renderer == null)
            return 0;

        int layerValue = SortingLayer.GetLayerValueFromID(renderer.sortingLayerID);
        return layerValue * 1000 + renderer.sortingOrder;
    }
}

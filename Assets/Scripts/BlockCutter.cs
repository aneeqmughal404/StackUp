using UnityEngine;

public static class BlockCutter
{
    public static float PerfectThreshold = 0.1f;

    public static bool Cut(BlockController previous, BlockController current)
    {
        if (previous == null || current == null)
            return false;

        bool isX = current.MoveOnX();
        Transform previousTransform = previous.transform;
        Transform currentTransform = current.transform;

        float previousPosition;
        float currentPosition;
        float previousSize;

        if (isX)
        {
            previousPosition = previousTransform.position.x;
            currentPosition = currentTransform.position.x;
            previousSize = previousTransform.localScale.x;
        }
        else
        {
            previousPosition = previousTransform.position.z;
            currentPosition = currentTransform.position.z;
            previousSize = previousTransform.localScale.z;
        }

        float difference = currentPosition - previousPosition;
        float threshold = GodModeManager.Instance != null && GodModeManager.Instance.IsGodMode ? 100f : PerfectThreshold;

        if (Mathf.Abs(difference) <= threshold)
        {
            Vector3 newPosition = currentTransform.position;

            if (isX)
                newPosition.x = previousPosition;
            else
                newPosition.z = previousPosition;

            currentTransform.position = newPosition;
            currentTransform.localScale = previousTransform.localScale;

            ScoreManager.Instance.AddScore(3);
            GameplayUIService.ShowPerfect();
            AudioManager.Instance?.PlayPerfect();

            return true;
        }

        float overlap = previousSize - Mathf.Abs(difference);
        if (overlap <= 0f)
        {
            AudioManager.Instance?.PlayGameOver();
            return false;
        }

        float currentSize = isX ? currentTransform.localScale.x : currentTransform.localScale.z;
        float previousHalf = previousSize / 2f;
        float currentHalf = currentSize / 2f;

        float previousMin = previousPosition - previousHalf;
        float previousMax = previousPosition + previousHalf;
        float currentMin = currentPosition - currentHalf;
        float currentMax = currentPosition + currentHalf;

        float overlapMin = Mathf.Max(previousMin, currentMin);
        float overlapMax = Mathf.Min(previousMax, currentMax);
        overlap = overlapMax - overlapMin;

        Vector3 originalScale = currentTransform.localScale;
        Vector3 originalPosition = currentTransform.position;

        Vector3 scale = originalScale;
        if (isX)
            scale.x = overlap;
        else
            scale.z = overlap;

        currentTransform.localScale = scale;

        Vector3 adjustedPosition = currentTransform.position;
        float newCenter = (overlapMin + overlapMax) / 2f;

        if (isX)
            adjustedPosition.x = newCenter;
        else
            adjustedPosition.z = newCenter;

        currentTransform.position = adjustedPosition;

        SpawnFallingPieces(current, originalScale, originalPosition, overlap, overlapMin, overlapMax, isX);

        ScoreManager.Instance.AddScore(1);
        AudioManager.Instance?.PlayPlace();

        return true;
    }

    private static void SpawnFallingPieces(BlockController current, Vector3 originalScale, Vector3 originalPosition, float overlap, float overlapMin, float overlapMax, bool isX)
    {
        float originalSize = isX ? originalScale.x : originalScale.z;
        float currentCenter = isX ? originalPosition.x : originalPosition.z;
        float currentMin = currentCenter - originalSize / 2f;
        float currentMax = currentCenter + originalSize / 2f;

        float leftSize = overlapMin - currentMin;
        if (leftSize > 0.01f)
        {
            float leftCenter = currentMin + leftSize / 2f;
            SpawnSingleFallingPiece(current, originalScale, originalPosition, leftSize, leftCenter, isX, true);
        }

        float rightSize = currentMax - overlapMax;
        if (rightSize > 0.01f)
        {
            float rightCenter = currentMax - rightSize / 2f;
            SpawnSingleFallingPiece(current, originalScale, originalPosition, rightSize, rightCenter, isX, false);
        }
    }

    private static void SpawnSingleFallingPiece(BlockController current, Vector3 originalScale, Vector3 originalPosition, float pieceSize, float pieceCenter, bool isX, bool isLeft)
    {
        GameObject piece = GameObject.CreatePrimitive(PrimitiveType.Cube);
        piece.transform.SetParent(current.transform.parent, true);
        piece.transform.rotation = current.transform.rotation;

        Vector3 pieceScale = originalScale;
        if (isX)
            pieceScale.x = pieceSize;
        else
            pieceScale.z = pieceSize;

        piece.transform.localScale = pieceScale;

        Vector3 piecePosition = originalPosition;
        if (isX)
            piecePosition.x = pieceCenter;
        else
            piecePosition.z = pieceCenter;

        piece.transform.position = piecePosition;

        MeshRenderer originalRenderer = current.GetComponent<MeshRenderer>();
        MeshRenderer pieceRenderer = piece.GetComponent<MeshRenderer>();

        if (originalRenderer != null && pieceRenderer != null)
        {
            pieceRenderer.sharedMaterial = originalRenderer.sharedMaterial;

            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            originalRenderer.GetPropertyBlock(propertyBlock);
            pieceRenderer.SetPropertyBlock(propertyBlock);
        }

        Rigidbody body = piece.AddComponent<Rigidbody>();
        body.mass = 1f;
        body.linearDamping = 0.1f;
        body.angularDamping = 0.15f;
        body.useGravity = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;

        body.AddForce(isLeft ? Vector3.left * 2f : Vector3.right * 2f, ForceMode.Impulse);
        body.AddTorque(Vector3.forward * 1.5f, ForceMode.Impulse);

        Object.Destroy(piece, 3f);
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float moveDistance = 6f;

    [Header("Color")]
    [SerializeField] private float colorTransitionSpeed = 5f;

    private bool isBaseBlock;
    private bool moveOnX;
    private bool isMoving;
    private float moveProgress;
    private Vector3 movementCenter;
    private Vector3 spawnPosition;
    private BlockSpawner spawner;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;
    private MeshCollider cachedMeshCollider;
    private Rigidbody cachedRigidbody;
    private Color currentColor;
    private Color targetColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (meshRenderer != null)
        {
            meshRenderer.GetPropertyBlock(propertyBlock);
            if (meshRenderer.sharedMaterial != null)
            {
                // Enable GPU Instancing to drastically reduce batches for blocks
                meshRenderer.sharedMaterial.enableInstancing = true; 
            }
        }

        cachedMeshCollider = GetComponent<MeshCollider>();
        cachedRigidbody = GetComponent<Rigidbody>();
    }

    public void SetSpawner(BlockSpawner s)
    {
        spawner = s;
    }

    public bool MoveOnX()
    {
        return moveOnX;
    }

    public void Initialize(bool moveOnXAxis, bool baseBlock)
    {
        moveOnX = moveOnXAxis;
        isBaseBlock = baseBlock;

        if (isBaseBlock)
        {
            isMoving = false;
            return;
        }

        spawnPosition = transform.position;
        movementCenter = transform.position;
        moveProgress = 0f;
        isMoving = true;
    }

    public void Initialize(bool moveOnXAxis, bool baseBlock, Vector3 moveCenter, float moveDistance, Vector3 blockScale)
    {
        moveOnX = moveOnXAxis;
        isBaseBlock = baseBlock;

        if (isBaseBlock)
        {
            isMoving = false;
            return;
        }

        transform.localScale = blockScale;
        this.moveDistance = moveDistance;

        movementCenter = moveCenter;
        spawnPosition = transform.position;
        moveProgress = 0f;
        isMoving = true;

        transform.position = GetStartPosition();
    }

    private void Update()
    {
        if (!isBaseBlock && isMoving)
        {
            Move();

            if (WasInputPressed())
                Stop();
        }

        UpdateColor();
    }

    private void Move()
    {
        moveProgress += Time.deltaTime * moveSpeed;

        float offset = Mathf.PingPong(moveProgress, moveDistance * 2f) - moveDistance;

        Vector3 position = spawnPosition;

        if (moveOnX)
            position.x = movementCenter.x + offset;
        else
            position.z = movementCenter.z + offset;

        transform.position = position;
    }

    private Vector3 GetStartPosition()
    {
        Vector3 position = spawnPosition;

        if (moveOnX)
            position.x = movementCenter.x - moveDistance;
        else
            position.z = movementCenter.z - moveDistance;

        return position;
    }

    private bool WasInputPressed()
    {
        if (GameManager.Instance == null)
            return false;

        if (GameManager.Instance.CurrentState != GameState.Playing)
            return false;

        // If a UI button (e.g. Pause) was just tapped, consume the flag and ignore this frame
        if (GameplayUIManager.IgnoreNextGameplayInput)
        {
            GameplayUIManager.IgnoreNextGameplayInput = false;
            return false;
        }

        bool hasSpaceInput = Input.GetKeyDown(KeyCode.Space);
        
        bool hasTouchInput = false;
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began)
                {
                    if (!IsPointerOverUI(touch.position))
                    {
                        hasTouchInput = true;
                    }
                }
            }
        }

        bool hasMouseInput = Input.GetMouseButtonDown(0) && !IsPointerOverUI(Input.mousePosition);

        if (hasSpaceInput || hasTouchInput || hasMouseInput)
        {
            if (hasSpaceInput) Debug.Log("Input Detected: Spacebar");
            if (hasTouchInput) Debug.Log("Input Detected: Touch");
            if (hasMouseInput) Debug.Log("Input Detected: Mouse Click");
            
            return true;
        }

        return false;
    }

    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            if (result.gameObject == null)
                continue;

            if (result.gameObject.GetComponent<Selectable>() != null)
                return true;

            if (result.gameObject.GetComponent<Button>() != null)
                return true;

            if (result.gameObject.GetComponent<Toggle>() != null)
                return true;

            if (result.gameObject.GetComponent<Slider>() != null)
                return true;

            if (result.gameObject.GetComponent<Scrollbar>() != null)
                return true;

            if (result.gameObject.GetComponent<Dropdown>() != null)
                return true;
        }

        return false;
    }

    private void Stop()
    {
        if (!isMoving)
            return;

        isMoving = false;
        spawner.OnBlockStopped(this);
    }

    public void SetColor(Color color)
    {
        targetColor = color;

        if (currentColor == default)
        {
            currentColor = color;

            if (meshRenderer != null)
            {
                propertyBlock.SetColor("_Color", color);
                meshRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }

    private void UpdateColor()
    {
        if (meshRenderer == null)
            return;

        if (currentColor == targetColor)
            return;

        currentColor = Color.Lerp(
            currentColor,
            targetColor,
            colorTransitionSpeed * Time.deltaTime);

        propertyBlock.SetColor("_Color", currentColor);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void Fall()
    {
        if (cachedMeshCollider == null)
            cachedMeshCollider = GetComponent<MeshCollider>();

        if (cachedMeshCollider == null)
            cachedMeshCollider = gameObject.AddComponent<MeshCollider>();

        cachedMeshCollider.convex = true;

        if (cachedRigidbody == null)
            cachedRigidbody = GetComponent<Rigidbody>();

        if (cachedRigidbody == null)
            cachedRigidbody = gameObject.AddComponent<Rigidbody>();

        cachedRigidbody.useGravity = true;
        cachedRigidbody.interpolation = RigidbodyInterpolation.Interpolate;

        StartCoroutine(DisableAfterDelay(3f));
    }

    private System.Collections.IEnumerator DisableAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (cachedRigidbody != null)
        {
            Destroy(cachedRigidbody);
            cachedRigidbody = null;
        }

        if (cachedMeshCollider != null)
        {
            Destroy(cachedMeshCollider);
            cachedMeshCollider = null;
        }

        gameObject.SetActive(false);
    }
}
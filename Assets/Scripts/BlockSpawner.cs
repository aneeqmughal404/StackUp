using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BlockController blockPrefab = null;
    [SerializeField] private BlockController baseBlock = null;

    [Header("Settings")]
    [SerializeField] private float blockHeight = 0.5f;
    [SerializeField] private float spawnOffset = 6f;

    [Header("Pooling")]
    [SerializeField] private int poolPrewarm = 8;

    private readonly System.Collections.Generic.List<BlockController> pool = new();
    private BlockController previousBlock;
    private BlockController currentBlock;
    private bool moveOnX = true;
    private BatchReducer batchReducer;
    private GameManager gameManager;
    private Camera mainCamera;
    private CameraController cameraController;

    private void Awake()
    {
        batchReducer = Object.FindFirstObjectByType<BatchReducer>();
        gameManager = Object.FindFirstObjectByType<GameManager>();
        mainCamera = Camera.main;

        if (mainCamera != null)
            cameraController = mainCamera.GetComponent<CameraController>();

        if (blockPrefab != null)
        {
            for (int i = 0; i < poolPrewarm; i++)
            {
                var block = Instantiate(blockPrefab, Vector3.down * 1000f, Quaternion.identity);
                block.gameObject.SetActive(false);
                pool.Add(block);
            }
        }
    }

    private void Start()
    {
        Debug.Log($"BlockSpawner: Start - scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, ColorManager.Instance={(ColorManager.Instance != null)}");

        if (batchReducer != null)
            batchReducer.ReduceBatches();

        SetupBaseBlock();
        SpawnNextBlock();

        if (gameManager != null)
            gameManager.StartGame();
    }

    private void SetupBaseBlock()
    {
        if (baseBlock == null)
        {
            Debug.LogError("BlockSpawner could not find the assigned base block.");
            return;
        }

        previousBlock = baseBlock;
        previousBlock.Initialize(false, true);

        if (ColorManager.Instance != null)
        {
            Debug.Log("BlockSpawner: Setting base block color from ColorManager");
            previousBlock.SetColor(ColorManager.Instance.GetBlockColor());
        }
        else
        {
            Debug.LogWarning("BlockSpawner: ColorManager.Instance is null when setting base block color");
        }
    }

    private void SpawnNextBlock()
    {
        if (previousBlock == null)
            SetupBaseBlock();

        if (previousBlock == null)
        {
            Debug.LogError("No valid base block.");
            return;
        }

        if (blockPrefab == null)
        {
            Debug.LogError("No block prefab assigned.");
            return;
        }

        Vector3 center = previousBlock.transform.position;
        Vector3 position = center;
        position.y += blockHeight;

        float axisSize = moveOnX
            ? previousBlock.transform.localScale.x
            : previousBlock.transform.localScale.z;

        float moveDistanceForBlock = axisSize / 2f + spawnOffset;

        currentBlock = GetPooledBlock(position);

        currentBlock.Initialize(
            moveOnX,
            false,
            center,
            moveDistanceForBlock,
            previousBlock.transform.localScale
        );

        currentBlock.SetSpawner(this);

        if (ColorManager.Instance != null)
            currentBlock.SetColor(ColorManager.Instance.GetBlockColor());
    }

    private BlockController GetPooledBlock(Vector3 position)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] == null)
                continue;

            if (!pool[i].gameObject.activeInHierarchy)
            {
                BlockController block = pool[i];

                block.gameObject.SetActive(true);
                block.transform.position = position;
                block.transform.rotation = Quaternion.identity;

                return block;
            }
        }

        BlockController newBlock = Instantiate(blockPrefab, position, Quaternion.identity);
        pool.Add(newBlock);

        return newBlock;
    }

    public void OnBlockStopped(BlockController current)
    {
        bool success = BlockCutter.Cut(previousBlock, current);

        if (!success)
        {
            current.Fall();

            if (gameManager != null)
                gameManager.GameOver();

            return;
        }

        previousBlock = current;
        moveOnX = !moveOnX;

        if (mainCamera != null)
        {
            if (cameraController == null)
            {
                cameraController = mainCamera.GetComponent<CameraController>();
                if (cameraController == null)
                    cameraController = mainCamera.gameObject.AddComponent<CameraController>();
            }

            cameraController.LiftCamera(blockHeight);
        }

        SpawnNextBlock();
    }
}
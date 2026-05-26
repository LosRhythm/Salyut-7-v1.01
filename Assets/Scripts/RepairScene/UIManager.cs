using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI设置")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private TMP_Text successMessagePrefab;
    [SerializeField] private float messageDisplayTime = 3f;

    // 用于存放临时UI元素的容器
    private GameObject uiContainer;

    private void Awake()
    {
        // 单例设置
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 确保Canvas存在
        SetupCanvas();

        // 创建非持久化的UI容器
        SetupUIContainer();
    }

    private void SetupCanvas()
    {
        if (mainCanvas == null)
        {
            // 尝试查找现有Canvas
            mainCanvas = FindAnyObjectByType<Canvas>();

            // 如果没有找到，创建新的Canvas
            if (mainCanvas == null)
            {
                GameObject canvasObj = new GameObject("Main Canvas");
                mainCanvas = canvasObj.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // 添加必要的Canvas组件
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();

                Debug.Log("自动创建了Main Canvas", canvasObj);
            }
        }

        // 确保Canvas设置正确
        if (mainCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            Debug.LogWarning("建议将Canvas的RenderMode设置为ScreenSpaceOverlay以确保UI正确显示", mainCanvas);
        }
    }

    // 创建一个非持久化的UI容器，用于存放动态生成的UI
    private void SetupUIContainer()
    {
        // 查找场景中的UI容器（不查找资源）
        uiContainer = GameObject.Find("Dynamic UI Container");

        // 如果没找到且当前在运行模式下，创建新的容器
        if (uiContainer == null && Application.isPlaying)
        {
            uiContainer = new GameObject("Dynamic UI Container");

            // 检查Canvas是否是预制体资源
            if (mainCanvas != null && !IsPrefabAsset(mainCanvas.gameObject))
            {
                // 只有当Canvas是场景中的对象时才设置父对象
                uiContainer.transform.SetParent(mainCanvas.transform, false);
            }
            else
            {
                Debug.LogWarning("Canvas是预制体资源，UI容器将不设置父对象");
            }
        }
    }

    // 检查对象是否是预制体资源
    private bool IsPrefabAsset(GameObject obj)
    {
        // 预制体资源不属于任何场景且不是实例化的对象
        return obj.scene.buildIndex == -1 && !obj.activeInHierarchy;
    }

    public Canvas GetCanvas()
    {
        return mainCanvas;
    }

    public void ShowSuccessMessage(string message)
    {
        if (successMessagePrefab == null)
        {
            Debug.LogWarning("请在UIManager中设置successMessagePrefab");
            return;
        }

        if (mainCanvas == null)
        {
            Debug.LogError("Canvas不存在，无法显示消息");
            return;
        }

        // 获取合适的父对象
        Transform parent = GetValidParent();

        // 创建成功消息
        TMP_Text messageText = Instantiate(successMessagePrefab, parent);
        messageText.text = message;

        // 设置消息位置
        RectTransform rect = messageText.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }

        StartCoroutine(HideMessageAfterDelay(messageText, messageDisplayTime));
    }

    // 获取有效的父对象（非预制体资源）
    private Transform GetValidParent()
    {
        // 优先使用UI容器
        if (uiContainer != null)
        {
            return uiContainer.transform;
        }

        // 检查Canvas是否是有效的父对象
        if (mainCanvas != null && !IsPrefabAsset(mainCanvas.gameObject))
        {
            return mainCanvas.transform;
        }

        // 最后使用默认的根节点
        return null;
    }

    private IEnumerator HideMessageAfterDelay(TMP_Text message, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 检查消息对象是否已被销毁
        if (message == null)
            yield break;


        // 渐隐效果
        float fadeTime = 0.5f;
        float elapsed = 0;
        Color originalColor = message.color;

        while (elapsed < fadeTime)
        {
            // 每帧都检查对象是否存在
            if (message == null)
                yield break;

            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
            message.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // 最后检查一次再销毁
        if (message != null)
            Destroy(message.gameObject);
    }
}

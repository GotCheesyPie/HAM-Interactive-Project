using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MoralChoiceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public bool isDraggable = true; 
    public string targetTag; 
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Canvas rootCanvas;

    // Event untuk memberitahu Manager
    public System.Action<GameObject> OnValidDrop; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        // Cari Canvas paling atas agar drag mulus di atas semua UI
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        // 1. Simpan Data Asli
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();

        // 2. Pindahkan ke Root Canvas agar visual drag di atas segalanya
        // (Ini solusi agar tidak tertutup objek lain di grid)
        transform.SetParent(rootCanvas.transform, true);

        // 3. Matikan Raycast agar mouse bisa menembus objek ini dan mendeteksi target di bawahnya
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Gerakkan mengikuti mouse (dengan kompensasi scale factor canvas)
        rectTransform.anchoredPosition += eventData.delta / rootCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Cek objek apa yang ada di bawah mouse
        // Kita gunakan pointerEnter dari eventData
        GameObject targetObj = eventData.pointerEnter;
        
        // Debugging untuk melihat apa yang kena raycast (Bantu fix bug no. 2)
        if (targetObj != null) 
            Debug.Log($"Dropped on: {targetObj.name} (Tag: {targetObj.tag})");
        else 
            Debug.Log("Dropped on NOTHING");

        bool dropSuccess = false;

        // Cek Tag Target
        if (targetObj != null && targetObj.CompareTag(targetTag))
        {
            dropSuccess = true;
        }

        if (dropSuccess)
        {
            // --- DROP BERHASIL ---
            Debug.Log($"VALID DROP: {name} -> {targetObj.name}");
            OnValidDrop?.Invoke(targetObj);
            
            // Hancurkan/Matikan objek ini
            gameObject.SetActive(false);
            // Kembalikan parent agar struktur hierarki rapi sebelum dimatikan (opsional tapi bagus)
            transform.SetParent(originalParent); 
        }
        else
        {
            Debug.Log("INVALID DROP: Returning to start.");
            
            transform.SetParent(originalParent);
            
            transform.SetSiblingIndex(originalSiblingIndex);
        }
    }

    public void SetDraggable(bool state)
    {
        isDraggable = state;
    }
}
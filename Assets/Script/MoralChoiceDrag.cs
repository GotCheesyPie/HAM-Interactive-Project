using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class MoralChoiceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    public bool isDraggable = true; // Apakah objek ini boleh digerakkan?
    public string targetTag; // Tag objek tujuan (misal: "Trash1" atau "Trash2")
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;
    
    // Event untuk memberitahu Manager
    public System.Action<GameObject> OnValidDrop; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        // Agar objek yang di-drag berada di layer paling atas (render last)
        // Kita pindahkan sementara ke root canvas atau parent paling tinggi
        transform.SetAsLastSibling(); 

        // Matikan raycast agar kita bisa mendeteksi objek di BAWAH kartu ini
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; // Visual feedback (transparan)
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;
        
        // Menggerakkan objek mengikuti mouse/jari
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDraggable) return;

        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;

        // Cek objek apa yang ada di bawah mouse saat dilepas
        if (eventData.pointerEnter != null && eventData.pointerEnter.CompareTag(targetTag))
        {
            // DROP BERHASIL!
            Debug.Log($"Dropped {name} on {eventData.pointerEnter.name}");
            OnValidDrop?.Invoke(eventData.pointerEnter);
            
            // Hancurkan atau sembunyikan objek ini
            gameObject.SetActive(false);
        }
        else
        {
            // DROP GAGAL (Kembali ke posisi awal)
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    // Fungsi helper untuk mengaktifkan drag (untuk Trash #1 nanti)
    public void SetDraggable(bool state)
    {
        isDraggable = state;
    }
}
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;

// Memerlukan interface ini untuk mendeteksi drag
public class OpinionCard : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("UI References")]
    public TextMeshProUGUI opinionText;
    public TextMeshProUGUI authorText;

    [Header("Swipe Settings")]
    public float swipeThreshold = 150f; // Jarak minimum untuk swipe
    public float rotationFactor = 10f; // Seberapa miring kartu saat di-drag
    public float moveSpeed = 20f; // Kecepatan kartu terbang
    public float snapBackSpeed = 10f; // Kecepatan kartu kembali

    // --- Data Internal ---
    private Opinion opinionData;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector2 dragStartPosition;
    private bool isDragging = false;
    
    // --- Sistem Kartu Stack ---
    // Event ini akan memberitahu Manager (OpinionReviewSwiper)
    // bahwa kartu ini telah di-swipe.
    public event Action<Opinion, bool> OnCardSwiped; // <data, didSwipeRight>

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    // Fungsi ini dipanggil oleh Manager untuk mengisi data kartu
    public void SetData(Opinion op)
    {
        opinionData = op;
        opinionText.text = op.opinionText;
        authorText.text = $"Ditulis oleh {op.authorName}, berumur {op.authorAge}, dari {op.authorCity}";
    }

    // ----------------------------------------------------
    // --- Implementasi Swipe Mechanic (Deteksi) ---
    // ----------------------------------------------------

    public void OnPointerDown(PointerEventData eventData)
    {
        // Catat posisi awal drag
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out dragStartPosition
        );
        isDragging = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector2 currentDragPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out currentDragPosition
        );

        // Hitung perpindahan
        Vector2 difference = currentDragPosition - dragStartPosition;

        // --- Pemrograman Animasi Swipe ---
        // 1. Pindahkan Kartu
        rectTransform.anchoredPosition = originalPosition + difference;
        
        // 2. Rotasi Kartu
        float rotationAngle = difference.x * rotationFactor / Screen.width;
        rectTransform.rotation = Quaternion.Euler(0, 0, -rotationAngle * 10f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        float currentX = rectTransform.anchoredPosition.x;

        if (Mathf.Abs(currentX) > swipeThreshold)
        {
            // --- SWIPE BERHASIL ---
            // true jika ke kanan (Agree), false jika ke kiri (Disagree)
            bool swipedRight = currentX > 0;
            StartCoroutine(SwipeOffScreen(swipedRight));
        }
        else
        {
            // --- GAGAL SWIPE (Kembali ke tengah) ---
            StartCoroutine(SnapBack());
        }
    }

    // ----------------------------------------------------
    // --- Animasi dan Logika Stack ---
    // ----------------------------------------------------
    
    IEnumerator SnapBack()
    {
        while (Vector2.Distance(rectTransform.anchoredPosition, originalPosition) > 0.1f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originalPosition, Time.deltaTime * snapBackSpeed);
            rectTransform.rotation = Quaternion.Lerp(rectTransform.rotation, Quaternion.identity, Time.deltaTime * snapBackSpeed);
            yield return null;
        }
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.rotation = Quaternion.identity;
    }

    IEnumerator SwipeOffScreen(bool swipedRight)
    {
        // Trigger event ke Manager
        OnCardSwiped?.Invoke(opinionData, swipedRight);

        // Animasi terbang
        float targetX = swipedRight ? Screen.width * 1.5f : -Screen.width * 1.5f;
        Vector2 targetPosition = new Vector2(targetX, rectTransform.anchoredPosition.y);

        while (Mathf.Abs(rectTransform.anchoredPosition.x - targetX) > 10f)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            yield return null;
        }

        // Hancurkan kartu setelah selesai
        Destroy(gameObject);
    }
}
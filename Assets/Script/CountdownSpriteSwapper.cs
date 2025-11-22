using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CountdownSpriteSwapper : MonoBehaviour
{
    public Sprite[] sprites = new Sprite[10];
    public Image Image;
    int CurrentIdx = 0;

    void Update()
    {
        ShowSpriteAtIndex(CurrentIdx);
    }

    void ShowSpriteAtIndex(int idx)
    {
        if(idx == sprites.Count()) { return; }
        if (Image.sprite == sprites[idx]) { return; }
        Image.sprite = sprites[idx];
    }

    [ContextMenu("IncrementIndex")]
    public void IncrementIndex()
    {
        CurrentIdx++;
    }
}

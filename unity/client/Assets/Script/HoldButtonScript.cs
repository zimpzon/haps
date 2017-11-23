using UnityEngine;

[ExecuteInEditMode]
public class HoldButtonScript : MonoBehaviour
{
    public Sprite SpriteLocked;
    public Sprite SpriteUnlocked;
    public bool IsLocked;

    SpriteRenderer renderer_;

    public void SetLocked(bool locked)
    {
        IsLocked = locked;
        UpdateVisual(IsLocked);
    }

    public void UpdateVisual(bool locked)
    {
        if (renderer_ == null)
            renderer_ = GetComponentInChildren<SpriteRenderer>(true);

        renderer_.sprite = locked ? SpriteLocked : SpriteUnlocked;
    }

    void OnValidate()
    {
        UpdateVisual(IsLocked);
    }

    void Awake()
    {
        UpdateVisual(IsLocked);
    }
}

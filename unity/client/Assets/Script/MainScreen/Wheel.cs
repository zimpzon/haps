using Assets.Script;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    float SpinSpeed = 12.0f;

    public RectTransform WheelRoot;
    public GameObject iconPrefab;
    public Sprite[] iconSprites;

    int iconCount;
    int errorIconIdx;
    IFinal9Provider final9Provider_;
    bool hasFinal9_;
    float spinStartTime_;

    private List<SpriteRenderer> row0Renderers = new List<SpriteRenderer>();
    private List<SpriteRenderer> row1Renderers = new List<SpriteRenderer>();
    private List<SpriteRenderer> row2Renderers = new List<SpriteRenderer>();
    private List<int> row0Icons = new List<int>();
    private List<int> row1Icons = new List<int>();
    private List<int> row2Icons = new List<int>();

    // Position: Position in list. Clamps at length - 3.
    float position0 = 0.0f;
    float position1 = 0.0f;
    float position2 = 0.0f;

    float targetPosition0 = 0.0f;
    float targetPosition1 = 0.0f;
    float targetPosition2 = 0.0f;

    float spriteW;
    float spriteH;
    float canvasW;
    float canvasH;

    [System.NonSerialized]
    public bool IsSpinning = false;

    List<SpriteRenderer> GetRenderers(int[] indices)
    {
        List<SpriteRenderer> result = new List<SpriteRenderer>();
        for (int i = 0; i < indices.Length; ++i)
        {
            int idx = indices[i];
            int row = idx % 3;
            if (row == 0)
                result.Add(row0Renderers[idx / 3]);
            else if (row == 1)
                result.Add(row1Renderers[idx / 3]);
            else if (row == 2)
                result.Add(row2Renderers[idx / 3]);
        }
        return result;
    }

    public List<int> GetFinal9()
    {
        // Round instead of clamp to be 100% to hit the correct icon
        int top0 = Mathf.RoundToInt(position0);
        int top1 = Mathf.RoundToInt(position1);
        int top2 = Mathf.RoundToInt(position2);

        return new List<int> {
            row0Icons[top0 + 0], row1Icons[top1 + 0], row2Icons[top2 + 0],
            row0Icons[top0 + 1], row1Icons[top1 + 1], row2Icons[top2 + 1],
            row0Icons[top0 + 2], row1Icons[top1 + 2], row2Icons[top2 + 2],
        };
    }

    Sprite GetSpriteForIcon(int idx)
    {
        Sprite sprite = iconSprites[idx];
        return sprite;
    }

    void Awake()
    {
        InitIcons();
    }

    public void SetFinal9Provider(IFinal9Provider provider)
    {
        final9Provider_ = provider;
    }

    void InitIcons()
    {
        canvasW = WheelRoot.rect.width;
        canvasH = WheelRoot.rect.height;

        spriteW = canvasW / 3;
        spriteH = canvasH / 3;

        iconCount = iconSprites.Length - 1; // Last one is used for out of bounds
        errorIconIdx = iconSprites.Length - 1;

        // Setup x-positions and initial icons
        for (int y = 0; y < 4; ++y)
        {
            for (int x = 0; x < 3; ++x)
            {
                int xx = x - 1;
                int yy = y - 2;
                var pos = new Vector3(xx * spriteW, yy * spriteH, -1.0f);
                var icon = (GameObject)Instantiate(iconPrefab);
                icon.transform.SetParent(WheelRoot);
                const float SpriteScale = 1.25f;
                icon.transform.localScale = new Vector3(SpriteScale, SpriteScale, SpriteScale);
                icon.transform.localPosition = pos;
                var renderer = icon.GetComponent<SpriteRenderer>();
                int iconIdx = (x + y * 3) % iconCount;
                if (x == 0)
                {
                    row0Renderers.Add(renderer);
                    row0Icons.Add(iconIdx);
                }
                else if (x == 1)
                {
                    row1Renderers.Add(renderer);
                    row1Icons.Add(iconIdx);
                }
                else if (x == 2)
                {
                    row2Renderers.Add(renderer);
                    row2Icons.Add(iconIdx);
                }
            }
        }

        UpdateRows();
    }

    private void SetNewRoll(List<int> spindle, List<int> newData, ref float position)
    {
        // Save the 3 currently visible
        int truncPos = (int)position;
        int v0, v1, v2;
        v0 = spindle[truncPos + 0];
        v1 = spindle[truncPos + 1];
        v2 = spindle[truncPos + 2];

        // Assign the new list
        spindle.Clear();
        spindle.AddRange(newData);

        // Append the 3 currently visible
        spindle.Add(v0);
        spindle.Add(v1);
        spindle.Add(v2);

        // Finally adjust position to show the last 3
        position = spindle.Count - 3;
    }

    void SetSpinTarget(bool fullSpin, bool holdFlag, float currentPosition, ref float targetPosition)
    {
        if (holdFlag)
        {
            // Hold, do not move
            targetPosition = currentPosition;
            return;
        }
        else if (fullSpin)
        {
            // Spin to top of list (0)
            targetPosition = 0;
            return;
        }
        else
        {
            // 1-down
            targetPosition = currentPosition - 1;
        }
    }

    void InsertForOneDown(List<int> row, bool holdFlag, ref float position)
    {
        if (holdFlag)
            return;

        // Insert duplicate of bottom visible row at top
        const int BottomVisibleIdx = 2;
        row.Insert(0, row[BottomVisibleIdx]);
        position += 1;
    }

    public void DoNudge(bool[] holdFlags)
    {
        InsertForOneDown(row0Icons, holdFlags[0], ref position0);
        InsertForOneDown(row1Icons, holdFlags[1], ref position1);
        InsertForOneDown(row2Icons, holdFlags[2], ref position2);

        const bool FullSpin = false;
        SetSpinTarget(FullSpin, holdFlags[0], position0, ref targetPosition0);
        SetSpinTarget(FullSpin, holdFlags[1], position1, ref targetPosition1);
        SetSpinTarget(FullSpin, holdFlags[2], position2, ref targetPosition2);

        IsSpinning = true;
    }

    public void DoFullSpin(bool[] holdFlags)
    {
        const bool FullSpin = true;
        SetSpinTarget(FullSpin, holdFlags[0], position0, ref targetPosition0);
        SetSpinTarget(FullSpin, holdFlags[1], position1, ref targetPosition1);
        SetSpinTarget(FullSpin, holdFlags[2], position2, ref targetPosition2);

        IsSpinning = true;
        spinStartTime_ = Time.time;
    }

    public void SetNextSpin(List<int> newRow0, List<int> newRow1, List<int> newRow2)
    {
        SetNewRoll(row0Icons, newRow0, ref position0);
        SetNewRoll(row1Icons, newRow1, ref position1);
        SetNewRoll(row2Icons, newRow2, ref position2);
        hasFinal9_ = false;
    }

    void UpdateRow(float position, List<SpriteRenderer> renderers, List<int> icons)
    {
        // Show 4 icons from this clamped position
        int iconIdx = (int)position;
        float frac = position - iconIdx;

        float yOffset = spriteH;

        // Top to bottom of row
        foreach (var renderer in renderers)
        {
            if (iconIdx < 0 || iconIdx >= icons.Count)
            {
                Debug.LogErrorFormat("iconIdx out of range: {0}", iconIdx);
                iconIdx = 0;
            }

            int spriteIdx = icons[iconIdx++];
            if (spriteIdx < 0 || spriteIdx >= iconSprites.Length)
            {
                spriteIdx = errorIconIdx;
            }

            Sprite sprite = iconSprites[spriteIdx];
            if (renderer.sprite != sprite)
                renderer.sprite = sprite;

            var pos = renderer.transform.localPosition;
            pos.y = yOffset + frac * spriteH;
            renderer.transform.localPosition = pos;
            yOffset -= spriteH;
        }
    }

    void UpdateRows()
    {
        UpdateRow(position0, row0Renderers, row0Icons);
        UpdateRow(position1, row1Renderers, row1Icons);
        UpdateRow(position2, row2Renderers, row2Icons);
    }

    bool MoveToTarget(float target, ref float position)
    {
        position -= Time.deltaTime * SpinSpeed;

        if (position < target)
        {
            position = target;
            return false;
        }

        return true;
    }

    void Update()
    {
        if (IsSpinning)
        {
            if (!hasFinal9_)
            {
                // Check every frame if the final9 are ready
                hasFinal9_ = final9Provider_.TryClaimFinal9(row0Icons, row1Icons, row2Icons);
            }

            int moveCount = 0;
            moveCount += (MoveToTarget(targetPosition0, ref position0) ? 1 : 0);
            moveCount += (MoveToTarget(targetPosition1, ref position1) ? 1 : 0);
            moveCount += (MoveToTarget(targetPosition2, ref position2) ? 1 : 0);

            bool atLeastOneStoppedMoving = moveCount < 3;
            if (atLeastOneStoppedMoving && !hasFinal9_)
            {
//                Debug.LogFormat("Spin time: {0}", Time.time - spinStartTime_);
                spinStartTime_ = Time.time;

                // Until we have final9 we keep restarting spin. All 3 are restarted as soon as one stops, to stay in sync.
                // 4 can be shown at a time (partially)
                position0 = row0Icons.Count - 4;
                position1 = row1Icons.Count - 4;
                position2 = row2Icons.Count - 4;

                moveCount = 3;
            }

            UpdateRows();

            IsSpinning = moveCount > 0;
        }
    }
}

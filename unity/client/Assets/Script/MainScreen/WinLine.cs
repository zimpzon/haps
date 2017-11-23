using System;
using UnityEngine;

public class WinLine : MonoBehaviour
{
    public RectTransform Parent;

    [NonSerialized]
    public int[] Line = new int[] { 3, 4, 5 }; // Default: middle line

    private LineRenderer line_;
    private Material lineMat_;

    private Vector3 target0_;
    private Vector3 target1_;
    private Vector3 current0_;
    private Vector3 current1_;

    private float z_;
    private float speed_ = 100.0f;

    public Color ColorFlash = Color.green;
    public Color Color = Color.white;
    float Width = 15f;
    float WidthFlash = 22f;
    private float flashT_ = 0.0f;

    float LineSizeHorizontal = 440f;
    float LineSizeDiagonal = 440f;

    void Awake()
    {
        z_ = transform.position.z;
        float y = transform.position.y;
        current0_ = new Vector3(0, y, z_);
        current1_ = new Vector3(0, y, z_);
        line_ = GetComponent<LineRenderer>();
        lineMat_ = line_.material;
        UpdateTarget(Line, 100);
    }

    public void SetDefault()
    {
        Line = new int[] { 3, 4, 5 }; // Default: middle line
        UpdateTarget(Line, 100);
    }

    public void UpdateTarget(int[] cells, float speed)
    {
        speed_ = speed;
        float cellH = Parent.rectTransform().rect.height / 3;

        // Horizontal: 012, 345, 678
        // Diagonal  : 048, 642 
        bool isLeftToRight = cells[0] < cells[1]; // Only 642 is right to left
        float signX = isLeftToRight ? 1 : -1;
        float x0 = -LineSizeHorizontal * 0.5f * signX;
        float x1 = LineSizeHorizontal * 0.5f * signX;

        bool isHorizontal = cells[2] == cells[0] + 2;
        float y0 = isHorizontal ? (cells[0] / 3 - 1) * -cellH : LineSizeDiagonal * 0.5f;
        float y1 = isHorizontal ? (cells[0] / 3 - 1) * -cellH : -LineSizeDiagonal * 0.5f;

        target0_ = new Vector3(x0, y0, z_);
        target1_ = new Vector3(x1, y1, z_);

        this.Line = cells;
        flashT_ = 1.0f;
    }

    void Update()
    {
        float step = Time.deltaTime * speed_ * 100;
        current0_ = Vector3.MoveTowards(current0_, target0_, step);
        current1_ = Vector3.MoveTowards(current1_, target1_, step);
        line_.SetPosition(0, current0_);
        line_.SetPosition(1, current1_);
        float width = Mathf.Lerp(Width, WidthFlash, (flashT_ - 0.75f) * 4);
        line_.startWidth = width;
        line_.endWidth = width;

        lineMat_.color = Color.Lerp(Color, ColorFlash, flashT_);
        float colorSpeed = 0.02f * speed_;
        flashT_ -= Time.deltaTime * colorSpeed;
    }
}

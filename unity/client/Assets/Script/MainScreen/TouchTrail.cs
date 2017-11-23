using UnityEngine;
using UnityEngine.EventSystems;

public class TouchTrail : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Transform Parent;
    public Transform Trail;

    private Vector2 startSwipe_;
    private Vector2 endSwipe_;
    private Main main_;
    Camera camera_;

    void Start()
    {
        main_ = GameObject.Find("Main").GetComponent<Main>();
        camera_ = Camera.main;
    }
	
    Vector2 GetMouseLocalPos()
    {
        Vector2 localPoint;
        RectTransform recTrans = this.rectTransform();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(recTrans, InputManager.GetMousePosition(), camera_, out localPoint);
        return localPoint;
    }

    void TrySetWinLine(Vector2 a, Vector2 b)
    {
        float iconH = this.rectTransform().rect.height  / 3;
        if (a.x > b.x)
        {
            // Swap a and b so a swipe is always from left to right
            Vector2 tmp = b;
            b = a;
            a = tmp;
        }

        int y0 = Mathf.RoundToInt(a.y / iconH) + 1;
        int y1 = Mathf.RoundToInt(b.y / iconH) + 1;
        if (y0 == y1)
        {
            // Horizontal
            switch (y0)
            {
                case 0: main_.SetWinLine(new int[] { 6, 7, 8 }); break;
                case 1: main_.SetWinLine(new int[] { 3, 4, 5 }); break;
                case 2: main_.SetWinLine(new int[] { 0, 1, 2 }); break;
            }
        }
        else if (y1 < y0)
        {
            // Diagonal, top left -> bot right
            main_.SetWinLine(new int[] { 0, 4, 8 });
        }
        else
        {
            // Diagonal, bot left, top right
            main_.SetWinLine(new int[] { 6, 4, 2 });
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (main_.SpinState != Main.SpinStateEnum.AwaitingUserDecisions)
            return;

        Vector3 mousePos = InputManager.GetMousePosition();
        mousePos.z = 0;
        mousePos = camera_.ScreenToWorldPoint(mousePos);
        mousePos.z = -1;
        Trail.position = mousePos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (main_.SpinState != Main.SpinStateEnum.AwaitingUserDecisions)
            return;

        startSwipe_ = GetMouseLocalPos();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (main_.SpinState != Main.SpinStateEnum.AwaitingUserDecisions)
            return;

        endSwipe_ = GetMouseLocalPos();
        TrySetWinLine(startSwipe_, endSwipe_);
    }
}

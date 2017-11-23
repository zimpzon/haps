using UnityEngine;

public class ScrollingUV : MonoBehaviour
{
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);
    public Material Material;

    Vector2 uvOffset = Vector2.zero;

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        Material.mainTextureOffset = uvOffset;
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class YardScript : MonoBehaviour
{
    public Camera YardCamera;
    public Transform Ball;
    public Transform BallShadow;

    void SwitchToYardCamera()
    {
        // Main camera clears all
        Camera.main.enabled = false;
    }

    private void Start()
    {
        SwitchToYardCamera();
    }

    void Update()
    {
        if (Camera.main != null && Camera.main.enabled)
            Camera.main.enabled = false;

        Vector3 shadowPos = Ball.position;
        shadowPos.z -= 0.1f;
        BallShadow.position = shadowPos;

        if (InputManager.DeviceBackButtonActivated())
        {
            SceneManager.LoadScene("GameScene");
        }
    }
}

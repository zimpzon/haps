using System.Collections;
using UnityEngine;
using UnityEngine.AI;

class Spring1D
{
    public float Target = 1.0f;
    public float Value = 1.2f;
    public float Velocity = 0.0f; // v
    public float Stiffness = 1.5f; // k
    public float Dampen = 0.0f; // b
    public float TimeScale = 1.0f;

    public void Update(float deltaTime)
    {
        // f = -kx - bv
        float x = Target - Value;
        float f = Stiffness * x - Dampen * Velocity;

        Velocity += f * deltaTime * TimeScale;
        Value += Velocity * deltaTime;
    }
};

// Mystery Manor just moves static images with a nameplate below. Could work. Maybe jumping.
// Pet rarity. Grey, green, blue, purple, orange name.
public class PetScript : MonoBehaviour
{
    public Transform RenderTransform;
    public Transform ShadowTransform;
    public NavMeshAgent Agent;
    public Transform Ball;
    public SpriteRenderer PetRenderer;
    public string PetId;

    Rigidbody ballBody_;
    Vector3 targetPos_;
    float timeNextRandom_;
    Vector3 shadowBaseScale_;
    Vector3 petBaseScale_;
    Spring1D heightSpring_ = new Spring1D();
    float springOffset_;

    private void Awake()
    {
        shadowBaseScale_ = ShadowTransform.localScale;
        petBaseScale_ = RenderTransform.localScale;
        ballBody_ = Ball.GetComponent<Rigidbody>();
        springOffset_ = Random.value * 10;
    }

    private void Start()
    {
        StartCoroutine(AI());
    }

    void SetRandomTarget()
    {
        // x: -2.3 - 2.3
        // y: -4.3 - 4.5
        float x = Random.Range(-2.3f, 2.3f);
        float y = Random.Range(-4.3f, 4.5f);
        targetPos_ = new Vector2(x, y);

        Agent.SetDestination(targetPos_);

        timeNextRandom_ = Time.time + Random.Range(5.0f, 6.0f);
    }

    // Chase and kick ball for x seconds
    IEnumerator KickBall()
    {
        float timeEnd = Time.time + 5 + Random.value * 10;
        int kickCount = 0;
        while (Time.time < timeEnd)
        {
            Agent.SetDestination(Ball.position);

            DebugText.SetLine(PetId, "Chase ball: {0}, kicks = {1}", timeEnd - Time.time, kickCount);
            if (Agent.remainingDistance < 0.5f)
            {
                Vector3 kickDirection = (Ball.position - RenderTransform.position).normalized;
                ballBody_.AddForce(kickDirection * 5);
                ballBody_.AddTorque(Random.insideUnitSphere.normalized, ForceMode.Impulse);
                kickCount++;
            }
            yield return null;
        }
    }

    void TargetRandomPosition()
    {
        // x 2.4, z 4
        Vector3 randomPos = new Vector3((Random.value * 5) - 2.5f, 0.0f, (Random.value * 8) - 4.0f);
        Agent.SetDestination(randomPos);
    }

    IEnumerator RandomWalk()
    {
        TargetRandomPosition();
        float timeEnd = Time.time + 3 + Random.value * 5;
        while (Time.time < timeEnd)
        {
            DebugText.SetLine(PetId, "Random walk: {0}, remaining = {1}", timeEnd - Time.time, Agent.remainingDistance);
            yield return null;
        }
    }

    IEnumerator AI()
    {
        while (true)
        {
            yield return RandomWalk();
            yield return KickBall();
            DebugText.SetLine(PetId, "Relaxing");
            yield return new WaitForSeconds(Random.value * 10);
        }
    }

    void Update()
    {
        float LookDir = RenderTransform.position.x < targetPos_.x ? -1 : 1;
        float springSpeed = 6.0f;
        float springY = Mathf.Sin((Time.time + springOffset_) * springSpeed) * 0.05f;
        float springX = -springY;
        RenderTransform.localScale = new Vector3(petBaseScale_.x + springX * LookDir, petBaseScale_.y + springY, petBaseScale_.z);

        PetRenderer.sortingOrder = Mathf.RoundToInt(RenderTransform.position.z * 100f) * -5;
    }
}

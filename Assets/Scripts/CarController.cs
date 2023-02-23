using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    Rigidbody2D rb;
    float steeringAmount, speed;

    [Header("Basic Params")]
    public float accelerationPower = 5f;
    public float steeringPower = 5f;
    public LayerMask layerMask;


    [Header("Controls")]
    public bool front;
    public bool back;
    public bool left;
    public bool right;
    public bool dead;

    [Header("Debug Vars")]
    public float[] distList = new float[8];
    public bool debug = true;

    [Header("Checkpoints")]
    public int zoneNum;
    public int zoneMax;
    public float score;


    // Use this for initialization
    void Start()
    {
        StartCoroutine(WaitAndDie());
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (left)
        {
            steeringAmount = 1;
        }
        else if (right)
        {
            steeringAmount = -1;
        }
        else
        {
            steeringAmount = 0;
        }

        if (front)
        {
            speed = 1;
        }
        else if (back)
        {
            speed = -1;
        }
        else
        {
            speed = 0;
        }

        // Reset Controls
        front = false;
        back = false;
        left = false;
        right = false;

        // Move
        steeringAmount *= steeringPower;
        speed *= accelerationPower;
        if (speed > 0)
        {
            rb.AddRelativeForce(Vector2.up * speed);
        }
        else
        {
            rb.velocity *= 0.9f;
        }
        transform.Rotate(new Vector3(0, 0, steeringAmount));

        distList[0] = DrawRay(transform.up);
        distList[1] = DrawRay(-transform.up);
        distList[2] = DrawRay(transform.right);
        distList[3] = DrawRay(-transform.right);

        distList[4] = DrawRay((transform.up + transform.right).normalized);
        distList[5] = DrawRay((transform.up - transform.right).normalized);
        distList[6] = DrawRay((-transform.up + transform.right).normalized);
        distList[7] = DrawRay((-transform.up - transform.right).normalized);
    }

    float DrawRay(Vector3 dir)
    {
        float dist = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            dist = (hit.point - (Vector2)transform.position).magnitude;
            if (debug)
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
            }
        }
        return dist;
    }

    public void Kill()
    {
        dead = true;
        rb.angularVelocity = 0;
        rb.velocity = Vector2.zero;
        GetComponent<NeuralNetwork>().enabled = false;
        this.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Kill();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (zoneNum == -1 || (zoneNum + 1) % zoneMax == collision.gameObject.GetComponent<CheckpointWarn>().checkNum)
        {
            zoneNum = collision.gameObject.GetComponent<CheckpointWarn>().checkNum;
            score++;
        }
    }

    IEnumerator WaitAndDie()
    {
        float before = score;
        yield return new WaitForSeconds(2);
        if (before == score)
        {
            Kill();
        }
        else
        {
            StartCoroutine(WaitAndDie());
        }
    }
}

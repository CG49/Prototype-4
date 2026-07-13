using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private bool isHoming;
    private readonly float speed = 15.0f;
    private readonly float rocketStrength = 15.0f;
    private readonly float aliveTimer = 5.0f;

    private Transform target;

    public void Fire(Transform newTarget)
    {
        target = newTarget;
        isHoming = true;
        Destroy(gameObject, aliveTimer);
    }

    void Update()
    {
        if (isHoming && target != null)
        {
            Vector3 moveDirection = (target.transform.position - transform.position).normalized;

            transform.position += speed * Time.deltaTime * moveDirection;

            transform.LookAt(target);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target != null)
        {
            if (collision.gameObject.CompareTag(target.tag))
            {
                Rigidbody targetRigidbody = collision.gameObject.GetComponent<Rigidbody>();

                Vector3 away = -collision.contacts[0].normal;

                targetRigidbody.AddForce(away * rocketStrength, ForceMode.Impulse);

                Destroy(gameObject);
            }
        }
    }
}

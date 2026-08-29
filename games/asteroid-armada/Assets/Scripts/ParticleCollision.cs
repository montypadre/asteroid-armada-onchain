using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParticleCollision(GameObject other)
    {
        if (other.gameObject.tag == "Player")
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
            PlayerHealth playerHealth = other.gameObject.GetComponent<PlayerHealth>();

            Rigidbody rb = other.GetComponent<Rigidbody>();
            int i = 0;

            while (i < numCollisionEvents)
            {
                if (rb)
                {
                    Vector3 pos = collisionEvents[i].intersection;
                    Vector3 force = collisionEvents[i].velocity * 10;
                    rb.AddForce(force);
                    playerHealth.DealDamage(5);
                }
                i++;
            }
        }
    }
}

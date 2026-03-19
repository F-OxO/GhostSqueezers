using UnityEngine;

public class Ghost : MonoBehaviour
{
    // Life counter for the ghost
    [SerializeField] private float lifeCounter = 100;
    [SerializeField] public float speed = 4.0f;               // Constant forward speed
    [SerializeField] public float rotationSpeed = 2.0f;       // Speed of steering towards the player
    [SerializeField] public float wobble = 50.0f;       // Speed of steering towards the player
    [SerializeField] public Doll player;  
    [SerializeField] public GameObject camera;               
    public GameObject explosion;
    public float light_damage = 0f;

    private float rand = 0f;

    void Start()
    {
        rand = Random.Range(0f, 3f);
    }


    // Update is called once per frame
    void Update()
    {
        // Check if the life counter is less than or equal to zero
        if (lifeCounter <= 0)
        {
            GameObject explo = Instantiate(explosion, transform.position, camera.transform.rotation);
            explo.SetActive(true);
            Destroy(gameObject);
            // gameObject.SetActive(false);
            // lifeCounter = 100f;
        }

        // Calculate the direction to the player
        Vector3 toPlayer = player.transform.position - transform.position;
        Vector3 directionToPlayer = toPlayer.normalized;

        if(player.myoStrength < 0.4f && toPlayer.magnitude < 1f)
        {
            directionToPlayer *= -1f;
        }

        // Determine the forward direction of the object
        Vector3 forward = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            
        // Calculate the oscillation effect
        float oscillationY = Mathf.Sin(Time.time * 6.15f + 13f + rand) * wobble;
        float oscillationZ = Mathf.Sin(Time.time * 6.87f + 21f + rand) * wobble;
        Quaternion oscillationRotation = Quaternion.Euler(0, oscillationY, oscillationZ); // Apply oscillation on the Y-axis
        
        // Combine the target rotation with the oscillation
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation * oscillationRotation, rotationSpeed * Time.deltaTime);
        
        // Apply the new rotation to the object
        transform.rotation = newRotation;

        // Move the object forward at constant speed
        transform.position += forward * speed * Time.deltaTime * (1f + Mathf.Sin(Time.time * 4f + rand) * .33f);


        float rad_damage = Mathf.Max(0f, 1f - (50f * toPlayer.magnitude * toPlayer.magnitude));
        TakeDamage(rad_damage * light_damage * (1f - player.myoStrength));
    }

    // Method to reduce the ghost's life
    public void TakeDamage(float damage)
    {
        lifeCounter -= damage;
        lifeCounter = Mathf.Max(lifeCounter, 0); // Ensure life doesn't go below zero
    }
}

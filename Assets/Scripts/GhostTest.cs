using UnityEngine;

public class Ghost : MonoBehaviour
{
    // Life counter for the ghost
    private float lifeCounter = 100;

    // Update is called once per frame
    void Update()
    {
        // Check if the life counter is less than or equal to zero
        if (lifeCounter <= 0)
        {
            RemoveGhost();
        }
    }

    // Method to reduce the ghost's life
    public void TakeDamage(float damage)
    {
        lifeCounter -= damage;
        lifeCounter = Mathf.Max(lifeCounter, 0); // Ensure life doesn't go below zero
    }

    // Method to remove the ghost from the game
    private void RemoveGhost()
    {
        // You can add any effects here before removal (e.g., animations)
        Destroy(gameObject); // Removes the game object from the scene
    }
}

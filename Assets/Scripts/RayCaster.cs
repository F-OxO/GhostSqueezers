using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public float rayDistance = 100f; // Distance to cast the ray


    public float CastRay(Ray ray, float damage)
    {
        RaycastHit hit;

        // Check if the ray hits any object
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            // Check if the hit object has an Enemy component
            Ghost ghost = hit.collider.GetComponent<Ghost>();
            if (ghost != null)
            {
                ghost.TakeDamage(damage); // Decrease the enemy's counter
            }


            return hit.distance;
        }

        return rayDistance;
    }
}

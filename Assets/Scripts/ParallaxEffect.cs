using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    private Transform cam;

    [SerializeField] private float parallaxEffect;

    private float length;
    private float xPosition;

    private void Start()
    {
        cam = Camera.main.transform;

        length = GetComponent<SpriteRenderer>().bounds.size.x;

        xPosition = transform.position.x;
    }

    private void Update()
    {
        float distanceMoved = cam.position.x *(1 - parallaxEffect);

        float distanceToMove = cam.position.x *parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove,transform.position.y,transform.position.z);

        if (distanceMoved >xPosition + length)
        {
            xPosition += length;
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class StoneProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifetime = 2f;

    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        body.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignora áreas de interação (triggers) e o próprio jogador.
        if (other.isTrigger) return;
        if (other.GetComponentInParent<PlayerInventory>() != null) return;

        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum ClothingType
    {
        Shoes,
        Jeans,
        Tshirt
    }

    public ClothingType type;
    public int value = 1;
    public float missDistanceBehindPlayer = 2f;

    private Transform player;
    private bool resolved;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.player != null)
            player = GameManager.Instance.player;
    }

    void Update()
    {
        if (resolved || player == null || GameManager.Instance == null || !GameManager.Instance.IsGameActive)
            return;

        if (transform.position.z < player.position.z - missDistanceBehindPlayer)
        {
            resolved = true;
            GameManager.Instance.MissClothing();
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        bool hitPlayer =
            other.CompareTag("Player") ||
            other.GetComponentInParent<PlayerLaneRunner>() != null ||
            other.GetComponentInParent<CharacterController>() != null;

        if (!hitPlayer || resolved)
            return;

        if (GameManager.Instance != null)
        {
            resolved = true;
            ApplyEffect();
        }

        Destroy(gameObject);
    }

    void ApplyEffect()
    {
        // ✅ ALWAYS count as clothing
        GameManager.Instance.AddClothing(value);

        switch (type)
        {
            case ClothingType.Shoes:
                GameManager.Instance.ActivateSpeedBoost(3f);
                break;

            case ClothingType.Jeans:
                GameManager.Instance.AddScore(value * 10); // bonus score
                break;

            case ClothingType.Tshirt:
                GameManager.Instance.AddCombo(2); // combo boost
                break;
        }
    }
}

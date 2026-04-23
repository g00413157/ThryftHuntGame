using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 2f;
    public float lifetime = 1f;

    private float timer;

    public void Setup(int amount)
    {
        text.text = "+" + amount;
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer > lifetime)
            Destroy(gameObject);
    }
}
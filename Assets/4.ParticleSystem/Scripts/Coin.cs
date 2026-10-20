using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public ParticleSystem particle;
    public SpriteRenderer sprite;

    // Игрок коснулся монетки: прячем её и запускаем эффект
    private void OnTriggerEnter2D(Collider2D other)
    {
        sprite.enabled = false;
        // Выключаем коллайдер, чтобы монетку нельзя было подобрать второй раз
        GetComponent<Collider2D>().enabled = false;
        particle.Play();
    }
}

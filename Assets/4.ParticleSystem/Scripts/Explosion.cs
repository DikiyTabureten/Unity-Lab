using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem particle;
    public SpriteRenderer sprite;

    // Бочка столкнулась: прячем спрайт и запускаем взрыв
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Спрайт уже спрятан — бочка уже взорвалась, второй раз не взрываем
        if (!sprite.enabled) return;

        sprite.enabled = false;
        // Выключаем физику бочки: она больше не падает набок и осколки о неё не бьются
        GetComponent<Rigidbody2D>().simulated = false;
        particle.Play();
    }
}

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
        sprite.enabled = false;
        particle.Play();
    }
}

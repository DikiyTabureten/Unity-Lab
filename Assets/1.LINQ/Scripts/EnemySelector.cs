using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySelector : MonoBehaviour
{
    private Entity[] _entities; 
    private void Start()
    {
        _entities = FindObjectsOfType<Entity>();
    }

    public void PaintAll()
    {
        _entities
            .Select(e => e.GetComponent<SpriteRenderer>())
            .ToList()
            .ForEach(PaintRed);
    }

    // Задание 1: летающие враги, у которых HP больше 50
    public void PaintAirStrong()
    {
        ResetAll();
        _entities
            .Where(e => e.GetComponent<Enemy>())
            .Where(e => e.Type == Entity.EntityType.Air)
            .Where(e => e.health > 50)
            .Select(e => e.GetComponent<SpriteRenderer>())
            .ToList()
            .ForEach(Highlight);
    }

    // Задание 2: летающие грибочки
    public void PaintAirMushrooms()
    {
        ResetAll();
        _entities
            .Where(e => e.GetComponent<Mushrooms>())
            .Where(e => e.Type == Entity.EntityType.Air)
            .Select(e => e.GetComponent<SpriteRenderer>())
            .ToList()
            .ForEach(Highlight);
    }

    // Задание 3: первый враг в массиве
    public void PaintFirst()
    {
        ResetAll();
        var e = _entities
            .Where(e => e.GetComponent<Enemy>())
            .FirstOrDefault();
        if (e)
            Highlight(e.GetComponent<SpriteRenderer>());
    }

    // Сбросить цвет у всех, чтобы подсветки не накладывались
    public void ResetAll()
    {
        StopAllCoroutines();
        _entities
            .Select(e => e.GetComponent<SpriteRenderer>())
            .ToList()
            .ForEach(ResetColor);
    }

    // Выделить спрайт на 3 секунды
    public void Highlight(SpriteRenderer spriteRenderer)
    {
        StartCoroutine(Flash(spriteRenderer));
    }

    private IEnumerator Flash(SpriteRenderer spriteRenderer)
    {
        PaintRed(spriteRenderer);
        yield return new WaitForSeconds(3f);

        ResetColor(spriteRenderer);
    }

    public void PaintRed(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = Color.red;
    }

    public void ResetColor(SpriteRenderer spriteRenderer) => spriteRenderer.color = Color.white;

}
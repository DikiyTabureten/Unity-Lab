using System;
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


    public void PaintRed(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = Color.red;
    }

    public void ResetColor(SpriteRenderer spriteRenderer) => spriteRenderer.color = Color.white;

}
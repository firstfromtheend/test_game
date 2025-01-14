using System.Collections.Generic;
using UnityEngine;

public class ItemPlace : MonoBehaviour
{
    [SerializeField] private int _maxCapacity = 5;
    [SerializeField] private List<GameObject> _draggables = new List<GameObject>();
    private Vector2 _bounds;
    private Vector2 _xBorders;
    private Vector2 _yBorders;

    [SerializeField] private bool _canTake = true;
    public bool CanTake { get { return _canTake; } }

    private void Awake()
    {
        // опеределяем размеры места для расстовки предметов
        _bounds = GetComponent<SpriteRenderer>().bounds.extents;
        _xBorders = new Vector2(transform.position.x - _bounds.x, transform.position.x + _bounds.x);
        _yBorders = new Vector2(transform.position.y - _bounds.y, transform.position.y + _bounds.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //добавляем предметы на полку
        var typeGameObject = collision.gameObject.TryGetComponent<Draggable>(out var draggableComponent);
        if (typeGameObject)
        {
            if (!_draggables.Contains(collision.gameObject))
            {
                _draggables.Add(collision.gameObject);
                for (int i = 0; i < _draggables.Count; i++)
                {
                    _draggables[i].GetComponent<SpriteRenderer>().sortingOrder = i;
                }
                collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder = _draggables.Count - 1;
            }
        }
        CheckFreeSpace();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //удаляем предметы с полки
        var typeGameObject = collision.gameObject.TryGetComponent<Draggable>(out var draggableComponent);
        if (typeGameObject && _draggables.Contains(collision.gameObject))
        {
            _draggables.Remove(collision.gameObject);
        }
        CheckFreeSpace();
    }

    public Vector2 GetXBorders()
    {
        //отдаем горизонтальные границы по запросу
        return _xBorders;
    }

    public Vector2 GetYBorders()
    {
        //отдаем вертикальные границы по запросу
        return _yBorders;
    }

    private void CheckFreeSpace()
    {
        //проверяем есть ли место на полке
        if (_draggables.Count <= _maxCapacity)
        {
            _canTake = true;
        }
        else _canTake = false;
    }
}
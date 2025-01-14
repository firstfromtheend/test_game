using UnityEngine;

public class Draggable : MonoBehaviour
{
    private Vector3 _mouseOffset;
    [SerializeField] private Rigidbody2D _rigidbody2;

    [SerializeField] private bool _isInItemPlace = false;

    [SerializeField] Vector2 _selfSize;

    private Vector2 _xBorders;
    private Vector2 _yBorders;

    [SerializeField] private bool _canPlace;

    private void Awake()
    {
        _rigidbody2 = GetComponent<Rigidbody2D>();
        _selfSize = GetComponent<SpriteRenderer>().bounds.extents;
    }

    private Vector3 GetMousePositioonIntheWorld()
    {
        //захватываем позицию курсора и переводим в мировые координаты
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;
    }

    private void OnMouseDown()
    {
        //определяем оффсет положения курсора что бы не было смещения при перетаскивании предмета
        _mouseOffset = gameObject.transform.position - GetMousePositioonIntheWorld();
        _rigidbody2.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void OnMouseDrag()
    {
        //передвигаем предмет за курсором
        transform.position = GetMousePositioonIntheWorld() + _mouseOffset;
    }

    private void OnMouseUp()
    {
        //проверяем находится ли предмет над полкой и  есть ли место на ней
        if (_isInItemPlace && _canPlace)
        {
            Debug.Log("hit!");
            SetItemToPlace(_xBorders, _yBorders);
            _rigidbody2.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            _rigidbody2.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    public void SetItemToPlace(Vector2 xData, Vector2 yData)
    {
        //ставим предмет на место
        var newXPos = Mathf.Clamp(transform.position.x, xData.x, xData.y);
        var newYPos = Mathf.Clamp(transform.position.y, yData.x + _selfSize.y, yData.y);

        _rigidbody2.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //проверяем есть можно ли поставить предмет на полку при его падении на неё, то есть не при непосредственном перетаскивании
        _canPlace = collision.gameObject.GetComponent<ItemPlace>().CanTake;
        if (collision.gameObject.layer == 7 && _canPlace)
        {
            _isInItemPlace = true;
            _rigidbody2.constraints = RigidbodyConstraints2D.FreezeAll;
            GetItemPlaceBorders(collision.gameObject);
            SetItemToPlace(_xBorders, _yBorders);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            _isInItemPlace = false;
        }
    }

    private void GetItemPlaceBorders(GameObject itemPlace)
    {
        //получаем данные о границах полки с которым столкнулся предмет
        _xBorders = itemPlace.GetComponent<ItemPlace>().GetXBorders();
        _yBorders = itemPlace.GetComponent<ItemPlace>().GetYBorders();
    }
}

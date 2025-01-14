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
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return mousePos;
    }

    private void OnMouseDown()
    {
        _mouseOffset = gameObject.transform.position - GetMousePositioonIntheWorld();
        _rigidbody2.constraints = RigidbodyConstraints2D.FreezeRotation;
        CheckRayCastHit();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMousePositioonIntheWorld() + _mouseOffset;
    }

    private void OnMouseUp()
    {
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
        var newXPos = Mathf.Clamp(transform.position.x, xData.x, xData.y);
        var newYPos = Mathf.Clamp(transform.position.y, yData.x + _selfSize.y, yData.y);

        _rigidbody2.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        _xBorders = itemPlace.GetComponent<ItemPlace>().GetXBorders();
        _yBorders = itemPlace.GetComponent<ItemPlace>().GetYBorders();
    }

    private void CheckRayCastHit()
    {
        RaycastHit2D[] raycastHit = Physics2D.GetRayIntersectionAll(Camera.main.ScreenPointToRay(Input.mousePosition));
        Debug.Log(raycastHit);
    }
}

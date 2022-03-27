using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeEffect : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _initialPosition;
    private float _distanceMoved;
    private bool _swipeRight;

    private bool _swipe = false;
    private bool _isBeingSwiped = false;


    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = new Vector2(transform.localPosition.x+eventData.delta.x, transform.localPosition.y);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _isBeingSwiped = true;
        _initialPosition = transform.localPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);
        if(_distanceMoved<0.3*Screen.width)
        {
            transform.localPosition = _initialPosition;
        }
        else
        {
            if (transform.localPosition.x > _initialPosition.x){
                _swipeRight = true;
            }
            else
            {
                _swipeRight = false;
            }
            StartCoroutine(MovedCard());
        }
        _isBeingSwiped = false;
    }

    private IEnumerator MovedCard()
    {
        _swipe = true;
        transform.localPosition = _initialPosition;
        yield return null;
    }

    public void unSwipe()
    {
        _swipe = false;
    }

    public bool hasSwiped()
    {
        return _swipe;
    }

    public bool swipeRight()
    {
        return _swipeRight;
    }
}

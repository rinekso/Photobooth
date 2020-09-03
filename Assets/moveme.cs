using UnityEngine;
using UnityEngine.EventSystems;

public class moveme : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = eventData.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}

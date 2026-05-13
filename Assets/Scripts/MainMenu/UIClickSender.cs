using Oculus.Interaction;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIClickSender : MonoBehaviour
{
    [SerializeField] private UnityEvent onClick;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var pointable = GetComponent<IPointable>();

        pointable.WhenPointerEventRaised += OnElementClicked;
    }

    void OnElementClicked(PointerEvent eventData)
    {
        if (eventData.Type == PointerEventType.Select)
        {
            onClick?.Invoke();
        }
    }
}

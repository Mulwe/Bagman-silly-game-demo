using UnityEngine;

public class PopupMessage
{
    //объект который реагирует - Object
    //объект который вызывает - Player

    //animation 

    private GameObject _trigger;
    private GameObject _triggeredObject;

    private float _radius = 0.5f;
    public float Radius => _radius;

    private string _text;
    public string Text => _text;

    public PopupMessage(GameObject trigger, GameObject triggeredObject)
    {
        _trigger = trigger;
        _triggeredObject = triggeredObject;
        if (_trigger != null)
            _text = $"{trigger.name} is near";

    }


    public void SetText(string text)
    {
        _text = text;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;
    }

    public void SetTrigger(GameObject obj)
    {
        _trigger = obj;
    }

    public void SetTriggeredObject(GameObject obj)
    {
        _triggeredObject = obj;
    }

    public void SetTriggerAndObject(GameObject trigger, GameObject obj)
    {
        SetTrigger(trigger);
        SetTriggeredObject(obj);
    }

    public void ActivatePopUp()
    {


    }

    private CircleCollider2D AddCircleCollider(GameObject interactableObj)
    {
        if (interactableObj == null) return null;
        CircleCollider2D collider = interactableObj.AddComponent<CircleCollider2D>();
        if (collider == null) return null;
        collider.isTrigger = true;
        collider.radius = _radius;

        //add script : t

        return collider;
    }
}

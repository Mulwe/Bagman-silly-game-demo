
using UnityEngine;


public class Cart
{
    public GameObject _cart;
    public Rigidbody2D _rb;


    public Cart(GameObject obj)
    {
        _cart = obj;
        if (_rb == null)
        {
            _rb = obj.GetComponent<Rigidbody2D>();
        }
    }

    public Cart(GameObject obj, Rigidbody2D rb)
    {
        _cart = obj;
        _rb = rb;
    }

    public Rigidbody2D GetRigidBody()
    {
        if (_cart != null)
        {
            return (_rb = _cart.GetComponent<Rigidbody2D>() == null ? null : _rb);
        }
        return null;
    }


    public void Dispose()
    {

    }


}

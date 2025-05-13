using UnityEngine;
/*
 *  Interface IMovable
 * */

public interface IPushable
{
    public float Mass { get; set; }
    public float Force { get; set; }

    Vector2 Position { get; set; }
    //public float Speed { get; set; }
    //public float Friction { get; set; }

    void Push(Vector2 direction, float force);
    //direction 
    Vector2 SetDirection(Vector2 direction);

}

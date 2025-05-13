using UnityEngine;

public class DropZoneController : MonoBehaviour
{
    //private System.Random rnd = new System.Random();

    BoxCollider2D DropZoneCollider;

    void Start()
    {
        DropZoneCollider = GetComponent<BoxCollider2D>();
        if (DropZoneCollider != null)
            DropZoneCollider.enabled = true;
        else
            Debug.Log("DropZoneCollider is not found");
    }
    //�������� ���� �� �������
    //
    // ����������, ��������� �����, 
    // ��������������� ��� ���������� ������

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("cart"))
            {
                //�������������� �������
                //���������� ���������, ��������� �����,
                //�������� ������� �� ������� ������
            }
        }
        else
            Debug.Log("Collision is null");
    }



    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.cyan;

        Collider2D col = GetComponent<Collider2D>();
        if (col is BoxCollider2D box)
        {
            Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);

        }
    }
}

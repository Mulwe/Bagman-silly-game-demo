using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Shadoweffect : MonoBehaviour
{
    public ShadowSettings Shadow;
    private Vector3 ShadowScale;

    private GameObject _shadow;

    private void Start()
    {

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (Shadow != null)
        {


            ShadowScale = new Vector3(renderer.sprite.bounds.size.x * Shadow.Width,
                    renderer.sprite.bounds.size.y * Shadow.Height, 1f);

            _shadow = new GameObject("Shadow");
            _shadow.transform.parent = transform;
            _shadow.transform.localPosition = Shadow.Offset;
            _shadow.transform.localRotation = Quaternion.identity;
            _shadow.transform.localScale = ShadowScale;

            SpriteRenderer sr = _shadow.AddComponent<SpriteRenderer>();
            sr.sprite = Shadow.ShadowSprite;

            sr.color = new Color(0f, 0f, 0f, Shadow.ShadowIntensity);

            sr.sortingLayerName = renderer.sortingLayerName;
            sr.sortingOrder = renderer.sortingOrder - 1;


        }
    }

    void LateUpdate()
    {
        if (_shadow != null && Shadow != null)
            _shadow.transform.localPosition = Shadow.Offset;
    }
}

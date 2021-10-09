using UnityEngine;
using DG.Tweening;

public class UnitController : MonoBehaviour
{
    public void MoveLocalPosition(Vector3 localPosition, float duration = 0.1f)
    {
        transform.DOLocalMove(localPosition, duration);
    }
}

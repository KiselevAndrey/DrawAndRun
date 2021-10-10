using UnityEngine;
using DG.Tweening;

public class UnitController : MonoBehaviour
{
    public static System.Action<UnitController> UnitDeath;
    [SerializeField] private Animator animator;

    public void MoveLocalPosition(Vector3 localPosition, float duration = 0.1f)
    {
        transform.DOLocalMove(localPosition, duration);
        animator.SetBool("Run", true);
    }

    public void IsFinised()
    {
        animator.SetTrigger("Finish");
    }
    
    private void OnTriggerEnter(Collider collider)
    {
        switch (collider.tag)
        {
            case "Trap":
                DeathUnit();
                break;
            default:
                break;
        }
    }

    private void DeathUnit()
    {
        UnitDeath?.Invoke(this);
        transform.parent = null;
        Sequence deathSequence = DOTween.Sequence();
        deathSequence.AppendInterval(1f)
            .AppendCallback(()=> KAP.Pool.Pool.Despawn(gameObject));
    }
}

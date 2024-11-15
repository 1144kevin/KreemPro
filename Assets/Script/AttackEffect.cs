using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    public ParticleSystem attackEffect;

    // 這個方法會在動畫事件中觸發
    public void PlayAttackEffect()
    {
        if (attackEffect != null && !attackEffect.isPlaying)
        {
            Debug.Log("Play");
            attackEffect.Play();  // 觸發播放
        }
    }

    // 可選：添加一個停止特效的方法，如果需要在其他地方手動停止
    public void StopAttackEffect()
    {
        if (attackEffect != null && attackEffect.isPlaying)
        {
            Debug.Log("Stop");
            attackEffect.Stop();  // 停止播放
        }
    }
}

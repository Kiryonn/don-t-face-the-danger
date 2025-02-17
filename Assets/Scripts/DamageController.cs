using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DamageController : MonoBehaviour
{
    public float health { get; private set; }
    public float maxHealth;
    Rigidbody rb;
    [SerializeField] AudioClip impactSFX;
    [SerializeField] float maxVolume;
    [SerializeField] float maxImpactMagnitude;
    [SerializeField] SkinnedMeshRenderer[] damageableParts;

    [SerializeField] List<VisualEffect> smokeVFX;
    private void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
        foreach (var item in smokeVFX)
        {
            item.Stop();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //MyDebug.Log("It's colliding with : "+collision.gameObject.tag);
        float v = Mathf.Lerp(0,maxVolume,
            Mathf.Clamp01(rb.velocity.magnitude/ maxImpactMagnitude));
        AudioManager.instance.PlaySFX(impactSFX,v);

        if (collision.gameObject.CompareTag("Hazard"))
        {
            DamageTractor(5f);
        }

        if (collision.gameObject.layer == 7)
        {
            DamageTractor(25f);
        }
    }


    public void DamageTractor(float amount, bool scaleWithSpeed = false)
    {
        if (!scaleWithSpeed)
        {
            health -= amount;
        }
        else
        {
            health -= amount * rb.velocity.magnitude;
        }
        if (health < 0)  health = 0;
        //MyDebug.Log("Endommagement "+rb.velocity.magnitude);

        if (amount < 10)
        {
            GameManager.Instance.GetComponent<TransitionManager>().FadeDamage(0.12f);
        }
        UpdateVisualDamage();
    }

    public void HealTractor(float amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth;
        UpdateVisualDamage();
    }

    void UpdateVisualDamage()
    {
        for (int i = 0; i<damageableParts.Length; i++)
        {
            //MyDebug.Log("Setting blendshape values to "+ (1 - health / maxHealth) * 100);
            damageableParts[i].SetBlendShapeWeight(0, (1 - health / maxHealth) * 100);
            //MyDebug.Log("Blendshape values are : " + damageableParts[i].GetBlendShapeWeight(0));
        }

        switch (health)
        {
            case float n when (n<=25f):
                smokeVFX[0].Play();
                smokeVFX[1].Play();
                break;
            case float n when (n > 25f && n <=50f):
                smokeVFX[0].Play();
                break;
            default:
                smokeVFX[0].Stop();
                smokeVFX[1].Stop();
                break;
        }

        GameManager.Instance.SetPenteScaledWithDmg();
    }
}

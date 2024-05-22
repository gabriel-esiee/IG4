using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlaneExplodeAnimation : MonoBehaviour
{
    [SerializeField] private float explosionForce = 20.0f;
    [SerializeField] private float explosionRadius = 5.0f;
    [SerializeField] private List<Rigidbody> elements;
    [SerializeField] private List<VisualEffect> vfx;

    private bool triggered = false;
    
    public void Trigger(Vector3 explositionPosition)
    {
        if (triggered == true)
            return;
        
        triggered = true;

        foreach (Rigidbody elem in elements)
        {
            elem.gameObject.SetActive(true);
            elem.isKinematic = false;
            elem.AddExplosionForce(explosionForce, explositionPosition, explosionRadius);
        }

        foreach (VisualEffect elem in vfx)
        {
            elem.gameObject.SetActive(true);
        }
    }
}

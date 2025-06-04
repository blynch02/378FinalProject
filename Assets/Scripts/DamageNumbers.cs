using System;
using TMPro;
using Unity.Mathematics;
using Unity.Multiplayer.Center.Common;
using UnityEngine;

public class DamageNumbers : MonoBehaviour
{
    private TextMeshPro mesh;
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeDuration = 2f;


    private void Awake()
    {
        mesh = transform.GetComponent<TextMeshPro>();

    }
    public void setUp(int damageAmount)
    {
        if (damageAmount == 0)
        {
            mesh.SetText("Miss!");
            mesh.color= new Color32(171, 5, 16, 255);
            StartCoroutine(Fade());
            return;
        }
        if (damageAmount < 0)
        {
            mesh.color = new Color32(10, 204, 68, 255);
            Debug.Log(mesh.color);
        }
        else
        {
            mesh.color = new Color32(171, 5, 16, 255);
        }
        int amount = Mathf.Abs(damageAmount);
        mesh.SetText(amount.ToString());

        StartCoroutine(Fade());
    }

    public void setUpStatusEffect(string effect)
    {
        mesh.SetText(effect);
        mesh.color = new Color32(171, 5, 16, 255);
        StartCoroutine(Fade());
    }

    private System.Collections.IEnumerator Fade()
    {
        float time = 0f;
        Color originalColor = mesh.color;
        Vector3 start = transform.position;
        while (time < fadeDuration)
        {
            transform.position = start + Vector3.up * (time * floatSpeed);
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            mesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}

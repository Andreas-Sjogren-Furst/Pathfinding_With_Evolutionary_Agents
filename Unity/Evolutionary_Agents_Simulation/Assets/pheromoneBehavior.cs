using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pheromoneBehavior : MonoBehaviour
{

    // Time in seconds before the object is destroyed
    public float evaporationConstant = 5f;
    public float alpha;

    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeOutRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator FadeOutRoutine()
    {
        Material mat = GetComponent<Renderer>().material;
        float elapsedTime = 0;

        Color startColor = mat.color;

        while (elapsedTime < evaporationConstant)
        {
            elapsedTime += Time.deltaTime;
            alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / evaporationConstant);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}

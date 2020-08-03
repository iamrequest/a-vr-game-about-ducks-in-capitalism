using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour {
    public Transform mainLightTransform;
    public List<Material> materials;

    private void FixedUpdate() {
        Vector4 lightPos = new Vector4(mainLightTransform.position.x, 
            mainLightTransform.position.y,
            mainLightTransform.position.z);

        foreach (Material m in materials) {
            // TODO: Cache this
            if (m.HasProperty("lightPosition")) {
                m.SetVector("lightPosition", lightPos);
            } else {
                Debug.LogError("Missing LightPosition property on this material: " + m.name);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPreview : MonoBehaviour
{
    public Texture Texture => tex;

    Camera cam;
    RenderTexture tex;

    GameObject currentCharacterPrefab;
    GameObject characterInstance;

    float rotationSpeed;

    public void Init(Color backgroundColor, int textureSize, float rotateSpeed)
    {
        rotationSpeed = rotateSpeed;

        cam = GetComponentInChildren<Camera>(true);
        cam.backgroundColor = backgroundColor;

        tex = new RenderTexture(new RenderTextureDescriptor(textureSize, textureSize));
        cam.targetTexture = tex;
    }

    public RenderTexture GetTexture() => tex;

    void FixedUpdate()
    {
        if (!characterInstance)
            return;

        float y = characterInstance.transform.eulerAngles.y;
        transform.GetChild(1).eulerAngles = Vector3.up * y;
        characterInstance.transform.eulerAngles = Vector3.up * (y + rotationSpeed);
    }

    public void SetCharacter(GameObject characterPrefab)
    {
        if (currentCharacterPrefab == characterPrefab)
            return;


        currentCharacterPrefab = characterPrefab;
        if (characterInstance)
            Destroy(characterInstance);

        if (characterPrefab == null)
            return;
        characterInstance = Instantiate(characterPrefab);
        characterInstance.transform.SetParent(transform, false);
        characterInstance.transform.localPosition = Vector3.zero;
    }
}

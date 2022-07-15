using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPreviewManager : MonoBehaviour
{
    [Header("references")]
    public GameObject[] selectingPreviews;
    public Transform confirmationParent;
    public GameObject previewPrefab;

    [Header("preview object placement")]
    public Vector3 basePosition;
    public Vector3 offset;

    [Header("settings")]
    public float backgroundSaturation;
    public int maxPreviews = 4;
    public float confirmationBorderSize = 150;
    public Sprite confirmationBorderSprite;
    public float confirmationSize = 150;
    public int previewTextureSize = 600;

    public float previewRotationSpeed;

    CharacterPreview[] characterPreviews;
    GameObject[] confirmationPreviews;

    Transform previewParent;


    void Start()
    {
    }

    void OnEnable()
    {
        if (previewParent)
            Destroy(previewParent);

        previewParent = new GameObject("character previews").transform;
        previewParent.SetParent(transform, false);
        previewParent.transform.position = basePosition;

        characterPreviews = new CharacterPreview[maxPreviews];

        for (int i = 0; i < maxPreviews; i++)
        {
            GameObject previewInstance = Instantiate(previewPrefab);
            previewInstance.transform.SetParent(previewParent, false);
            previewInstance.transform.localPosition = i * offset;
            characterPreviews[i] = previewInstance.GetComponent<CharacterPreview>();

            Color bgColour = MenuSelections.GetColor(i);
            float h, s, v;
            Color.RGBToHSV(bgColour, out h, out s, out v);
            bgColour = Color.HSVToRGB(h, backgroundSaturation, v);
            characterPreviews[i].Init(bgColour, previewTextureSize, previewRotationSpeed);
        }

        confirmationPreviews = new GameObject[InputProxy.playerCount];
        for(int i = 0; i < InputProxy.playerCount; i++)
        {
            selectingPreviews[i].GetComponentInChildren<RawImage>(true)
                .texture = characterPreviews[i].Texture;

            GameObject confirmationBorder = new GameObject("P" + i + " border");
            confirmationBorder.transform.SetParent(confirmationParent, false);
            Image border = confirmationBorder.AddComponent<Image>();
            border.rectTransform.sizeDelta = Vector2.one * confirmationBorderSize;
            border.sprite = confirmationBorderSprite;

            GameObject previewInstance = new GameObject("P" + i);
            previewInstance.transform.SetParent(confirmationBorder.transform, false);
            RawImage im = previewInstance.AddComponent<RawImage>();
            im.rectTransform.sizeDelta = Vector2.one * confirmationSize;
            im.texture = characterPreviews[i].Texture;

            confirmationPreviews[i] = confirmationBorder;
        }
        OnConfirmationStart();
        OnReject();
    }

    public void OnCharacterSelected(int playerIndex, NodeElement selectedNode)
    {
        characterPreviews[playerIndex].SetCharacter(selectedNode.PrefabPayload);
    }

    public void OnConfirmationStart()
    {
        transform.SetAsLastSibling();

        foreach (GameObject o in selectingPreviews)
            o.SetActive(false);
        foreach (GameObject o in confirmationPreviews)
            o.SetActive(true);
    }

    public void OnReject()
    {
        for (int i = 0; i < InputProxy.playerCount; i++)
            selectingPreviews[i].SetActive(true);
        foreach (GameObject o in confirmationPreviews)
            o.SetActive(false);
    }
}

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

    [Header("preview settings")]
    public float backgroundSaturation;
    public int maxPreviews = 4;
    public int previewTextureSize = 600;
    public float previewRotationSpeed;

    [Header("Team Materials")]
    public Material[] teamMaterials;

    [Header("confirmation preview settings")]
    public float confirmationBorderSize = 150;
    public Sprite confirmationBorderSprite;
    public float confirmationSize = 150;

    [Header("confirmation indicator settings")]
    public Sprite indicatorSprite;
    public Vector2 indicatorOffset;
    public float indicatorSize;


    CharacterPreview[] characterPreviews;
    GameObject[] confirmationPreviews;
    Image[] confirmationIndicators;

    Transform previewParent;

    int[] playerTeams;

    void OnEnable()
    {
        playerTeams = new int[InputProxy.playerCount];
        for(int i = 0; i < playerTeams.Length; i++)
            for (int j = 0; j < MenuSelections.teams.Count; j++)
                if (MenuSelections.teams[j].Contains(i))
                    playerTeams[i] = j;

        if (previewParent)
            Destroy(previewParent.gameObject);

        previewParent = new GameObject("character previews").transform;
        previewParent.SetParent(transform, false);
        previewParent.transform.position = basePosition;

        foreach (Transform child in confirmationParent)
            Destroy(child.gameObject);

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
        confirmationIndicators = new Image[InputProxy.playerCount];
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

            GameObject indicatorInstance = new GameObject("indicator");
            indicatorInstance.transform.SetParent(confirmationBorder.transform, false);
            indicatorInstance.transform.localPosition = indicatorOffset;
            Image ind = indicatorInstance.AddComponent<Image>();
            ind.sprite = indicatorSprite;
            ind.rectTransform.sizeDelta = Vector2.one * indicatorSize;
            ind.enabled = false;

            confirmationIndicators[i] = ind;
        }
        OnConfirmationStart();
        OnReject(-1);
    }

    public void OnCharacterSelected(int playerIndex, NodeElement selectedNode)
    {
        characterPreviews[playerIndex].SetCharacter(selectedNode.PrefabPayload);
        characterPreviews[playerIndex].SetTeamMaterial(teamMaterials[playerTeams[playerIndex]]);
    }

    public void OnConfirmationStart()
    {
        //transform.SetAsLastSibling();

        foreach (GameObject o in selectingPreviews)
            o.SetActive(false);
        foreach (GameObject o in confirmationPreviews)
            o.SetActive(true);
    }

    public void OnReject(int playerIndex)
    {
        for (int i = 0; i < InputProxy.playerCount; i++)
            selectingPreviews[i].SetActive(true);
        foreach (GameObject o in confirmationPreviews)
            o.SetActive(false);

        if (playerIndex >= 0)
            characterPreviews[playerIndex].SetCharacter(null);

        foreach (Image i in confirmationIndicators)
            i.enabled = false;
    }

    public void OnConfirm(int playerIndex)
    {
        confirmationIndicators[playerIndex].enabled = true;
    }
}

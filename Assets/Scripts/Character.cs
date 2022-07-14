using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

/// <summary>
/// Holds placement settings and animation functionality for a generic playable character
/// </summary>
public class Character : MonoBehaviour
{
    [Header("Placements")]
    public Placement throwingPlacement;
    public Placement brushingWaitPlacement;
    public Placement brushingPlacement;
    public Placement resultPlacement;
    public Placement podiumPlacement;
    public Placement previewPlacement;

    [Header("Team Colour reference")]
    public SkinnedMeshRenderer teamColourMesh;
    public int teamMaterialIndex;

    [Header("Broom Bones")]
    public GameObject broomHeadBone;
    public GameObject broomstickBone;

    [Header("Misc settings")]
    public bool hideBroomOnThrow = true;
    public bool hideBroomOnIntro;
    public bool hideBroomOnPodium;

    private Animator animator;
    public float BrushSpeed { set { animator.SetFloat("brush speed", value); } }

    private MeshRenderer broomHead;
    private MeshFilter broomHeadMeshFilter;
    private MeshRenderer broomstick;
    private MeshFilter broomstickMeshFilter;


    public void Init()
    {
        animator = GetComponentInChildren<Animator>(true);
        ApplyRandomOffsetToAnimator();

        var broomHeadObject = new GameObject("mesh");
        broomHeadObject.transform.SetParent(broomHeadBone.transform, false);
        broomHeadObject.transform.localScale = Vector3.one / 100;
        broomHead = broomHeadObject.AddComponent<MeshRenderer>();
        broomHeadMeshFilter = broomHeadObject.AddComponent<MeshFilter>();

        var broomstickObject = new GameObject("mesh");
        broomstickObject.transform.localScale = Vector3.one / 100;
        broomstickObject.transform.SetParent(broomstickBone.transform, false);
        broomstick = broomstickObject.AddComponent<MeshRenderer>();
        broomstickMeshFilter = broomstickObject.AddComponent<MeshFilter>();
    }

    public void SetBrushMeshes(Mesh head, Material[] headMaterials, Mesh stick, Material[] stickMaterials)
    {
        broomHeadMeshFilter.mesh = head;
        broomHead.materials = headMaterials;

        broomstickMeshFilter.mesh = stick;
        broomstick.materials = stickMaterials;
    }

    public void SetTeamMaterial(Material teamMaterial)
    {
        Material[] mats = teamColourMesh.materials;
        mats[teamMaterialIndex] = teamMaterial;
        teamColourMesh.materials = mats;
    }

    public void SetUpBrushing()
    {
        Show();
        brushingWaitPlacement.Apply(transform);
        animator.SetTrigger("brushing");
        ShowBroom();
    }

    public void SetUpThrowing()
    {
        Show();
        throwingPlacement.Apply(transform);
        animator.SetTrigger("throwing");
        if (hideBroomOnThrow)
            HideBroom();
    }

    public void OnThrow() => animator.SetTrigger("throw");
    public void OnResult() => animator.SetTrigger("brushing");
    public void OnWin() => OnResult("win");
    public void OnLose() => OnResult("lose");
    void OnResult(string trigger)
    {
        Show();
        if (hideBroomOnPodium)
            HideBroom();
        else
            ShowBroom();

        animator.SetTrigger(trigger);
        ApplyRandomOffsetToAnimator();
    }

    public void Hide() => gameObject.SetActive(false);
    public void Show() => gameObject.SetActive(true);

    public void ApplyRandomOffsetToAnimator() => animator.Update(Random.Range(0, 10f));

    public void HideBroom()
    {
        broomHead.enabled = false;
        broomstick.enabled = false;
    }
    public void ShowBroom()
    {
        broomHead.enabled = true;
        broomstick.enabled = true;
    }

    public void MoveToRock(Vector3 rockPosition, float lerp)
    {
        brushingPlacement.LerpTo(transform, rockPosition, lerp);
    }
    public void MoveToResult(float lerp) => resultPlacement.LerpTo(transform, lerp);
}

[System.Serializable]
public class Placement
{
    public Vector3 position;
    public float rotation;

    public void Apply(Transform t)
    {
        t.localPosition = position;
        t.localEulerAngles = Vector3.up * rotation;
    }

    public void LerpTo(Transform t, float posLerp = 0.05f, float rotLerp = 0.1f)
        => LerpTo(t, Vector3.zero, posLerp, rotLerp);
    public void LerpTo(Transform t, Vector3 positionOffset,
        float posLerp = 0.05f, float rotLerp = 0.1f)
    {
        t.localPosition = Vector3.Lerp(t.localPosition, position + positionOffset, posLerp);
        t.localEulerAngles = Vector3.up * Mathf.Lerp(LoopAngle(t.localEulerAngles.y), rotation, rotLerp);
    }

    private float LoopAngle(float angle, float centre = 0)
    {
        if (angle > centre + 180)
            return LoopAngle(angle - 360, centre);
        else if (angle < centre - 180)
            return LoopAngle(angle + 360, centre);
        return angle;
    }
}

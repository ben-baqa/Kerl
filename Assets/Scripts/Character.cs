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

    [Header("Team Colour reference")]
    public SkinnedMeshRenderer teamColourMesh;
    public int teamMaterialIndex;

    [Header("Animator references")]
    public AnimatorController animationController;
    public GameObject animationTarget;

    [Header("Broom Bones")]
    public GameObject broomHeadBone;
    public GameObject broomstickBone;

    private Animator animator;
    public float BrushSpeed { set { animator.SetFloat("brush speed", value); } }

    private MeshRenderer broomHead;
    private MeshRenderer broomstick;


    public void Init()
    {
        animator = animationTarget.AddComponent<Animator>();
        animator.runtimeAnimatorController = animationController;

        broomHead = broomHeadBone.AddComponent<MeshRenderer>();
        broomstick = broomstickBone.AddComponent<MeshRenderer>();
    }

    public void SetBrushMeshes(Mesh head, Material[] headMaterials, Mesh stick, Material[] stickMaterials)
    {
        broomHead.GetComponent<MeshFilter>().mesh = head;
        broomHead.materials = headMaterials;

        broomstick.GetComponent<MeshFilter>().mesh = stick;
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
        brushingWaitPlacement.Apply(transform);
        animator.SetTrigger("brushing");
    }

    public void SetUpThrowing()
    {
        throwingPlacement.Apply(transform);
        animator.SetTrigger("throwing");
    }

    public void OnThrow() => animator.SetTrigger("throw");
    public void OnWin() => animator.SetTrigger("win");
    public void OnLose() => animator.SetTrigger("lose");

    public void Hide() => gameObject.SetActive(false);
    public void Unhide() => gameObject.SetActive(true);


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
    }
}

using UnityEngine;

public class FolderController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        spriteRenderer.material.SetFloat("_Toggle", 1.0f);
    }

    private void OnMouseExit()
    {
        spriteRenderer.material.SetFloat("_Toggle", 0f);
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance != null && GameManager.Instance.optionsMenuOpen == true) return;
        GameManager.Instance.OpenCaseFile();
    }

}
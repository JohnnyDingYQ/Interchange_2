using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [SerializeField]
    Material idle;
    [SerializeField]
    Material hovered;
    Build build;
    public uint RoadId;

    void Awake()
    {
        GetComponent<Renderer>().material = idle;
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material = hovered;
        build.SelectedSnap = this;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material = idle;
        build.SelectedSnap = null;
    }

    public void Initialize(uint roadId, Build build)
    {
        RoadId = roadId;
        this.build = build;
    }
}

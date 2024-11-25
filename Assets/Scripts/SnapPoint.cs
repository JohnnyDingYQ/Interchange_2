using UnityEngine;

public class SnapPoint : MonoBehaviour
{
    [SerializeField]
    Material idle;
    [SerializeField]
    Material hovered;
    Build build;
    public uint RoadId;
    public int LaneIndex;

    void Awake()
    {
        GetComponent<Renderer>().material = idle;
    }

    void OnMouseOver()
    {
        GetComponent<Renderer>().material = hovered;
        build.HoveredSnap = this;
    }

    void OnMouseExit()
    {
        GetComponent<Renderer>().material = idle;
        build.HoveredSnap = null;
    }

    public void Initialize(uint roadId, int laneIndex, Build build)
    {
        RoadId = roadId;
        this.build = build;
        LaneIndex = laneIndex;
        gameObject.name = $"Road {roadId} Lane {laneIndex}";
    }
}

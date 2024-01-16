using UnityEngine;
using Cinemachine;

public class SwitchBounds : MonoBehaviour
{
    // TODO: 后续切换场景需要修改
    private void Start()
    {
        SwitchConfinerShape();
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();
        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();
        confiner.m_BoundingShape2D = confinerShape;
        // Call this if the bounding shape's points change at runtime
        confiner.InvalidatePathCache();  
    }
}

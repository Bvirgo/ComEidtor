using UnityEngine;
using System.Collections;

public interface IMouseCtrl
{
    void MouseDown(Vector2 screenPoint, Vector3 worldPoint, int mouse);
    void MouseUp(Vector2 screenPoint, Vector3 worldPoint, int mouse);
    void MouseClick(Vector2 screenPoint, Vector3 worldPoint, int mouse);
    void MouseDoubleClick(Vector2 screenPoint, Vector3 worldPoint, int mouse);

    void OnDragStart(Vector2 screenPoint, Vector3 worldPoint, int mouse);
    void OnDraging(Vector2 screenPoint, Vector3 worldPoint, int mouse);
    void OnDragEnd(Vector2 screenPoint, Vector3 worldPoint, int mouse);

}

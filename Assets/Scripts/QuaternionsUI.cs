using UnityEngine;
using UnityEngine.UIElements;

public class QuaternionsUI : MonoBehaviour
{
    private UIDocument document;
    private Label leftEuler;
    private Label leftQuaternion;
    private Label rightEuler;
    private Label rightQuaternion;

    private const string eulerFormat = "<color=#f14121ff>X: {0:F0}°</color>\n" +
                                       "<color=#98f145ff>Y: {1:F0}°</color>\n" +
                                       "<color=#3d80f1ff>Z: {2:F0}°</color>";
    private const string quaternionFormat = "<color=#f14121ff>QX: {0:F2}</color>\n" +
                                            "<color=#98f145ff>QY: {1:F2}</color>\n" +
                                            "<color=#3d80f1ff>QZ: {2:F2}</color>\n" +
                                            "QW: {3:F2}";

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        leftEuler = document.rootVisualElement.Q<Label>("LeftEuler");
        leftQuaternion = document.rootVisualElement.Q<Label>("LeftQuaternion");
        rightEuler = document.rootVisualElement.Q<Label>("RightEuler");
        rightQuaternion = document.rootVisualElement.Q<Label>("RightQuaternion");
    }

    public void SetLeftEuler(Vector3 value)
    {
        leftEuler.text = string.Format(eulerFormat, value.x, value.y, value.z);
    }

    public void SetLeftQuaternion(Quaternion value)
    {
        leftQuaternion.text = string.Format(quaternionFormat, value.x, value.y, value.z, value.w);
    }

    public void SetRightEuler(Vector3 value)
    {
        rightEuler.text = string.Format(eulerFormat, value.x, value.y, value.z);
    }

    public void SetRightQuaternion(Quaternion value)
    {
        rightQuaternion.text = string.Format(quaternionFormat, value.x, value.y, value.z, value.w);
    }
}

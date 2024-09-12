using UnityEngine;
using UnityEngine.UIElements;

public class GimbalLockUI : MonoBehaviour
{
    private UIDocument document;
    private Label ad;
    private Label sw;
    private Label qe;
    private Label rotation;
    private Label commentary;
    private Label reset;
    private Label autoRotation;
    private Label rotationType;

    public const string xColor = "<color=#f14121ff>";
    public const string yColor = "<color=#98f145ff>";
    public const string zColor = "<color=#3d80f1ff>";
    public const string eulerColor = xColor;
    public const string quaternionColor = yColor;

    private const string eulerFormat = xColor + "X: {0:F0}° {1}</color>\n" +
                                       yColor + "Y: {2:F0}° {3}</color>\n" +
                                       zColor + "Z: {4:F0}° {5}</color>";

    private void Awake()
    {
        document = GetComponent<UIDocument>();
        ad = document.rootVisualElement.Q<Label>("AD");
        sw = document.rootVisualElement.Q<Label>("SW");
        qe = document.rootVisualElement.Q<Label>("QE");
        rotation = document.rootVisualElement.Q<Label>("Rotation");
        commentary = document.rootVisualElement.Q<Label>("Commentary");
        reset = document.rootVisualElement.Q<Label>("Reset");
        autoRotation = document.rootVisualElement.Q<Label>("AutoRotation");
        rotationType = document.rootVisualElement.Q<Label>("RotationType");
    }

    public void SetEuler(Vector3 value, string arrowX, string arrowY, string arrowZ)
    {
        rotation.text = string.Format(eulerFormat, value.x, arrowX, value.y, arrowY, value.z, arrowZ);
    }

    public void SetCommentary(string value)
    {
        commentary.text = value;
    }

    public void SetControlsVisible(bool value)
    {
        ad.visible = value;
        sw.visible = value;
        qe.visible = value;
        reset.visible = value;
    }

    public void SetAutoRotation(bool value)
    {
        if (value)
        {
            autoRotation.text = "Tab — Выкл. автовращение";
        }
        else
        {
            autoRotation.text = "Tab — Вкл. автовращение";
        }
    }

    public void SetAutoRotationVisible(bool value)
    {
        autoRotation.visible = value;
    }

    public void SetUseQuaternions(bool value)
    {
        if (value)
        {
            rotationType.text = "Shift —\nИспользовать " + eulerColor + "углы Эйлера</color>";
        }
        else
        {
            rotationType.text = "Shift —\nИспользовать " + quaternionColor + "кватернионы</color>";
        }
    }

    public void SetUseQuaternionsVisible(bool value)
    {
        rotationType.visible = value;
    }
}

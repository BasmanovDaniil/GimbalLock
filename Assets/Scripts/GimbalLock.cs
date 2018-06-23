using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GimbalLock : MonoBehaviour
{
    [Header("Gimbals")]
    public Transform gimbalX;
    public Transform gimbalY;
    public Transform gimbalZ;
    [Header("UI")]
    public Text xText;
    public Text yText;
    public Text zText;
    public Text shift;
    public Text tab;
    public Text comment;
    public GameObject controls;
    [Header("Audio")]
    public AudioSource click;
    public AudioSource tick;

    private const string xColor = "<color=#f14121ff>";
    private const string yColor = "<color=#98f145ff>";
    private const string zColor = "<color=#3d80f1ff>";
    private const string xTextFormat = xColor + "X: {0:F0}° {1}</color>";
    private const string yTextFormat = yColor + "Y: {0:F0}° {1}</color>";
    private const string zTextFormat = zColor + "Z: {0:F0}° {1}</color>";
    private const string eulerColor = xColor;
    private const string quaternionColor = yColor;

    private float oldX;
    private float oldY;
    private float oldZ;
    private string xArrow;
    private string yArrow;
    private string zArrow;
    private bool useQuaternions;
    private bool auto;
    private bool clicked;
    private bool reset;

    private int screenshotCount;
    private bool onAir;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) SwitchUseQuaternions();
        if (Input.GetKeyDown(KeyCode.Tab)) SwitchAuto();
        if (Input.GetKeyDown(KeyCode.Space)) ResetRotation();

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) StartCoroutine(Demo1());
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) StartCoroutine(Demo2());
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) StartCoroutine(Demo3());
        if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            if (onAir)
            {
                ResetComment();
                onAir = false;
            }
            else
            {
                StartCoroutine(RotateTo(
                    x: gimbalX.localEulerAngles.x + 180,
                    y: gimbalY.localEulerAngles.y + 180,
                    z: gimbalZ.localEulerAngles.z + 180));
                comment.text = ";)";
                onAir = true;
                StartCoroutine(Capture(0.3f));
            }
        }

        if (useQuaternions)
        {
            if (Input.GetKey(KeyCode.D) || auto) gimbalX.localRotation *= Quaternion.Euler(+1, 0, 0);
            if (Input.GetKey(KeyCode.A)) gimbalX.localRotation *= Quaternion.Euler(-1, 0, 0);
            if (Input.GetKey(KeyCode.W) || auto) gimbalY.localRotation *= Quaternion.Euler(0, +1, 0);
            if (Input.GetKey(KeyCode.S)) gimbalY.localRotation *= Quaternion.Euler(0, -1, 0);
            if (Input.GetKey(KeyCode.E) || auto) gimbalZ.localRotation *= Quaternion.Euler(0, 0, +1);
            if (Input.GetKey(KeyCode.Q)) gimbalZ.localRotation *= Quaternion.Euler(0, 0, -1);
        }
        else
        {
            if (Input.GetKey(KeyCode.D) || auto) gimbalX.localEulerAngles += new Vector3(+1, 0, 0);
            if (Input.GetKey(KeyCode.A)) gimbalX.localEulerAngles += new Vector3(-1, 0, 0);
            if (Input.GetKey(KeyCode.W) || auto) gimbalY.localEulerAngles += new Vector3(0, +1, 0);
            if (Input.GetKey(KeyCode.S)) gimbalY.localEulerAngles += new Vector3(0, -1, 0);
            if (Input.GetKey(KeyCode.E) || auto) gimbalZ.localEulerAngles += new Vector3(0, 0, +1);
            if (Input.GetKey(KeyCode.Q)) gimbalZ.localEulerAngles += new Vector3(0, 0, -1);

            if (!clicked && (gimbalX.localEulerAngles.x == 90 || gimbalX.localEulerAngles.x == 270))
            {
                click.Play();
                clicked = true;
            }
        }

        if (clicked && gimbalX.localEulerAngles.x != 90 && gimbalX.localEulerAngles.x != 270)
        {
            tick.Play();
            clicked = false;
        }

        UpdateArrow(gimbalX.localEulerAngles.x, oldX, ref xArrow);
        UpdateArrow(gimbalY.localEulerAngles.y, oldY, ref yArrow);
        UpdateArrow(gimbalZ.localEulerAngles.z, oldZ, ref zArrow);

        oldX = gimbalX.localEulerAngles.x;
        oldY = gimbalY.localEulerAngles.y;
        oldZ = gimbalZ.localEulerAngles.z;
        xText.text = string.Format(xTextFormat, gimbalX.localEulerAngles.x, xArrow);
        yText.text = string.Format(yTextFormat, gimbalY.localEulerAngles.y, yArrow);
        zText.text = string.Format(zTextFormat, gimbalZ.localEulerAngles.z, zArrow);
    }

    private IEnumerator Capture(float pause)
    {
        while (onAir)
        {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/Screenshot" + screenshotCount + ".png");
            screenshotCount++;
            yield return new WaitForSeconds(pause);
        }
    }

    private IEnumerator Demo1()
    {
        comment.text = "";
        controls.SetActive(false);
        tab.gameObject.SetActive(false);
        shift.gameObject.SetActive(false);
        StartCoroutine(RotateTo(90, 0, 0));
        while (true)
        {
            if (reset)
            {
                auto = false;
                useQuaternions = false;
                comment.text = "Выравнивание рамки " + xColor + "X</color>\nи рамки " + zColor +
                               "Z</color> приводит\nк потере подвижности.\nВращение вокруг\nоси " + yColor +
                               "Y</color> и оси " + zColor + "Z</color>\nприводит к\nодинаковому результату";
                StartCoroutine(RotateY(720, 1, wait: 3));
                StartCoroutine(RotateZ(720, 1, wait: 3));
                Invoke("ResetRotation", 16);
                Invoke("ResetComment", 16);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    private IEnumerator Demo2()
    {
        comment.text = "";
        controls.SetActive(false);
        tab.gameObject.SetActive(false);
        shift.gameObject.SetActive(false);
        StartCoroutine(RotateTo(90, 0, 0));
        while (true)
        {
            if (reset)
            {
                auto = false;
                useQuaternions = false;
                comment.text = "Из-за выравнивания рамок\nв данном положении\nневозможен крен";
                StartCoroutine(RotateGimbals(45, 1, wait: 3));
                StartCoroutine(RotateGimbals(90, -1, wait: 5));
                StartCoroutine(RotateGimbals(90, 1, wait: 7));
                StartCoroutine(RotateGimbals(45, -1, wait: 9));
                Invoke("ResetRotation", 11);
                Invoke("ResetComment", 11);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    private IEnumerator Demo3()
    {
        comment.text = "";
        controls.SetActive(false);
        tab.gameObject.SetActive(false);
        StartCoroutine(RotateTo(90, 0, 0));
        while (true)
        {
            if (reset)
            {
                auto = false;
                useQuaternions = false;
                comment.text = "Внутренний блок в Unity3d.\nВращение вокруг\nоси " + xColor +
                               "X</color> застопоривается\nв положении 90° и -90°\nпри использовании\n" + eulerColor +
                               "углов Эйлера</color>\nно работает\n с " + quaternionColor + "кватернионами</color>";
                Invoke("SwitchAuto", 3);
                Invoke("SwitchUseQuaternions", 10);
                Invoke("SwitchUseQuaternions", 18);
                Invoke("SwitchUseQuaternions", 26);
                Invoke("ResetComment", 34);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    private void SwitchAuto()
    {
        if (auto)
        {
            auto = false;
            tab.text = "Tab — Вкл. автовращение";
        }
        else
        {
            auto = true;
            tab.text = "Tab — Выкл. автовращение";
        }
    }

    private void SwitchUseQuaternions()
    {
        if (useQuaternions)
        {
            useQuaternions = false;
            shift.text = "Shift —\nИспользовать " + quaternionColor + "кватернионы</color>";
        }
        else
        {
            useQuaternions = true;
            shift.text = "Shift —\nИспользовать " + eulerColor + "углы Эйлера</color>";
        }
    }

    private IEnumerator RotateY(int count, int speed = 1, float wait = 0)
    {
        yield return StartCoroutine(RotateAxis(count, gimbalY, 1, speed, wait));
    }

    private IEnumerator RotateZ(int count, int speed = 1, float wait = 0)
    {
        yield return StartCoroutine(RotateAxis(count, gimbalZ, 2, speed, wait));
    }

    private IEnumerator RotateGimbals(int count, int speed, float wait = 0)
    {
        yield return StartCoroutine(RotateAxis(count, gimbalZ, 1, speed, wait));
    }

    private IEnumerator RotateAxis(int count, Transform gimbal, int axis, int speed = 1, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        var rotation = new Vector3();
        rotation[axis] = speed;
        for (var i = 0; i < count; i++)
        {
            if (useQuaternions)
            {
                gimbal.localRotation *= Quaternion.Euler(rotation);
                yield return null;
            }
            else
            {
                gimbal.localEulerAngles += rotation;
                yield return null;
            }
        }
    }

    private IEnumerator RotateTo(float x, float y, float z)
    {
        reset = false;
        if (!useQuaternions) SwitchUseQuaternions();
        if (auto) SwitchAuto();
        var quatX = Quaternion.Euler(x, 0, 0);
        var quatY = Quaternion.Euler(0, y, 0);
        var quatZ = Quaternion.Euler(0, 0, z);
        while (true)
        {
            if (gimbalX.localRotation != quatX)
            {
                gimbalX.localRotation = Quaternion.RotateTowards(gimbalX.localRotation, quatX, Time.deltaTime*60);
            }
            if (gimbalY.localRotation != quatY)
            {
                gimbalY.localRotation = Quaternion.RotateTowards(gimbalY.localRotation, quatY, Time.deltaTime*60);
            }
            if (gimbalZ.localRotation != quatZ)
            {
                gimbalZ.localRotation = Quaternion.RotateTowards(gimbalZ.localRotation, quatZ, Time.deltaTime*60);
            }
            if (gimbalZ.localRotation == quatZ && gimbalY.localRotation == quatY && gimbalX.localRotation == quatX)
            {
                reset = true;
                break;
            }
            yield return null;
        }
        yield return null;
    }

    private void ResetRotation()
    {
        StartCoroutine(RotateTo(0, 0, 0));
    }

    private void ResetComment()
    {
        comment.text = "1, 2, 3 —\nПереключение\nдемонстраций\nс комментариями";
        controls.SetActive(true);
        tab.gameObject.SetActive(true);
        shift.gameObject.SetActive(true);
    }

    private void UpdateArrow(float angle, float oldAngle, ref string axisArrow)
    {
        if (angle > oldAngle)
        {
            axisArrow = "↑";
        }
        else if (angle < oldAngle)
        {
            axisArrow = "↓";
        }
    }
}

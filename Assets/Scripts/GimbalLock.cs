using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GimbalLock : MonoBehaviour
{
    public Transform gimbalX;
    public Transform gimbalY;
    public Transform gimbalZ;
    public Text xText;
    public Text yText;
    public Text zText;
    public Text shift;
    public Text tab;
    public Text comment;
    public GameObject controls;
    public AudioSource click;
    public AudioSource tick;

    private const string xColor = "<color=#f14121ff>";
    private const string yColor = "<color=#98f145ff>";
    private const string zColor = "<color=#3d80f1ff>";
    private const string eulerColor = xColor;
    private const string quaternionColor = yColor;

    private float oldX;
    private float oldY;
    private float oldZ;
    private string xArrow;
    private string yArrow;
    private string zArrow;
    private bool quaternions;
    private bool auto;
    private bool clicked;
    private bool reset;

    private int screenshotCount;
    private bool onAir;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) SwitchQuaternions();
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
                    X: gimbalX.localEulerAngles.x + 180,
                    Y: gimbalY.localEulerAngles.y + 180,
                    Z: gimbalZ.localEulerAngles.z + 180));
                comment.text = ";)";
                onAir = true;
                StartCoroutine(Capture(0.3f));
            }
        }

        if (quaternions)
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

        if (gimbalX.localEulerAngles.x > oldX) xArrow = "↑";
        else if (gimbalX.localEulerAngles.x < oldX) xArrow = "↓";
        if (gimbalY.localEulerAngles.y > oldY) yArrow = "↑";
        else if (gimbalY.localEulerAngles.y < oldY) yArrow = "↓";
        if (gimbalZ.localEulerAngles.z > oldZ) zArrow = "↑";
        else if (gimbalZ.localEulerAngles.z < oldZ) zArrow = "↓";
        oldX = gimbalX.localEulerAngles.x;
        oldY = gimbalY.localEulerAngles.y;
        oldZ = gimbalZ.localEulerAngles.z;
        xText.text = xColor + "X: " + gimbalX.localEulerAngles.x.ToString("F0") + "° " + xArrow + "</color>";
        yText.text = yColor + "Y: " + gimbalY.localEulerAngles.y.ToString("F0") + "° " + yArrow + "</color>";
        zText.text = zColor + "Z: " + gimbalZ.localEulerAngles.z.ToString("F0") + "° " + zArrow + "</color>";
    }

    IEnumerator Capture(float pause)
    {
        while (onAir)
        {
            Application.CaptureScreenshot(Application.dataPath + "/Screenshot" + screenshotCount + ".png");
            screenshotCount++;
            yield return new WaitForSeconds(pause);
        }
    }

    IEnumerator Demo1()
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
                quaternions = false;
                comment.text = "Выравнивание рамки " + yColor + "Y</color>\nи рамки " + zColor +
                    "Z</color> приводит\nк потере подвижности.\nВращение вокруг\nоси " + xColor +
                    "X</color> и оси " + zColor + "Z</color>\nприводит к\nодинаковому результату";
                StartCoroutine(RotateY(720, 1, 3));
                StartCoroutine(RotateZ(720, 1, 3));
                Invoke("ResetRotation", 16);
                Invoke("ResetComment", 16);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator Demo2()
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
                quaternions = false;
                comment.text = "Из-за выравнивания рамок\nв данном положении\nневозможен крен";
                StartCoroutine(RotateGimbals(45, 1, 3));
                StartCoroutine(RotateGimbals(90, -1, 5));
                StartCoroutine(RotateGimbals(90, 1, 7));
                StartCoroutine(RotateGimbals(45, -1, 9));
                Invoke("ResetRotation", 11);
                Invoke("ResetComment", 11);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator Demo3()
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
                quaternions = false;
                comment.text = "Внутренний блок в Unity3d.\nВращение вокруг\nоси " + xColor +
                    "X</color> застопоривается\nв положении 90° и -90°\nпри использовании\n" + eulerColor +
                    "углов Эйлера</color>\nно работает\n с " + quaternionColor + "кватернионами</color>";
                Invoke("SwitchAuto", 3);
                Invoke("SwitchQuaternions", 10);
                Invoke("SwitchQuaternions", 18);
                Invoke("SwitchQuaternions", 26);
                Invoke("ResetComment", 34);
                break;
            }
            yield return null;
        }
        yield return null;
    }

    void SwitchAuto()
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

    void SwitchQuaternions()
    {
        if (quaternions)
        {
            quaternions = false;
            shift.text = "Shift —\nИспользовать " + quaternionColor + "кватернионы</color>";
        }
        else
        {
            quaternions = true;
            shift.text = "Shift —\nИспользовать " + eulerColor + "углы Эйлера</color>";
        }
    }

    IEnumerator RotateX(int count, int speed = 1, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        for (var i = 0; i < count; i++)
        {
            if (quaternions)
            {
                gimbalX.localRotation *= Quaternion.Euler(speed, 0, 0);
                yield return null;
            }
            else
            {
                gimbalX.localEulerAngles += new Vector3(speed, 0, 0);
                yield return null;
            }
        }
    }

    IEnumerator RotateY(int count, int speed = 1, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        for (var i = 0; i < count; i++)
        {
            if (quaternions)
            {
                gimbalY.localRotation *= Quaternion.Euler(0, speed, 0);
                yield return null;
            }
            else
            {
                gimbalY.localEulerAngles += new Vector3(0, speed, 0);
                yield return null;
            }
        }
    }

    IEnumerator RotateZ(int count, int speed = 1, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        for (var i = 0; i < count; i++)
        {
            if (quaternions)
            {
                gimbalZ.localRotation *= Quaternion.Euler(0, 0, speed);
                yield return null;
            }
            else
            {
                gimbalZ.localEulerAngles += new Vector3(0, 0, speed);
                yield return null;
            }
        }
    }

    IEnumerator RotateGimbals(int count, int speed, float wait = 0)
    {
        yield return new WaitForSeconds(wait);
        for (var i = 0; i < count; i++)
        {
            if (quaternions)
            {
                gimbalZ.localRotation *= Quaternion.Euler(0, speed, 0);
                yield return null;
            }
            else
            {
                gimbalZ.localEulerAngles += new Vector3(0, speed, 0);
                yield return null;
            }
        }
    }

    IEnumerator RotateTo(float X, float Y, float Z)
    {
        reset = false;
        if (!quaternions) SwitchQuaternions();
        if (auto) SwitchAuto();
        var quatX = Quaternion.Euler(X, 0, 0);
        var quatY = Quaternion.Euler(0, Y, 0);
        var quatZ = Quaternion.Euler(0, 0, Z);
        while (true)
        {
            if (gimbalX.localRotation != quatX)
            {
                gimbalX.localRotation = Quaternion.RotateTowards(gimbalX.localRotation, quatX, Time.deltaTime * 60);
            }
            if (gimbalY.localRotation != quatY)
            {
                gimbalY.localRotation = Quaternion.RotateTowards(gimbalY.localRotation, quatY, Time.deltaTime * 60);
            }
            if (gimbalZ.localRotation != quatZ)
            {
                gimbalZ.localRotation = Quaternion.RotateTowards(gimbalZ.localRotation, quatZ, Time.deltaTime * 60);
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

    void ResetRotation()
    {
        StartCoroutine(RotateTo(0, 0, 0));
    }

    void ResetComment()
    {
        comment.text = "1, 2, 3 —\nПереключение\nдемонстраций\nс комментариями";
        controls.SetActive(true);
        tab.gameObject.SetActive(true);
        shift.gameObject.SetActive(true);
    }
}

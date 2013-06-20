using System.Collections;
using UnityEngine;

public class GimbalLock : MonoBehaviour
{
    public Transform gimbalX;
    public Transform gimbalY;
    public Transform gimbalZ;
    public GUIText xText;
    public GUIText yText;
    public GUIText zText;
    public GUIText shift;
    public GUIText tab;
    public GUIText comment;
    public GameObject controls;
    public AudioSource click;
    public AudioSource tick;

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

	void Update ()
	{
        if (Input.GetKey("escape")) Application.Quit();
        if (Input.GetKeyDown("left shift") || Input.GetKeyDown("right shift")) SwitchQuaternions();
        if (Input.GetKeyDown("tab")) SwitchAuto();
        if (Input.GetKeyDown("space")) ResetRotation();

        if (Input.GetKeyDown("1")) StartCoroutine(Demo1());
        if (Input.GetKeyDown("2")) StartCoroutine(Demo2());
        if (Input.GetKeyDown("3")) StartCoroutine(Demo3());
        if (Input.GetKeyDown("0"))
        {
            if (onAir)
            {
                ResetComment();
                onAir = false;
            }
            else
            {
                StartCoroutine(RotateTo(gimbalX.localEulerAngles.x + 180, gimbalY.localEulerAngles.y + 180, gimbalZ.localEulerAngles.z + 180));
                comment.text = ";)";
                onAir = true;
                StartCoroutine(Capture(0.3f));
            }
        }

        if (quaternions)
        {
            if (Input.GetKey("d") || auto) gimbalX.localRotation *= Quaternion.Euler(+1, 0, 0);
            if (Input.GetKey("a")) gimbalX.localRotation *= Quaternion.Euler(-1, 0, 0);
            if (Input.GetKey("w") || auto) gimbalY.localRotation *= Quaternion.Euler(0, +1, 0);
            if (Input.GetKey("s")) gimbalY.localRotation *= Quaternion.Euler(0, -1, 0);
            if (Input.GetKey("e") || auto) gimbalZ.localRotation *= Quaternion.Euler(0, 0, +1);
            if (Input.GetKey("q")) gimbalZ.localRotation *= Quaternion.Euler(0, 0, -1);
        }
        else
        {
            if (Input.GetKey("d") || auto) gimbalX.localEulerAngles += new Vector3(+1, 0, 0);
            if (Input.GetKey("a")) gimbalX.localEulerAngles += new Vector3(-1, 0, 0);
            if (Input.GetKey("w") || auto) gimbalY.localEulerAngles += new Vector3(0, +1, 0);
            if (Input.GetKey("s")) gimbalY.localEulerAngles += new Vector3(0, -1, 0);
            if (Input.GetKey("e") || auto) gimbalZ.localEulerAngles += new Vector3(0, 0, +1);
            if (Input.GetKey("q")) gimbalZ.localEulerAngles += new Vector3(0, 0, -1);

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
        xText.text = "<color=#f14121ff>X: " + gimbalX.localEulerAngles.x.ToString("F0") + "° " + xArrow + "</color>";
        yText.text = "<color=#98f145ff>Y: " + gimbalY.localEulerAngles.y.ToString("F0") + "° " + yArrow + "</color>";
        zText.text = "<color=#3d80f1ff>Z: " + gimbalZ.localEulerAngles.z.ToString("F0") + "° " + zArrow + "</color>";
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
                comment.text = "Выравнивание рамки <color=#98f145ff>Y</color>\nи рамки <color=#3d80f1ff>Z</color> приводит\nк потере подвижности.\nВращение вокруг\nоси <color=#f14121ff>X</color> и оси <color=#3d80f1ff>Z</color>\nприводит к\nодинаковому результату";
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
                comment.text = "Внутренний блок в Unity3d.\nВращение вокруг\nоси <color=#f14121ff>X</color> застопоривается\nв положении 90° и -90°\nпри использовании\n<color=#f14121ff>углов Эйлера</color>\nно работает\n с <color=#98f145ff>кватернионами</color>";
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
            shift.text = "Shift —\nИспользовать <color=#98f145ff>кватернионы</color>";
        }
        else
        {
            quaternions = true;
            shift.text = "Shift —\nИспользовать <color=#f14121ff>углы Эйлера</color>";
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
        if(!quaternions) SwitchQuaternions();
        if(auto) SwitchAuto();
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

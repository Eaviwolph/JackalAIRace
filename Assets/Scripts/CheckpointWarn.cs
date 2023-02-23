using UnityEngine;

public class CheckpointWarn : MonoBehaviour
{
    public int checkNum;
    public int bonus = 100;

    void Start()
    {
        string num = "";
        int i = 0;
        while (i < gameObject.name.Length && gameObject.name[i] != '(')
        {
            i++;
        }
        i++;
        while (i < gameObject.name.Length && gameObject.name[i] != ')')
        {
            num += gameObject.name[i];
            i++;
        }
        try
        {
            checkNum = int.Parse(num);
        }
        catch (System.Exception)
        {
            checkNum = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.gameObject.GetComponent<CarController>().score / other.gameObject.GetComponent<CarController>().zoneMax >= 3)
        {
            other.gameObject.GetComponent<CarController>().score += bonus;
            bonus--;
            other.gameObject.GetComponent<CarController>().Kill();
        }
    }
}

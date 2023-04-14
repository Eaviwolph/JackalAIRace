using UnityEngine;

public class CheckpointWarn : MonoBehaviour
{
    public Spawner spawner;
    public int arrived = 0;
    public int checkNum;
    public int bonus = 100;

    void Start()
    {
        arrived = 0;
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
            other.gameObject.GetComponent<CarController>().Kill();
            arrived++;
            Debug.Log("Someone has does 3 turns");
        }
        if (arrived >= spawner.selected)
        {
            arrived = 0;
            spawner.KillAll();
            Debug.Log("Selecting the best cars");
        }
    }
}

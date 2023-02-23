using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class Spawner : SavePath
{
    [Header("KeyCodes")]
    public KeyCode killAllKey = KeyCode.K;
    public KeyCode autoMut = KeyCode.M;

    [Header("Commands")]
    public bool launch;
    public bool mutate;
    public bool autoMutate;
    public bool autoMutateActivation;
    public bool destroy;
    public bool killAll;

    [Header("Selector")]
    public int selected;
    public int childRand;
    public int childMed;
    public float mutatePercent;

    [Header("Cars")]
    public GameObject carPref;
    public GameObject[] Spawned = new GameObject[10];


    public IEnumerator couroutine = null;

    private void Update()
    {
        if (launch)
        {
            Launch();
        }
        if (destroy)
        {
            Destroy();
        }
        if (mutate)
        {
            Mutate();
        }
        if (killAll || Input.GetKeyDown(killAllKey))
        {
            KillAll();
        }
        if (Input.GetKeyDown(autoMut) && autoMutateActivation)
        {
            autoMutateActivation = false;
        }
        else if (Input.GetKeyDown(autoMut) && !autoMutateActivation)
        {
            autoMutate = true;
        }
        if (autoMutate && !autoMutateActivation)
        {
            autoMutateActivation = true;
            autoMutate = false;
            Launch();
            StartCoroutine(WaitAndMutate());
        }
    }

    public void KillAll()
    {
        for (int i = 0; i < Spawned.Length; i++)
        {
            if (Spawned[i] != null)
            {
                Spawned[i].GetComponent<CarController>().Kill();
            }
        }
        killAll = false;
    }

    public void Mutate()
    {
        KillAll();
        Array.Sort(Spawned, (x, y) => y.GetComponent<CarController>().score.CompareTo(x.GetComponent<CarController>().score));
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
        Directory.CreateDirectory(path);
        for (int i = 0; i < selected; i++)
        {
            if (Spawned[i] != null)
            {
                Spawned[i].GetComponent<NeuralNetwork>().SaveAll(i, path);
                Destroy(Spawned[i]);
                Spawned[i] = null;
            }
        }
        for (int i = selected; i < Spawned.Length; i++)
        {
            if (Spawned[i] != null)
            {
                Destroy(Spawned[i]);
                Spawned[i] = null;
            }
        }

        for (int i = 0; i < selected; i++)
        {
            if (Spawned[i] == null)
            {
                Spawned[i] = Instantiate(carPref, transform.position, transform.rotation);
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().Init(i, path);

                Spawned[i].GetComponent<Renderer>().material.color = Color.blue;
            }
        }
        for (int i = selected; i < selected + childRand; i++)
        {
            if (Spawned[i] == null)
            {
                Spawned[i] = Instantiate(carPref, transform.position, transform.rotation);
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().Init(i % selected, path);

                Spawned[i].GetComponent<NeuralNetwork>().ChilRandWith(Spawned[(i + UnityEngine.Random.Range(0, selected)) % selected].GetComponent<NeuralNetwork>());
                Spawned[i].GetComponent<NeuralNetwork>().MutateAll(mutatePercent / 10);
                Spawned[i].GetComponent<Renderer>().material.color = Color.red;
            }
        }

        for (int i = selected + childRand; i < selected + childRand + childMed; i++)
        {
            if (Spawned[i] == null)
            {
                Spawned[i] = Instantiate(carPref, transform.position, transform.rotation);
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().Init(i % selected, path);

                Spawned[i].GetComponent<NeuralNetwork>().ChildAverWith(Spawned[(i + UnityEngine.Random.Range(0, selected)) % selected].GetComponent<NeuralNetwork>());
                Spawned[i].GetComponent<NeuralNetwork>().MutateAll(mutatePercent / 10);
                Spawned[i].GetComponent<Renderer>().material.color = Color.green;
            }
        }

        for (int i = selected + childRand + childMed; i < Spawned.Length; i++)
        {
            if (Spawned[i] == null)
            {
                Spawned[i] = Instantiate(carPref, transform.position, transform.rotation);
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().Init(i % selected, path);

                Spawned[i].GetComponent<NeuralNetwork>().MutateAll(mutatePercent);
            }
        }
        mutate = false;
    }

    public void Launch()
    {
        for (int i = 0; i < Spawned.Length; i++)
        {
            if (Spawned[i] == null)
            {
                Spawned[i] = Instantiate(carPref, transform.position, transform.rotation);
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().Init(i, path);
            }
        }
        launch = false;
    }

    public void Activate()
    {
        for (int i = 0; i < Spawned.Length; i++)
        {
            if (Spawned[i] != null)
            {
                Spawned[i].GetComponent<CarController>().enabled = true;
                Spawned[i].GetComponent<NeuralNetwork>().enabled = true;
            }
        }
    }

    public void Destroy()
    {
        for (int i = 0; i < Spawned.Length; i++)
        {
            if (Spawned[i] != null)
            {
                Destroy(Spawned[i]);
                Spawned[i] = null;
            }
        }
        destroy = false;
    }

    IEnumerator WaitAndMutate()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(1);
        bool noerror = true;
        for (int i = 0; i < Spawned.Length; i++)
        {
            if (Spawned[i] == null || !Spawned[i].GetComponent<CarController>().dead)
            {
                noerror = false;
                break;
            }
        }
        if (noerror)
        {
            Mutate();
        }
        if (autoMutateActivation)
        {
            StartCoroutine(WaitAndMutate());
        }
    }
}

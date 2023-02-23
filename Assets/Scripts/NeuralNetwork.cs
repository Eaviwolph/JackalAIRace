using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    CarController carController;
    
    public int[] layersCount;
    public List<Neuron[]> layers;

    public void Init(int neuralId, string path)
    {
        carController = GetComponent<CarController>();
        layersCount[0] = carController.distList.Length;
        layers = new List<Neuron[]>();
        int count = 0;
        for (int i = 1; i < layersCount.Length; i++)
        {
            Neuron[] lay = new Neuron[layersCount[i]];
            for (int j = 0; j < layersCount[i]; j++)
            {
                lay[j] = new Neuron(count, layersCount[i - 1], path + neuralId + @"\");
                count++;
            }
            layers.Add(lay);
        }
    }

    public void FixedUpdate()
    {
        float[] inputs = new float[layersCount[0]];
        float max = Mathf.Max(carController.distList);
        for (int j = 0; j < layersCount[0]; j++)
        {
            inputs[j] = carController.distList[j] / max;
        }

        for (int i = 0; i < layers.Count; i++)
        {
            float[] nextInput = new float[layers[i].Length];
            for (int j = 0; j < layers[i].Length; j++)
            {
                nextInput[j] = layers[i][j].Calculate(inputs);
            }
            inputs = nextInput;
        }

        if (inputs[0] < 0.5)
        {
            carController.front = true;
        }
        else if (inputs[0] < 0.6)
        {
            carController.back = true;
        }
        if (inputs[1] < 0.3)
        {
            carController.right = true;
        }
        else if (inputs[1] < 0.6)
        {
            carController.left = true;
        }
    }

    public void SaveAll(int nid, string path)
    {
        if (!Directory.Exists(path + nid))
        {
            Directory.CreateDirectory(path + nid);
        }
        foreach (Neuron[] layer in layers)
        {
            foreach (Neuron neuron in layer)
            {
                neuron.Save(path + nid + @"\");
            }
        }
    }

    public void MutateAll(float mutatePercent)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Length; j++)
            {
                layers[i][j].Mutate(mutatePercent);
            }
        }
    }
    
    public void ChilRandWith(NeuralNetwork other)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Length; j++)
            {
                for (int k = 0; k < layers[i][j].weights.Length; k++)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        layers[i][j].weights[k] = other.layers[i][j].weights[k];
                    }
                }
            }
        }
    }

    public void ChildAverWith(NeuralNetwork other)
    {
        for (int i = 0; i < layers.Count; i++)
        {
            for (int j = 0; j < layers[i].Length; j++)
            {
                for (int k = 0; k < layers[i][j].weights.Length; k++)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        layers[i][j].weights[k] += other.layers[i][j].weights[k];
                        layers[i][j].weights[k] /= 2;
                    }
                }
            }
        }
    }
}

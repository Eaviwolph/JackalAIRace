using System.IO;
using UnityEngine;

public class Neuron
{
    public float[] weights;
    public float bias;

    public float output;
    public int id;
    public Neuron(int id, int prec, string path)
    {
        this.id = id;
        if (File.Exists(path + id) && File.ReadAllLines(path + id).Length == prec + 1)
        {
            string line = File.ReadAllText(path + id);
            string[] values = line.Split('\n');
            this.bias = float.Parse(values[0]);
            this.weights = new float[values.Length - 1];
            for (int i = 1; i < values.Length; i++)
            {
                this.weights[i - 1] = float.Parse(values[i]);
            }
        }
        else
        {
            this.bias = Random.Range(-1f, 1f);
            this.weights = new float[prec];
            for (int i = 0; i < this.weights.Length; i++)
            {
                this.weights[i] = Random.Range(-1f, 1f);
            }
        }
    }

    public Neuron(int NetworkId, int id, float output)
    {
        this.output = output;
    }

    public void Save(string path)
    {
        string line = bias + "\n";
        for (int i = 0; i < weights.Length; i++)
        {
            if (i != weights.Length - 1)
            {
                line += weights[i] + "\n";
            }
            else
            {
                line += weights[i];
            }
        }
        File.WriteAllText(path + id, line);
    }

    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    public float Calculate(float[] inputs)
    {
        float sum = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            sum += inputs[i] * weights[i];
        }
        sum += bias;
        output = Sigmoid(sum);
        return output;
    }

    public void Mutate(float mutatePercent)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = weights[i] * (1 + Random.Range(-mutatePercent, mutatePercent));
        }
        bias = bias * (1 + Random.Range(-mutatePercent, mutatePercent));
    }
}

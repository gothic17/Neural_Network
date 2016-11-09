using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public class LinearNetwork
    {
        // Tworzy sieæ o liczbie neuronów = nummberOfNeurons z liczb¹ wejœæ w ka¿dym = numberOfInputs
        public LinearNetwork(int numberOfInputs, int numberOfNeurons)
        {
            _neurons = new Neuron[numberOfNeurons];
            _inputCount = numberOfInputs;
            for (int i = 0; i < _neurons.Length; i++)
                _neurons[i] = new Neuron(numberOfInputs);
        }

        // Na wejœciu bierze tablicê 2- wymiarow¹. GetLength(0) zwraca liczbê neuronów (czyli liczbê np. wierszy), 
        // a w kolumnach podane s¹ kolejne wartoœci wag dla danego wejœcia
        public LinearNetwork(double[,] initialWeights)
            : this(initialWeights.GetLength(1), initialWeights.GetLength(0)) // Wywo³uje powy¿szy konstruktor 
                                                        //(tylko tworzy sieæ o podanej liczbie neuronów i wejœæ).
        {
            // Ustala wartoœci wag na wszystkich wejœciach neuronów
            for (int neuron = 0; neuron < _neurons.Length; neuron++)
                for (int input = 0; input < _inputCount; input++)
                    _neurons[neuron].Weights[input] = initialWeights[neuron, input];
        }

        private Neuron[] _neurons;

        private double[] _neurons_dist;

        private readonly int _inputCount;

        public double[] Response(double[] inputSignals)
        {
            if (inputSignals == null || inputSignals.Length != _inputCount)
                throw new ArgumentException(
                    "The signal array's length must be equal to the number of inputs.");

            double[] res = new double[_neurons.Length];
            for (int i = 0; i < res.Length; i++)
                res[i] = _neurons[i].Response(inputSignals);

            return res;
        }


        public double Winner(double[] inputSignals)
        {
            double max=0;

            double res;
            for (int i = 0; i < _neurons.Length; i++)
            {
                res= _neurons[i].Response(inputSignals);
                if (Math.Abs(res)>max)
                    max=res;
            }
            return max;
        }     
        
        public void Learn(TeachingSet.Element teachingElement, double etha)
        {
            double _max = 0;

            
            _max = Winner(teachingElement.Inputs);

            for (int x = 1; x < _neurons.Length; x++)
            {

            }

                for (int neuronIndex = 0; neuronIndex < _neurons.Length; neuronIndex++)
                {
                    _neurons[neuronIndex].Learn(
                        teachingElement.Inputs,
                        etha,
                        _max
                    );
                }
        }

        /* dodano dla samouczacego sie neuronu */
        public void Learn(TeachingSet.Element teachingElement,int neuronIndex, double etha)
        {
                 _neurons[neuronIndex].LearnSelf(
                    teachingElement.Inputs,
                    etha
                );
        }

        public void Learn(TeachingSet.Element teachingElement, double ratio,
            ref double[] previousResponse, ref double[] previousError)
        {
            if (previousResponse == null)
                previousResponse = new double[_neurons.Length];
            if (previousError == null)
                previousError = new double[_neurons.Length];

            for (int neuronIndex = 0; neuronIndex < _neurons.Length; neuronIndex++)
            {
                _neurons[neuronIndex].Learn(
                    teachingElement.Inputs,
                    teachingElement.ExpectedOutputs[neuronIndex],
                    ratio,
                    out previousResponse[neuronIndex],
                    out previousError[neuronIndex]
                );
            }
        }

        public void Randomize(Random randomGenerator, double min, 
            double max, double epsilon)
        {
            foreach (Neuron n in _neurons)
                n.Randomize(randomGenerator, min, max,epsilon);
        }
    }
}

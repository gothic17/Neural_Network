using System;

namespace NeuralNetwork {
    public class LinearNetwork {
        private Neuron[] neurons;
        private readonly int inputCount;

        // Tworzy sieć o liczbie neuronów = nummberOfNeurons z liczbą wejść w każdym = numberOfInputs
        public LinearNetwork(int numberOfInputs, int numberOfNeurons) {
            neurons = new Neuron[numberOfNeurons];
            inputCount = numberOfInputs;
            for (int i = 0; i < _neurons.Length; i++) {
                neurons[i] = new Neuron(numberOfInputs);
            }
        }

        // Na wejściu bierze tablicę 2- wymiarową. GetLength(0) zwraca liczbę neuronów (czyli liczbę np. wierszy), 
        // a w kolumnach podane są kolejne wartości wag dla danego wejścia
        public LinearNetwork(double[,] initialWeights)
            : this(initialWeights.GetLength(1), initialWeights.GetLength(0)) // Wywołuje powyższy konstruktor 
                                                                             //(tylko tworzy sieć o podanej liczbie neuronów i wejść).
        {
            // Ustala wartości wag na wszystkich wejściach neuronów
            for (int neuron = 0; neuron < neurons.Length; neuron++)
                for (int input = 0; input < inputCount; input++)
                    neurons[neuron].Weights[input] = initialWeights[neuron, input];
        }

        public double[] Response(double[] inputSignals) {
            if (inputSignals == null || inputSignals.Length != inputCount)
                throw new ArgumentException("The signal array's length must be equal to the number of inputs.");

            double[] response = new double[neurons.Length];
            for (int i = 0; i < neurons.Length; i++) {
                response[i] = neurons[i].Response(inputSignals);
            }

            return response;
        }


        public double Winner(double[] inputSignals) {
            double max = 0;

            double res;
            for (int i = 0; i < _neurons.Length; i++) {
                res = neurons[i].Response(inputSignals);
                if (Math.Abs(res) > max)
                    max = res;
            }
            return max;
        }

        public void Learn(double[] inputSignals, double etha) {
            double max = 0;
            max = Winner(inputSignals);

            for (int neuronIndex = 0; neuronIndex < _neurons.Length; neuronIndex++) {
                neurons[neuronIndex].Learn(inputSignals, etha, max);
            }
        }

        /* dodano dla samouczacego sie neuronu */
        public void Learn(double[] inputSignals, int neuronIndex, double etha) {
            neurons[neuronIndex].SelfLearn(inputSignals, etha);
        }

        public void Learn(double[] inputSignals, double ratio,
            ref double[] previousResponse, ref double[] previousError) {
            if (previousResponse == null)
                previousResponse = new double[_neurons.Length];
            if (previousError == null)
                previousError = new double[_neurons.Length];

            for (int neuronIndex = 0; neuronIndex < _neurons.Length; neuronIndex++) {
                _neurons[neuronIndex].Learn(
                    inputSignals,
                    teachingElement.ExpectedOutputs[neuronIndex],
                    ratio,
                    out previousResponse[neuronIndex],
                    out previousError[neuronIndex]
                );
            }
        }

        public void RandomizeWeightsOfAllNeurons(double min, double max, double epsilon) {
            foreach (Neuron n in neurons) {
                n.Randomize(min, max, epsilon);
            }
        }
    }
}

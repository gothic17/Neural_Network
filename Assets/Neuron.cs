using System;

namespace NeuralNetwork {
    [Serializable]
    public class Neuron {
        public int numberOfInputs;
        private double[] weights;
        private double[] previousWeights;

        // Creates weights and previousWeights of neuron (it's number is numberOfInputs)
        public Neuron(int numberOfInputs) {
            this.numberOfInputs = numberOfInputs;
            weights = new double[numberOfInputs];
            previousWeights = new double[numberOfInputs];
        }

        // Sigmoidal function used on output of a neuron. output - signal, that will be modified, t - param of sigmoidal function (it is used in exponential function)
        public double ActivationFunction(double output, double t) {
            return 1.0 / (1.0 + Math.Exp(-t * output));
        }

        // It returns response of neuron - it is Sqrt(Sum(weight(i) * input(i)))
        public double Response(double[] inputSignals) {
            double response = 0.0;
            if (inputSignals.Length == numberOfInputs) {
                for (int i = 0; i < numberOfInputs; i++) {
                    response += weights[i] * inputSignals[i];
                }
            } else {
                throw new ArgumentException("Number of input signals unequal to amount of inputs in neuron, or is null");
            }

            return ActivationFunction(response, 0.25);
        }

        // Returns strength of given array. It is calculated according to the Euclidean norm - 
        // Sqrt(Sum(weight(i)^2)). It can be later expanded to use other norms.
        public static double Strength(double[] inputSignals) {
            double strenght = 0.0;
            for (int i = 0; i < inputSignals.Length; i++) {
                strenght += Math.Pow(inputSignals[i], 2);
            }
            strenght = Math.Sqrt(strenght);

            return strenght;
        }

        // Normalizes input signals so that they are <= 1
        public static void Normalize(double[] inputSignals) {
            double strength = Strength(inputSignals);
            for (int i = 0; i < inputSignals.Length; i++) {
                inputSignals[i] /= strength;
            }
        }

        // Self learning
        public void SelfLearn(double[] inputSignals, double etha) {
            for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += etha * (inputSignals[i] - weights[i]);
            }
        }

        // Self learning with maximum
/*        public void SelfLearn(double[] inputSignals, double etha, double max) {
            double previous_response = Response(inputSignals);

            if (previous_response < 0.2 * max)
                previous_response *= 0.3;
            if (previous_response < 0)
                previous_response *= 0.1;

            for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += etha * previous_response * (inputSignals[i] - weights[i]);
            }
        }

*/
        // Generate random value for each weight (between min and max)
        public void RandomizeWeights(Random randomGenerator, double min, double max, double epsilon) {
            double length = max - min;
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = min + length * randomGenerator.NextDouble();
                if (Math.Abs(weights[i]) < epsilon)
                    weights[i] = epsilon;
            }
        }

        public double[] GetWeights() {
            return this.weights;
        }

        public void SetWeights(double[] weights) {
            this.weights = weights;
        }

        public double[] GetPreviousWeights() {
            return this.previousWeights;
        }

        public void SetPreviousWeights(double[] previousWeights) {
            this.previousWeights = previousWeights;
        }
    }
}

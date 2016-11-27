using System;

namespace NeuralNetwork {
    [Serializable]
    public class Neuron {
        public int numberOfInputs;
        public double[] weights;
        public double[] previousWeights;
        public double p0;

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

        public void RestorePreviousWeights() {
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = previousWeights[i];
            }
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

       /* public void Learn(double[] signals, double[] previous_weights, double error, double sigma, double ratio, double momentum) {
            for (int i = 0; i < weights.Length; i++)
                weights[i] += ratio * sigma * signals[i] - momentum * (weights[i] - previous_weights[i]);
        }*/

        /* dodane dla przykladu 10a,ax,b */
        public void Learn(double[] signals, double etha) {
            double previous_response = Response(signals);

            for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += etha * previous_response * (signals[i] - weights[i]);
            }
        }

        public void RandomLearn(Random randomGenerator, double etha) {
            double length = 2.0;
            double p = -1.0 + length * randomGenerator.NextDouble(); // Generate random variable p from [-1.0, 1.0], which will be added to our weights
            p0 = p;
            //double previous_response = Response(signals);

            /*for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += etha * previous_response * (signals[i] - weights[i]);
            }*/
            for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += (p * weights[i]);
            }
        }


        // Self learning
        public void SelfLearn(double[] inputSignals, double etha) {
            for (int i = 0; i < weights.Length; i++) {
                previousWeights[i] = weights[i];
                weights[i] += etha * (inputSignals[i] - weights[i]);
            }
        }

        // Generate random value for each weight (between min and max)
        public void RandomizeWeights(Random randomGenerator, double min, double max, double epsilon) {
            double length = max - min;
            for (int i = 0; i < weights.Length; i++) {
                weights[i] = min + length * randomGenerator.NextDouble();
                if (Math.Abs(weights[i]) < epsilon) {
                    weights[i] = epsilon;
                }
                previousWeights[i] = weights[i];
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

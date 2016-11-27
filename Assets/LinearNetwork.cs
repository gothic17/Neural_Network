using System;

namespace NeuralNetwork {
    [Serializable]
    public class LinearNetwork {
        public Neuron[] neurons;
        public int numberOfInputs;
        public int numberOfNeurons;
        public Random randomGenerator;

        public LinearNetwork(int numberOfInputs, int numberOfNeurons, Random randomGenerator) {
            this.numberOfInputs = numberOfInputs;
            this.numberOfNeurons = numberOfNeurons;
            this.randomGenerator = randomGenerator;
            neurons = new Neuron[numberOfNeurons];
            for (int i = 0; i < numberOfNeurons; i++) {
                    neurons[i] = new Neuron(numberOfInputs);
            }
        }

        public void RandomizeWeightsOfAllNeurons(double min, double max, double epsilon) {
           // Random randomGenerator = new Random(); // Random generator is initialized using clock value. We have to create only one instance and pass it 
            // to neuron.RandomWeights. If we would create new instance every time in short period of time it would be returning same number every time
            for (int i = 0; i < numberOfNeurons; i++) {
                    neurons[i].RandomizeWeights(randomGenerator, min, max, epsilon);
            }
        }

        public void RestorePreviousWeights() {
            for (int i = 0; i < neurons.Length; i++) {
                neurons[i].RestorePreviousWeights();
            }
        }

        public void RandomLearn(Random randomGenerator) {
            for(int i = 0; i < neurons.Length; i++) {
                neurons[i].RandomLearn(randomGenerator, 0.1); // 0.1 is etha
            }
        }

        public double[] Response(double[] inputSignals) {
            double[] response = new double[numberOfNeurons];
            for (int i = 0; i < numberOfNeurons; i++) {
                response[i] = neurons[i].Response(inputSignals);
            }
            return response;
        }
    }
}

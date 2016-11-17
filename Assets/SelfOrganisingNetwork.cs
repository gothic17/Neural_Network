using System;

namespace NeuralNetwork {
    public class SelfOrganisingNetwork {

        public Neuron[,] neurons;
        public int numberOfInputs;
        public int netSizeX;
        public int netSizeY;

        // Tworzy sieć o liczbie neuronów = nummberOfNeurons z liczbą wejść w każdym = numberOfInputs
        public SelfOrganisingNetwork(int numberOfInputs, int netSizeX, int netSizeY) {
            this.numberOfInputs = numberOfInputs;
            this.netSizeX = netSizeX;
            this.netSizeY = netSizeY;
            neurons = new Neuron[netSizeX, netSizeY];
            for (int i = 0; i < netSizeX; i++) {
                for (int j = 0; j < netSizeY; j++) {
                    neurons[i, j] = new Neuron(numberOfInputs);
                }
            }
        }

        public void RandomizeWeightsOfAllNeurons(double min, double max, double epsilon) {
            Random randomGenerator = new Random(); // Random generator is initialized using clock value. We have to create only one instance and pass it 
            // to neuron.RandomWeights. If we would create new instance every time in short period of time it would be returning same number every time
            for (int i = 0; i < netSizeX; i++) {
                for (int j = 0; j < netSizeY; j++) {
                    neurons[i, j].RandomizeWeights(randomGenerator, min, max, epsilon);
                }
            }
        }

        public double[] Response(double[] inputSignals) {
            double[] response = new double[netSizeX * netSizeY];
            for (int i = 0; i < netSizeX; i++) {
                for (int j = 0; j < netSizeY; j++) {
                    response[(netSizeX * i) + j] = neurons[i, j].Response(inputSignals);
                }
            }
            return response;
        }

        // Returns array with location [x,y] of neuron, whose distance is closest to the vector of input signals
        public int[] Winner(double[] inputSignals) {
            int x = 0;
            int y = 0;
            double minimalDistance = 999999999;
            double distance;
            for (int i = 0; i < netSizeX; i++) {
                for (int j = 0; j < netSizeY; j++) {
                    distance = 0;
                    for (int k = 0; k < neurons[i, j].GetWeights().Length; k++) {
                        distance += Math.Pow(neurons[i, j].GetWeights()[k] - inputSignals[k], 2);
                    }
                    distance = Math.Sqrt(distance);

                    if (distance < minimalDistance) {
                        minimalDistance = distance;
                        x = i;
                        y = j;
                    }
                }
            }
            return new int[] { x, y };
        }
    }
}

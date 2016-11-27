using System;

namespace NeuralNetwork {
    [Serializable]
    public class SelfOrganisingNetwork {
        public Neuron[,] neurons;
        public int numberOfInputs;
        public int netSizeX;
        public int netSizeY;
        public Random randomGenerator;

        // Tworzy sieć o liczbie neuronów = nummberOfNeurons z liczbą wejść w każdym = numberOfInputs
        public SelfOrganisingNetwork(int numberOfInputs, int netSizeX, int netSizeY, Random randomGenerator) {
            this.numberOfInputs = numberOfInputs;
            this.netSizeX = netSizeX;
            this.netSizeY = netSizeY;
            this.randomGenerator = randomGenerator;
            neurons = new Neuron[netSizeX, netSizeY];
            for (int i = 0; i < netSizeX; i++) {
                for (int j = 0; j < netSizeY; j++) {
                    neurons[i, j] = new Neuron(numberOfInputs);
                }
            }
        }

        public void RandomizeWeightsOfAllNeurons(double min, double max, double epsilon) {
            //Random randomGenerator = new Random(); // Random generator is initialized using clock value. We have to create only one instance and pass it 
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

        public void Learn(double[] inputSignals) {
            double odlmin = 10000;
            double alpha;
            double odl = 0.0;

			int imin = Winner(inputSignals)[0];
            int jmin = Winner(inputSignals)[1];

            double alpha0 = 0.1;
            double alpha1 = 0.02;
            int min;
            int max;
            min = netSizeX;
            if (netSizeY < netSizeX)
                min = netSizeY;

            max = netSizeX;
            if (netSizeY > netSizeX)
                max = netSizeY;

            // Round neighbour
            int s = Convert.ToInt32((min + max) / 4.0 + 1); // s is here instead of neighbour
            //int s = Convert.ToInt16(Neighbour);

            for (int i = imin - s; i <= imin + s; i++) {
                if (i >= 0 && i < netSizeX) {
                    for (int j = jmin - s; j <= jmin + s; j++) {
                        if (j >= 0 && j < netSizeY) {
                            odl = Math.Abs(i - imin);

                            if (Math.Abs(j - jmin) > odl)
                                odl = Math.Abs(j - jmin);

                            if (odl <= s) {
                                if (s > 0) {
                                    alpha = (alpha0 * (s - odl) + alpha1 * odl) / s;
                                } else {
                                    alpha = alpha0;
                                }
                                //_examinedNetwork.Learn(_teachingElement, _MapIndex(i, j), alpha);
								neurons[i, j].SelfLearn(inputSignals, alpha);
                            }
                        }
                    }
                }
            }

        }
    }
}

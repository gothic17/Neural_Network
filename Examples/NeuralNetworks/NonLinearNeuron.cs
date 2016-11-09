using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public abstract class NonLinearNeuron: Neuron
    {
        public NonLinearNeuron(int inputCount) : base(inputCount) { }

        public abstract double ActivationFunction(double arg);

        public override double Response(double[] inputSignals)
        {
            return ActivationFunction(base.Response(inputSignals));
        }

        public void LearnWidrowHoff(double[] signals, double expectedOutput,
            double ratio, out double previousResponse, out double previousError)
        {
            // Take the linear response into consideration.
            previousResponse = base.Response(signals);
            previousError = expectedOutput - previousResponse;
            for (int i = 0; i < Weights.Length; i++)
                Weights[i] += ratio * previousError * signals[i];
        }

    }
}

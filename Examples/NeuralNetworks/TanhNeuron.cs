using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public class TanhNeuron:NonLinearNeuron
    {
        public TanhNeuron(int inputCount) : base(inputCount) { }

        public override double ActivationFunction(double arg)
        {
            return Math.Tanh(arg);
        }
    }
}

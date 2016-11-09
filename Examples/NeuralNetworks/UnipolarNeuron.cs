using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public class UnipolarNeuron : NonLinearNeuron
    {
        public UnipolarNeuron(int inputCount) : base(inputCount) { }

        public override double ActivationFunction(double arg)
        {
            return arg > 0 ? 1 : 0;
        }
    }
}

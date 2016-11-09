using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public class SigmoidalNeuron : NonLinearNeuron
    {
        public SigmoidalNeuron(int inputCount) : base(inputCount) { }

        private double _beta = 1.0;

        public double Beta
        {
            get { return _beta; }
            set { _beta = value; }
        }

        public override double ActivationFunction(double arg)
        {
            return 1.0 / (1.0 + Math.Exp(-_beta * arg));
        }

    }
}

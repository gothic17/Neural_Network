using System;
using System.Collections.Generic;
using System.Text;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    public class MlpNetwork
    {
        private MlpNetwork(int inputCount, bool hasBias)
        {
            if (inputCount <= 0)
                throw new ArgumentOutOfRangeException("inputCount");
            _inputCount = inputCount;
            _hasBias = hasBias;
        }

        public MlpNetwork(int inputCount, bool hasBias, IList<NonLinearNeuron[]> layers)
            : this(inputCount, hasBias)
        {
            if (layers == null)
                throw new ArgumentNullException("layers");
            if (layers.Count <= 0)
                throw new ArgumentException("No layers", "layers");
            for (int i = 0; i < layers.Count; i++)
                if (layers[i].Length <= 0)
                    throw new ArgumentException(
                        String.Format("No neurons in layer {0}", i));
            _layers = layers;
        }

        public MlpNetwork(int inputCount, bool hasBias, int[] neuronCounts)
            : this(inputCount, hasBias)
        {
            _layers = new List<NonLinearNeuron[]>(neuronCounts.Length);
            for (int nLayer = 0; nLayer < neuronCounts.Length; nLayer++)
            {
                if (neuronCounts[nLayer] <= 0)
                    throw new ArgumentException(
                        String.Format("No neruons in layer {0}", nLayer));
                _layers.Add(new NonLinearNeuron[neuronCounts[nLayer]]);
            }
        }

        public MlpNetwork(int inputCount, bool hasBias, int[] neuronCounts,
            Type neuronType)
            : this(inputCount, hasBias, neuronCounts)
        {
            int biasInputs = hasBias ? 1 : 0;
            int neuronInputs = inputCount + biasInputs;
            foreach (Neuron[] layer in _layers)
            {
                for (int nNeuron = 0; nNeuron < layer.Length; nNeuron++)
                    layer[nNeuron] =
                        Activator.CreateInstance(neuronType, neuronInputs) as Neuron;
                neuronInputs = layer.Length + biasInputs;
            }
        }

        private int _inputCount;

        public int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }

        public int OutputCount
        {
            get { return _layers[_layers.Count - 1].Length; }
        }

        private IList<NonLinearNeuron[]> _layers;

        public IList<NonLinearNeuron[]> Layers
        {
            get { return _layers; }
            set { _layers = value; }
        }

        private bool _hasBias;

        public bool HasBias
        {
            get { return _hasBias; }
            set { _hasBias = value; }
        }

        private double[] AppendBias(double[] signals, bool lastLayer)
        {
            if (_hasBias && !lastLayer)
            {
                double[] result = new double[signals.Length + 1];
                Array.Copy(signals, result, signals.Length);
                result[signals.Length] = 1;
                return result;
            }
            else
                return signals;
        }

        public double[] Response(double[] inputSignals, double[][] layerResponses)
        {
            if (inputSignals == null || inputSignals.Length != _inputCount)
                throw new ArgumentException(
                    "The signal array's length must be equal to the number of inputs.");
            if (layerResponses != null && layerResponses.Length != _layers.Count)
                throw new ArgumentException(
                    "The layerResponses parameter should be null or contain the same " +
                    "number of elements as the Layers collection.");

            double[] signals = AppendBias(inputSignals, _layers.Count == 0);
            for (int nLayer = 0; nLayer < _layers.Count; nLayer++)
            {
                Neuron[] layer = _layers[nLayer];
                double[] layerResponse = new double[layer.Length];
                for (int nNeuron = 0; nNeuron < layer.Length; nNeuron++)
                    layerResponse[nNeuron] = layer[nNeuron].Response(signals);
                signals = AppendBias(layerResponse, nLayer == _layers.Count - 1);
                if (layerResponses != null)
                    layerResponses[nLayer] = layerResponse;
            }

            return signals;
        }

        public double[] Response(double[] inputSignals)
        {
            return Response(inputSignals, null);
        }

        public void Randomize(Random randomGenerator, double min, double max)
        {
            foreach (Neuron[] layer in _layers)
                foreach (Neuron n in layer)
                    n.Randomize(randomGenerator, min, max);
        }

        public void Randomize(Random randomGenerator, double min, double max,
            double epsilon)
        {
            foreach (Neuron[] layer in _layers)
                foreach (Neuron n in layer)
                    n.Randomize(randomGenerator, min, max, epsilon);
        }

        public enum LearningMethod { Perceptron, WidrowHoff };

        public void LearnSimple(TeachingSet.Element teachingElement, double ratio,
            ref double[] previousResponse, ref double[] previousError,
            LearningMethod method)
        {
            if (_layers.Count != 1)
                throw new InvalidOperationException(
                    "The simple learning algorithm can be applied only to one-layer networks.");

            NonLinearNeuron[] layer = _layers[0];
            if (previousResponse == null)
                previousResponse = new double[layer.Length];
            if (previousError == null)
                previousError = new double[layer.Length];
            double[] actualInputs = AppendBias(teachingElement.Inputs, false);

            for (int neuronIndex = 0; neuronIndex < layer.Length; neuronIndex++)
            {
                switch (method)
                {
                    case LearningMethod.Perceptron:
                        layer[neuronIndex].Learn(
                            actualInputs,
                            teachingElement.ExpectedOutputs[neuronIndex],
                            ratio,
                            out previousResponse[neuronIndex],
                            out previousError[neuronIndex]
                        );
                        break;
                    case LearningMethod.WidrowHoff:
                        layer[neuronIndex].LearnWidrowHoff(
                            actualInputs,
                            teachingElement.ExpectedOutputs[neuronIndex],
                            ratio,
                            out previousResponse[neuronIndex],
                            out previousError[neuronIndex]
                        );
                        break;
                }
            }
        }

        public void LearnSimple(TeachingSet.Element teachingElement, double ratio,
            ref double[] previousResponse, ref double[] previousError)
        {
            LearnSimple(teachingElement, ratio, ref previousResponse,
                ref previousError, LearningMethod.Perceptron);
        }

        public void LearnSimpleWidrowHoff(TeachingSet.Element teachingElement,
            double ratio, ref double[] previousResponse, ref double[] previousError)
        {
            LearnSimple(teachingElement, ratio, ref previousResponse,
                ref previousError, LearningMethod.WidrowHoff);
        }
    }
}

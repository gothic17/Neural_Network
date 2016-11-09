using System;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    /// <summary>
    /// This class represents a linear neuron.
    /// </summary>
    /// <remarks>
    /// A neuron is the basic component of a neural network. It's analogous to the
    /// biological neuron. Its basic property is a set (or a vector, from the
    /// mathematical point of view) of the remembered weights. Each weight is a number
    /// assigned to a particular input, describing how strongly (positively or
    /// negatively) the signal given to that input influences the neuron's response.
    /// </remarks>
    public class Neuron
    {
        /// <summary>
        /// Creates a neuron containing the given number of inputs.
        /// </summary>
        /// <param name="inputCount">The number of neuron's inputs. The weights array
        /// will have the size equal to the given number of inputs.</param>
        public Neuron(int inputCount)
        {
            _weights = new double[inputCount];
            _prevWeights = new double[inputCount];
        }

        // Stores the value of the Weights property.
        private double[] _weights;

        /// Przechowuje poprzednie wartoœæ w³asnoœci Weights.
        private double[] _prevWeights;

        /// <summary>
        /// Returns an array containing the weights of each of this neuron's inputs.
        /// </summary>
        public double[] Weights
        {
            get
            {
                return _weights;
            }
        }

        /// <summary>
        /// Tablica zawieraj¹ca poprzednie wagi poszczególnych wejœæ neuronu.
        /// </summary>
        public double[] PrevWeights
        {
            get
            {
                return _prevWeights;
            }
        }

        /// <summary>
        /// Computes the response of this neuron to the given signal.
        /// <remarks>
        /// To compute the response of a linear network, we need to calculate the
        /// product of each input signal component and its corresponding input weight.
        /// The sum of these products is our response. In practice it turns out that
        /// the more the signal is "similar" to the weights remembered by the neuron,
        /// the more positive the neuron's response will be.
        /// </remarks>
        /// <param name="inputSignals">The signal provided to the neuron inputs. Each
        /// element of the array corresponds to an individual input, so the array's
        /// length should math the number of inputs.</param>
        /// <returns>The neuron's response to the given signal.</returns>
        public virtual double Response(double[] inputSignals)
        {
            // Check if the argument is correct.
            if (inputSignals == null || inputSignals.Length != _weights.Length)
                throw new ArgumentException(
                    "The signal array must have the same length as the weight array.");

            // Calculate the output as the sum of products.
            double result = 0.0;
            for (int i = 0; i < _weights.Length; i++)
                result += _weights[i] * inputSignals[i];
            return result;
        }

        // response neuronu dla exampla 10c white
        public virtual double Response(double[] inputSignals, out double odl)
        {
            // Sprawdzamy, czy argument jest poprawny.
            if (inputSignals == null || inputSignals.Length != _weights.Length)
                throw new ArgumentException(
                    "The signal array must have the same length as the weight array.");

            // Obliczamy wyjœcie.
            double result = 0.0;
            odl = 0;
            for (int i = 0; i < _weights.Length; i++)
                odl += Math.Pow(_weights[i] - inputSignals[i], 2);

            result = 1 / (1E-10 + odl);
            return result;
        }

        /// <summary>
        /// Oblicza moc podanego sygna³u, u¿ywaj¹c zadanej miary.
        /// </summary>
        /// <param name="signals">Sygna³, którego moc obliczamy</param>
        /// <param name="norm">Norma, jak¹ mierzymy moc (euklidesowa lub manhattañska)
        /// </param>
        /// <returns>Obliczona moc</returns>
        /// <remarks>
        /// Moc sygna³u wp³ywa na to, jak bardzo "istotny" jest on dla neuronu.
        /// Jeœli zamiast sygna³u podamy wartoœci wspó³czynników wag, dostaniemy
        /// moc œladu pamiêciowego - czyli wartoœæ okreœlaj¹c¹, jak bardzo
        /// "zdecydowany" jest dany neuron.
        /// </remarks>
        /// <seealso cref="MemoryTraceStrength(StrengthNorm)"/>
        public static double Strength(double[] signals, StrengthNorm norm)
        {
            double strength = 0;
            switch (norm)
            {
                case StrengthNorm.Manhattan:
                    foreach (double s in signals)
                        strength += Math.Abs(s);
                    return strength;

                case StrengthNorm.Euclidean:
                    foreach (double s in signals)
                        strength += s * s;
                    return Math.Sqrt(strength);

                default:
                    return 0.0;
            }
        }

        /// <summary>
        /// Oblicza moc œladu pamiêciowego.
        /// </summary>
        /// <param name="norm">Norma, jak¹ mierzymy moc (euklidesowa lub manhattañska)
        /// </param>
        /// <returns>Obliczona moc œladu pamiêciowego</returns>
        /// <remarks>
        /// Moc œladu pamiêciowego okreœla, jak bardzo "zdecydowany" jest
        /// neuron. Im wiêksza moc œladu pamiêciowego, tym bardziej radykalne bêd¹
        /// jego "opinie".
        /// </remarks>
        public double MemoryTraceStrength(StrengthNorm norm)
        {
            return Strength(_weights, norm);
        }

        /// <summary>
        /// Normalizuje podany sygna³.
        /// </summary>
        /// <param name="signals">Sygna³ do normalizacji. Metoda zmienia zawartoœæ
        /// tej tablicy, wiêc aby zachowaæ orygina³, trzeba go najpierw gdzieœ
        /// skopiowaæ.</param>
        /// <remarks>
        /// Normalizacja polega na tym, ¿e wszystkie sk³adowe s¹ zmieniane tak, aby
        /// nie uleg³y zmianie ich proporcje, za to moc sygna³u wynios³a dok³adnie 1.
        /// Innymi s³owy, wektor sygna³u jest skalowany tak, aby mia³ d³ugoœæ 1 (wg
        /// normy Euklidesowej).
        /// </remarks>
        public static void Normalize(double[] signals)
        {
            double strength = Strength(signals, StrengthNorm.Euclidean);
            for (int i = 0; i < signals.Length; i++)
                signals[i] /= strength;
        }

        /// <summary>
        /// Uczy neuron reakcji na dany sygna³. Jest to "uczenie z nauczycielem".
        /// </summary>
        /// <param name="signals">Sygna³, na który neuron ma zareagowaæ.</param>
        /// <param name="expectedOutput">Prawid³owa odpowiedŸ neuronu.</param>
        /// <param name="ratio">Wspó³czynnik uczenia. Im wiêkszy, tym szybszy bêdzie
        /// proces uczenia, lecz mniejsza jego stabilnoœæ.</param>
        /// <param name="previousResponse">Parametr wyjœciowy; metoda umieszcza
        /// w nim odpowiedŸ neuronu na zadany sygna³ przed uczeniem.</param>
        /// <param name="previousError">Parametr wyjœciowy; metoda umieszcza w nim
        /// wielkoœæ b³êdu pope³nianego przez neuron przed uczeniem.</param>
        public void Learn(double[] signals, double expectedOutput, double ratio,
            out double previousResponse, out double previousError)
        {
            /* Nie sprawdzamy rozmiaru tablicy sygna³ów - Response() zrobi to
             * za nas. */
            previousResponse = Response(signals);
            previousError = expectedOutput - previousResponse;
            for (int i = 0; i < _weights.Length; i++)
                _weights[i] += ratio * previousError * signals[i];
        }

        public void Learn(double[] signals, double[] previous_weights, double error, double sigma, double ratio, double momentum)
        {
            for (int i = 0; i < _weights.Length; i++)
                _weights[i] += ratio * sigma * signals[i] - momentum * (_weights[i] - previous_weights[i]);
        }

        public void Randomize(Random randomGenerator, double min, double max)
        {
            double length = max - min;
            for (int i = 0; i < _weights.Length; i++)
                _weights[i] = min + length * randomGenerator.NextDouble();
        }

        /* dodane dla przykladu 10a,ax,b */
        public void Learn(double[] signals, double etha, double max)
        {

            double previous_response = Response(signals);

            if (previous_response < 0.2 * max)
                previous_response *= 0.3;
            if (previous_response < 0)
                previous_response *= 0.1;

            for (int i = 0; i < _weights.Length; i++)
            {
                _prevWeights[i] = _weights[i];
                _weights[i] += etha * previous_response * (signals[i] - _weights[i]);
            }
        }

        /* dodane dla przykladu 10c samouczenie z konkurencja */
        public void LearnSelf(double[] signals, double etha)
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                _prevWeights[i] = _weights[i];
                _weights[i] += etha * (signals[i] - _weights[i]);
            }
        }

        public void Randomize(Random randomGenerator, double min, double max, double epsilon)
        {
            double length = max - min;
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = min + length * randomGenerator.NextDouble();
                if (Math.Abs(_weights[i]) < epsilon)
                    _weights[i] = epsilon;
            }
        }

    }

    /// <summary>
    /// Rodzaje miar mocy sygna³u.
    /// </summary>
    public enum StrengthNorm
    { 

        /// <summary>
        /// Moc obliczana jako suma wartoœci bezwzglêdnych poszczególnych sk³adników
        /// sygna³u. Jej nazwa wziê³a siê st¹d, ¿e mo¿e ona s³u¿yæ do okreœlania
        /// d³ugoœci, jak¹ nale¿y przejechaæ w wielkim mieœcie miêdzy dwoma punktami,
        /// ale nie w linii prostej, tylko po prostopad³ych do siebie ulicach.
        /// </summary>
        Manhattan,

        /// <summary>
        /// Moc obliczana jako pierwiastek kwadratowy sumy kwadratów poszczególnych
        /// sk³adników sygna³u. Innymi s³owy - jest to odleg³oœæ w przestrzeni miêdzy
        /// pocz¹tkiem uk³adu wspó³rzêdnych a punktem o wspó³rzêdnych odpowiadaj¹cych
        /// sk³adnikom sygna³u.
        /// </summary>
        Euclidean
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;

namespace RTadeusiewicz.NN.NeuralNetworks
{
    /// <summary>
    /// Ta klasa reprezentuje zbiór ucz¹cy, u¿ywany do uczenia sieci.
    /// </summary>
    /// <remarks>
    /// Klasa ta jest po prostu list¹ elementów ucz¹cych, wzbogacon¹ o kilka
    /// u¿ytecznych metod pomocniczych.
    /// 
    /// Zbiór ucz¹cy mo¿e zostaæ wczytany z pliku. Plik ten musi mieæ nastêpuj¹cy
    /// format:
    /// 
    /// <code>
    /// 3, 2
    /// pierwszy obiekt
    /// 3, 4.5, 6.3
    /// 0.5, 0.23
    /// drugi obiekt
    /// 3, 2, 1
    /// 0, 0
    /// </code>
    /// 
    /// Pierwszy wiersz pliku zawiera kolejno iloœæ wejœæ i wyjœæ sieci neuronowej,
    /// dla której przeznaczony jest dany zbiór ucz¹cy. Kolejne linie opisuj¹
    /// poszczególne obiekty ci¹gu ucz¹cego. Opis ka¿dego obiektu sk³ada siê z:
    /// 
    /// <list type="bullet">
    /// <item><description>komentarza (nazwy obiektu)</description></item>
    /// <item><description>wartoœci wejœæ</description></item>
    /// <item><description>prawid³owych wartoœci wyjœæ</description></item>
    /// </list>
    /// 
    /// Iloœæ wartoœci wejœæ i wyjœæ dla ka¿dego obiektu musi byæ zgodna z wymiarami
    /// sieci podanymi w pierwszej linii.
    /// </remarks>
    public class TeachingSet : List<TeachingSet.Element>
    {
        /// <summary>
        /// Element ci¹gu ucz¹cego. Zawiera wejœcia, prawid³owe wyjœcie i komentarz
        /// (nazwê obiektu opisywanego przez wejœcia).
        /// </summary>
        public struct Element
        {
            /// <summary>
            /// Wartoœci sygna³ów, jakie powinny zostaæ podane na wejœcia sieci
            /// neuronowej. Mog¹ wymagaæ normalizacji.
            /// </summary>
            public double[] Inputs;

            /// <summary>
            /// Wartoœci, jakich oczekujemy na wyjœciu sieci neuronowej.
            /// </summary>
            public double[] ExpectedOutputs;

            /// <summary>
            /// Komentarz. Mo¿e byæ to nazwa obiektu reprezentowanego przez ten
            /// element.
            /// </summary>
            public string Comment;
            
            public Element Clone()
            {
                Element ret = this;
                ret.Inputs = (double[])Inputs.Clone();
                ret.ExpectedOutputs = (double[])ExpectedOutputs.Clone();
                return ret;
            }
        }

        /// <summary>
        /// Tworzy nowy zbiór ucz¹cy.
        /// </summary>
        /// <param name="inputCount">Iloœæ wejœæ sieci neuronowej, dla której
        /// przeznaczony jest ten zbiór.</param>
        /// <param name="outputCount">Iloœæ wyjœæ sieci neuronowej, dla której
        /// przeznaczony jest ten zbiór.</param>
        public TeachingSet(int inputCount, int outputCount)
        {
            _inputCount = inputCount;
            _outputCount = outputCount;
        }

        public TeachingSet(TeachingSet set): base(set.Count)
        {
            _inputCount = set._inputCount;
            _outputCount = set._outputCount;
            foreach (Element elem in set)
                Add(elem.Clone());
        }

        // Przechowuje wartoœæ w³asnoœci InputCount
        private int _inputCount;

        /// <summary>
        /// Iloœæ wejœæ sieci neuronowej, dla której przeznaczony jest ten zbiór.
        /// W³asnoœæ ta ma jedynie charakter informacyjny i wcale nie musi
        /// odzwierciedlaæ stanu faktycznego, sczególnie jeœli zosta³a zmieniona.
        /// Istnieje jedynie gwarancja, ¿e po odczytaniu zbioru z pliku w³asnoœæ
        /// ta bêdzie mia³a prawid³ow¹ wartoœæ.
        /// </summary>
        public int InputCount
        {
            get { return _inputCount; }
            set { _inputCount = value; }
        }

        // Przechowuje wartoœæ w³asnoœci OutputCount.
        private int _outputCount;

        /// <summary>
        /// Iloœæ wyjœæ sieci neuronowej, dla której przeznaczony jest ten zbiór.
        /// W³asnoœæ ta ma jedynie charakter informacyjny i wcale nie musi
        /// odzwierciedlaæ stanu faktycznego, sczególnie jeœli zosta³a zmieniona.
        /// Istnieje jedynie gwarancja, ¿e po odczytaniu zbioru z pliku w³asnoœæ
        /// ta bêdzie mia³a prawid³ow¹ wartoœæ.
        /// </summary>
        public int OutputCount
        {
            get { return _outputCount; }
            set { _outputCount = value; }
        }

        // Przetwarza pojedyncz¹ liniê z pliku opisuj¹cego zbiór ucz¹cy. Zwraca
        // tablicê liczb zawartych w tej linii lub wyrzuca wyj¹tek o podanej treœci,
        // jeœli ich iloœæ nie zgadza siê z oczekiwaniami.
        private static double[] ParseLine(TextReader reader, int expectedLength,
            string exceptionString)
        {
            /* Musimy to zrobiæ, aby format pliku by³ uniwersalny (ró¿ne narody
             * w ró¿ny sposób zapisuj¹ liczby dziesiêtne). */
            NumberFormatInfo invariantFormat =
                CultureInfo.InvariantCulture.NumberFormat;

            String line = reader.ReadLine();
            if (line == null)
                return null;
            if (line == "")
                throw new ApplicationException(
                    "Empty line encountered where the numbers were expected.");

            /* Rozbijamy liniê na liczby rozdzielone przecinkami. Spacje nam nie
             * przeszkadzaj¹. */
            string[] fields = line.Split(',');
            if (fields.Length != expectedLength)
                throw new ApplicationException(exceptionString);
            double[] ret = new double[expectedLength];
            for (int i = 0; i < fields.Length; i++)
                ret[i] = double.Parse(fields[i], invariantFormat);
            return ret;
        }

        /// <summary>
        /// Wczytuje zbiór ucz¹cy z pliku.
        /// </summary>
        /// <param name="fileName">Nazwa pliku.</param>
        /// <exception cref="ApplicationException">Plik ma nieprawid³owy format.
        /// </exception>
        /// <returns>Wczytany zbiór ucz¹cy lub <c>null</c>, jeœli plik jest pusty.
        /// </returns>
        public static TeachingSet FromFile(string fileName)
        {
            using (StreamReader sr = new StreamReader(fileName))
            {
                return TeachingSet.FromText(sr);
            }
        }

        /// <summary>
        /// Wczytuje zbiór ucz¹cy ze strumienia.
        /// </summary>
        /// <param name="stream">Strumieñ zawieraj¹cy zbiór ucz¹cy</param>
        /// <exception cref="ApplicationException">Strumieñ ma nieprawid³owy format.
        /// </exception>
        /// <returns>Wczytany zbiór ucz¹cy lub <c>null</c>, jeœli strumieñ jest pusty.
        /// </returns>
        public static TeachingSet FromStream(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                return TeachingSet.FromText(sr);
            }
        }

        /// <summary>
        /// Interpretuje podany tekst jako opis zbioru ucz¹cego.
        /// </summary>
        /// <param name="reader">Obiekt odczytuj¹cy tekst.</param>
        /// <exception cref="ApplicationException">Tekst ma nieprawid³owy format.
        /// </exception>
        /// <returns>Wczytany zbiór ucz¹cy lub <c>null</c>, jeœli tekst jest pusty.
        /// </returns>
        public static TeachingSet FromText(TextReader reader)
        {
            try
            {
                // Pierwsza linia - wymiary sieci.
                string line = reader.ReadLine();
                if (line == null)
                    return null;

                string[] fields = line.Split(',');
                if (fields.Length != 2)
                    throw new ApplicationException("First line must contain two numbers.");
                int inputCount = int.Parse(fields[0]);
                int outputCount = int.Parse(fields[1]);

                TeachingSet ret = new TeachingSet(inputCount, outputCount);

                // Poszczególne obiekty.
                while ((line = reader.ReadLine()) != null)
                {
                    Element elem;
                    elem.Comment = line;

                    elem.Inputs =
                        ParseLine(reader, inputCount, "Invalid number of inputs.");
                    if (elem.Inputs == null)
                        break;

                    elem.ExpectedOutputs =
                        ParseLine(reader, outputCount, "Invalid number of outputs.");
                    if (elem.ExpectedOutputs == null)
                        break;

                    ret.Add(elem);
                }
                return ret;
            }
            catch (ApplicationException) { throw; }
            catch (Exception ex)
            {
                throw new ApplicationException("The file is corrupted.", ex);
            }
        }

        public void Normalize()
        {
            foreach (Element elem in this)
                Neuron.Normalize(elem.Inputs);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorseCoder
{
    // Diccionario con caracters en morse

    public class Caracteres
    {
        public static Dictionary<string, string> PuntoLinea = new Dictionary<string, string>
        {
             // letras

            { "a", ".-"}, { "b", "-..."}, { "c", "-.-."}, { "d", "-.."}, { "e", "."},
            { "f", "..-."}, { "g", "--."}, { "h", "...."}, { "i", ".."}, { "j", ".---"}, 
            { "k", "-.-"}, { "l", ".-.."}, { "m", "--"}, { "n", "-."}, { "o", "---"}, 
            { "p", ".--."}, { "q", "--.-"}, { "r", ".-."}, { "s", "..."}, { "t", "-"}, 
            { "u", "..-"}, { "v", "..-"}, { "w", ".--"}, { "x", "-..-"}, { "y", "-.--"}, 
            { "z", "--.."},

            // numeros

            { "0", "-----"}, { "1", ".----"}, { "2", "..---"}, { "3", "...--"}, 
            { "4", "....-"}, { "5", "....."}, { "6", "-...."}, { "7", "--..."}, 
            { "8", "---.."}, { "9", "----."},

            // simbolos

            { ".", ".-.-.-"}, { ",", "--..--"}, { "?", "..--.."}, { "!", "-.-.--"}, 
            { "'", ".----."}, { "/", "-..-."}, { "(", "-.--."}, { ")", "-.--.-"}, 
            { ":", "---..."}, { ";", "-.-.-."}, { "=", "-...-"}, { "+", ""}, { "-", ""}, 
            { "$", "...-..-"}, { "@", ".--.-."}, { "_", "..--.-"}, { " ", " "}
        };

        public static string CoderTranslate(string word)
        {
            word = word.ToLower();
            string result = "";

            foreach (var Caracter in word)
            {   
                result += Caracteres.PuntoLinea[Convert.ToString(Caracter)] + "|";
            }
            int Separator = result.LastIndexOf("|");
            result = result.Remove(Separator);
            return result;
        }

        public static string Null_Exception(string nuller)
        {
            if (string.IsNullOrEmpty(nuller))
            {
                throw new ArgumentNullException();
            }
            return nuller;
        }
    }

    class TextMorseConverter
    {
        public int RapidezCaracter { get; set; }
        public int RapidezPalabra { get; set; }
        public double Frecuencia { get; set; }

        public TextMorseConverter(int RapCar, int RapPal, double Frec)
        {
            RapidezCaracter = RapCar;
            RapidezPalabra = RapPal;
            Frecuencia = Frec;
        }

        public TextMorseConverter(int RapCar, int RapPal) : this(RapCar, RapPal, 600) { }
        public TextMorseConverter(int wpm) : this(wpm, wpm) { }
        public TextMorseConverter() : this(20) { }

        private short[] TimeProgress(double segundos)
        {
            short[] ProgressArray;
            int pruebas = (int)(11025 * segundos);
            ProgressArray = new short[pruebas];

            for (int i = 0; i < pruebas; i++)
            {
                ProgressArray[i] = Convert.ToInt16(32760 * Math.Sin(i * 2 * Math.PI * Frecuencia / 11025));
            }
            return ProgressArray;
        }

        private short[] TimeCompress(double segundos)
        {
            short[] ProgressArray;
            int pruebas = (int)(11025 * segundos);
            ProgressArray = new short[pruebas];
            return ProgressArray;
        }

        private short[] Punto()
        {
            return TimeProgress(1.2 / RapidezCaracter);
        }

        private short[] Linea()
        {
            return TimeProgress(3.6 / RapidezCaracter);
        }

        private short[] Espacio()
        {
            return TimeCompress(1.2 / RapidezCaracter);
        }

        private short[] DemoraCaracter()
        {
            double pausa = (60 / RapidezPalabra) - (32 / RapidezCaracter);
            double distancia = 3 * pausa / 19;
            return TimeCompress(distancia);
        }

        private short[] DemoraPalabra()
        {
            double pausa = (60 / RapidezPalabra) - (32 / RapidezCaracter);
            double distancia = 7 * pausa / 19;
            return TimeCompress(distancia);
        }

        private short[] CaracteresMorse(string caracter)
        {
            short[] espacio = Espacio();
            short[] punto = Punto();
            short[] linea = Linea();

            List<short> morse = new List<short>();

            string SimbolosMorse = Caracteres.PuntoLinea[caracter];

            for(int i =0; i < SimbolosMorse.Length; i++)
            {
                if(i > 0)
                {
                    morse.AddRange(espacio);
                }
                if (SimbolosMorse[i] == '-')
                {
                    morse.AddRange(linea);
                }
                else if (SimbolosMorse[i] == '.')
                {
                    morse.AddRange(punto);
                }
            }
            return morse.ToArray<short>();
        }

        private short[] PalabrasMorse(string palabra)
        {
            List<short> dato = new List<short>();

            for(int i = 0; i < palabra.Length; i++)
            {
                if(i > 0)
                {
                    dato.AddRange(DemoraCaracter());
                }
                if(palabra[i] == '<')
                {
                    int final = palabra.IndexOf('>', i);

                    if (final < 0)
                    {
                        throw new ArgumentException();
                    }
                    dato.AddRange(CaracteresMorse(palabra.Substring(i, final + 1 - i)));
                }
                else
                {
                    dato.AddRange(CaracteresMorse(palabra[i].ToString()));
                }
            }
            return dato.ToArray<short>();
        }

        private DC TextoMorse(string texto)
        {
            List<short> dato = new List<short>();

            string[] palabras = texto.Split(' ');

            for(int i = 0; i < palabras.Length; i++)
            {
                if (i < 0)
                {
                    dato.AddRange(DemoraPalabra());
                }
                dato.AddRange(PalabrasMorse(palabras[i]));
            }

            dato.AddRange(DemoraCaracter());

            DC dc = new DC(dato.ToArray<short>());

            return dc;
        }

        public byte[] Conversion_Morse(string texto)
        {
            DC dato = TextoMorse(texto.ToLower());
            FC fc = new FC();
            HC hc = new HC(fc, dato);
            return hc.ToBytes();
        }
    }

    // Arreglos extra

    abstract class WC
    {
        public char[] CId { get; set; }
        public uint CSize { get; set; }
        public abstract byte[] ToBytes();
    }

    class DC : WC
    {
        short[] CData {get; set;}

        public DC(short[] datos)
        {
            CId = "datos".ToCharArray();
            CSize = (uint)(datos.Length * 2);
            CData = datos;
        }

        public override byte[] ToBytes()
        {
            List<byte> bits = new List<byte>();

            bits.AddRange(Encoding.UTF8.GetBytes(CId));
            bits.AddRange(BitConverter.GetBytes(CSize));

            foreach (short Datom in CData)
            {
                bits.AddRange(BitConverter.GetBytes(Datom));
            }
            return bits.ToArray<byte>();
        }
    }

    class FC : WC
    {
        short CodigoCompreso { get; set; }
        short NumeroCanal { get; set; }
        short Alineacion { get; set; }
        short BitsImportantes { get; set; }
        uint BytesPorSegundo { get; set; }
        uint RangoPrueba { get; set; }

        public FC()
        {
            CId = "fmt ".ToCharArray();
            CSize = 16;
            CodigoCompreso = 1;
            NumeroCanal = 1;
            RangoPrueba = 11025;
            BytesPorSegundo = 22050;
            Alineacion = 2;
            BitsImportantes = 16;
        }

        public override byte[] ToBytes()
        {
            List<byte> bits = new List<byte>();

            bits.AddRange(Encoding.UTF8.GetBytes(CId));
            bits.AddRange(BitConverter.GetBytes(CSize));
            bits.AddRange(BitConverter.GetBytes(CodigoCompreso));
            bits.AddRange(BitConverter.GetBytes(NumeroCanal));
            bits.AddRange(BitConverter.GetBytes(RangoPrueba));
            bits.AddRange(BitConverter.GetBytes(BytesPorSegundo));
            bits.AddRange(BitConverter.GetBytes(Alineacion));
            bits.AddRange(BitConverter.GetBytes(BitsImportantes));

            return bits.ToArray<byte>();
        }
    }

    class HC : WC
    {
        public char[] frontera { get; set; }
        public FC FC { get; set; }
        public DC DC { get; set; }

        public HC(FC fc, DC dc)
        {
            CId = "RIFF".ToCharArray();
            frontera = "WAVE".ToCharArray();
            FC = fc;
            DC = dc;
            CSize = 36 + DC.CSize;
        }

        public override byte[] ToBytes()
        {
            List<byte> bits = new List<byte>();

            bits.AddRange(Encoding.UTF8.GetBytes(CId));
            bits.AddRange(BitConverter.GetBytes(CSize));
            bits.AddRange(Encoding.UTF8.GetBytes(frontera));
            bits.AddRange(FC.ToBytes());
            bits.AddRange(DC.ToBytes());

            return bits.ToArray<byte>();
        }
    }
    
}

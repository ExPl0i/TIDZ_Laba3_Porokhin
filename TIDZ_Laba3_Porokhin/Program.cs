namespace TIDZ_Laba3_Porokhin
{
    internal class Program
    {
        /// <summary>
        /// Представляет источники сообщений (источники S)
        /// </summary>
        class SourceS
        {
            const int m = 8; // Число состояний марковской цепи
            string[] s;      // Массив состояний
            double[,] p;     // Матрица переходных вероятностей 
            int[] f;         // Массив частот появления состояний
            Random r;        // Случайное поле
            int k;           // Индекс текущего состояния

            /// <summary>
            /// Конструктор с параметрами по умолчанию
            /// </summary>
            public SourceS()
            {
                s = new string[m] { "s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8" };
                p = new double[m, m] { {0.05, 0.1, 0.21, 0.06, 0.1, 0.14, 0.25, 0.9},
                                    {0.02, 0.2, 0.08, 0.2, 0.27, 0.1, 0.08, 0.05},
                                    {0.11, 0.03, 0.3, 0.09, 0.17, 0.04, 0.2, 0.06},
                                    {0.09, 0.28, 0.1, 0.09, 0.08, 0.1, 0.06, 0.2},
                                    {0.13, 0.1, 0.08, 0.2, 0.04, 0.2, 0.18, 0.07},
                                    {0.25, 0.07, 0.09, 0.2, 0.15, 0.1, 0.04, 0.1},
                                    {0.24, 0.03, 0.1, 0.19, 0.2, 0.06, 0.05, 0.13},
                                    {0.11, 0.23, 0.08, 0.13, 0.08, 0.2, 0.1, 0.07}};
                f = new int[m];
                r = new Random();
                k = r.Next(0, m - 1);
            }

            /// <summary>
            /// Генерирует случайным образом состояние цепи и возвращает его
            /// </summary>
            /// <returns>Состояние</returns>
            public string GenerSymbol()
            {
                string st = ""; // Текущее состояние цепи
                double x = r.NextDouble();
                double q = 0.0; // Кумулятивная вероятность
                for (int i = 0; i < m; i++)
                {
                    // Проверяем принадлежность x определенному
                    // интервалу переходных вероятностей
                    if ((x > q) && (x <= q + p[k, i]))
                    {
                        st = s[i];
                        k = i;
                        f[i]++;
                        break;
                    }
                    q += p[k, i];
                }
                return st;
            }

            /// <summary>
            /// Возвращает строку с частотами появления состояний
            /// </summary>
            /// <returns>Частоты появления состояний</returns>
            public string GetFreqs()
            {
                string buf = "\nЧастоты появления состояний: \n";
                for (int i = 0; i < m; i++)
                {
                    buf += string.Format("s{0} - {1}\n", i + 1, f[i]);
                }
                return buf;
            }
        }

        /// <summary>
        /// Представляет источники сообщений (источники U), статистически
        /// связанные с другими источниками (источники S)
        /// </summary>
        class SourceU
        {
            const int mu = 6; // Объем алфавита источника сообщений U
            const int ms = 8; // Объем алфавита источника сообщений S
            string[] u;       // Алфавит источника сообщений U
            double[,] p;      // Матрица переходных вероятностей 
            int[] f;          // Массив частот появления символов
            Random rnd;

            /// <summary>
            /// Конструктор с параметрами по умолчанию
            /// </summary>
            public SourceU()
            {
                u = new string[mu] { "u1", "u2", "u3", "u4", "u5", "u6" };
                p = new double[ms, mu] { {0.24, 0.09, 0.15, 0.27, 0.06, 0.19},
                                         {0, 0.35, 0.08, 0.16, 0.31, 0.1},
                                         {0.23, 0.14, 0, 0.34, 0.18, 0.11},
                                         {0.07, 0.26, 0.28, 0.21, 0.15, 0.03},
                                         {0.16, 0.18, 0.22, 0.17, 0.2, 0.07},
                                         {0.3, 0.06, 0.34, 0, 0.09, 0.21},
                                         {0.19, 0.24, 0.16, 0.04, 0.25, 0.12},
                                         {0.14, 0.19, 0, 0.25, 0.07, 0.35}};
                f = new int[mu];
                rnd = new Random();
            }

            /// <summary>
            /// Генерирует случайным образом символ и возвращает его
            /// </summary>
            /// <param name="si">Символ источника S</param>
            /// <returns>Символ источника U</returns>
            public string GenerSymbol(string si)
            {
                string uj = ""; // Текущий символ источника U
                int i = Convert.ToInt32(si.Remove(0, 1)) - 1; // Индекс источника S
                double q = 0.0; // Нижняя граница интервала вероятностей
                double r = rnd.NextDouble();
                for (int j = 0; j < mu; j++)
                {
                    if ((r > q) && (r <= q + p[i, j]))
                    {
                        uj = u[j]; f[j]++; break;
                    }
                    q += p[i, j];
                }
                return uj;
            }

            /// <summary>
            /// Возвращает строку с частотами появления символов
            /// </summary>
            /// <returns>Частоты появления симоволов</returns>
            public string GetFreqs()
            {
                string buf = "\nЧастоты появления символов:\n";
                for (int j = 0; j < mu; j++)
                {
                    buf += string.Format("{0} - {1}\n", u[j], f[j]);
                }
                return buf;
            }
        }

        /// <summary>
        /// Представляет кодер источника сообщений
        /// </summary>
        class SourcEncoder
        {
            int errNum; // Число ошибок кодирования

            /// <summary>
            /// Выполняет простое кодирование символов источника
            /// </summary>
            /// <param name="_s">Символ источника</param>
            /// <returns>Кодовое слово</returns>
            public string SimpleEncoding(string _s)
            {
                string c = ""; // Кодовое слово
                switch (_s)
                {
                    case "s1": c = "00"; break;
                    case "s2": c = "01"; break;
                    case "s3": c = "10"; break;
                    case "s4": c = "11"; break;
                    case "s5": c = "100"; break;
                    case "s6": c = "101"; break;
                    case "s7": c = "110"; break;
                    case "s8": c = "111"; break;
                    default: c = "><"; errNum++; break; // Знак ошибки
                }
                return c;
            }

            /// <summary>
            /// Возвращает строку с данными об ошибках декодирования
            /// </summary>
            /// <returns>Данные об ошибках</returns>
            public string GetErrorNumber()
            {
                return string.Format("Число ошибок кодирования: {0}", errNum);
            }
        }

        /// <summary>
        /// Представляет декодер источника сообщений
        /// </summary>
        class SourceDecoder
        {
            int errNum; // Число ошибок декодирования

            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            public SourceDecoder()
            {
                this.errNum = 0;
            }

            /// <summary>
            /// Выполняет простое декодирование кодовых слов
            /// </summary>
            /// <param name="_c">Кодовое слово</param>
            /// <returns>Символ источника</returns>
            public string SimpleDecoding(string _c)
            {
                string s = ""; // Символ
                switch (_c)
                {
                    case "00": s = "s1"; break;
                    case "01": s = "s2"; break;
                    case "10": s = "s3"; break;
                    case "11": s = "s4"; break;
                    case "100": s = "s5"; break;
                    case "101": s = "s6"; break;
                    case "110": s = "s7"; break;
                    case "111": s = "s8"; break;
                    default: s = "><"; errNum++; break; // Знак ошибки
                }
                return s;
            }

            /// <summary>
            /// Возвращает строку с данными об ошибках декодирования
            /// </summary>
            /// <returns>Данные об ошибках</returns>
            public string GetErrorNumber()
            {
                return string.Format("Число ошибок декодирования: {0}", errNum);
            }
        }

        /// <summary>
        /// Представляет двоичный канал связи со стиранием (ДКС)
        /// </summary>
        class BinaryChanel
        {
            const int m = 2; // Объем входного алфавита ДКС
            const int n = 3; // Объем выходного алфавита
            string[] x; // Входной алфавит ДКС
            string[] y; // Выходной алфавит ДКС
            double p; // Вероятность ошибки при передаче символа через ДКС
            double e; // Вероятность стирания символа при передаче через ДКС
            double[,] pM; // Матрица переходных вероятностей ДКС
            int errNum; // Число ошибок при передаче через ДКС
            Random rnd;

            /// <summary>
            /// Конструктор с параметрами по умолчанию
            /// </summary>
            public BinaryChanel()
            {
                x = new string[m] { "0", "1" };
                y = new string[n] { "0", " ", "1" };
                p = 0.02;
                e = 0.01;
                pM = new double[m, n] { { 1 - p - e, e, p },
                                       { p, e, 1 - e } };
                errNum = 0;
                rnd = new Random();
            }

            /// <summary>
            /// Возвращает переданную двоичную последователность
            /// </summary>
            /// <param name="cx">Входная последовательность</param>
            /// <returns>Выходная последовательность</returns>
            public string TransmitBits(string cx)
            {
                string cy = ""; // Выходная последовательность
                double r = rnd.NextDouble();
                double q = 0.0; // Кумулятивная вероятность
                int i = 0; // Индекс символа входного алфавита

                for (int k = 0; k < cx.Length; k++)
                {
                    i = Convert.ToInt32(cx.Substring(k, 1));
                    for (int j = 0; j < n; j++)
                    {
                        if (r > q && (r <= q + pM[i, j]))
                        {
                            cy += y[j];
                            if (x[i] != y[j]) errNum++; // Подсчет ошибок
                            break;
                        }
                        q += pM[i, j];
                    }
                }
                return cy;
            }

            /// <summary>
            /// Возвращает строку с данными об ошибках при передачи
            /// </summary>
            /// <returns>Данные об ошибках</returns>
            public string GetErrorNumber()
            {
                return string.Format("Число ошибок при передаче: {0}", errNum);
            }
        }

        /// <summary>
        /// Представляет кодер канала связи (ККС)
        /// </summary>
        class ChannelEncoder
        {
            /// <summary>
            /// Выполяет кодирование с битом четности
            /// </summary>
            /// <param name="cx">Входная последовательность</param>
            /// <returns>Кодовое слово с битом четности</returns>
            public string ParityBitEncoding(string cx)
            {
                int n = cx.Length; // Длина входной последовательности
                int u = 0; // Число единиц во входной последовательности
                for (int i = 0; i < n; i++)
                {
                    // Проверить, является ли символ единицей
                    if (cx.Substring(i, 1) == "1") u++;
                }
                // Проверить, является ли число единиц четным
                if (u % 2 == 0) cx += "0"; // Добавить бит четности
                else cx += "1";

                return cx;
            }
        }

        /// <summary>
        /// Представляет декодер канала связи
        /// </summary>
        class ChannelDecoder
        {
            int errNum; // Число ошибок при декодировании

            /// <summary>
            /// Конструктор по умолчанию
            /// </summary>
            public ChannelDecoder()
            {
                this.errNum = 0;
            }

            /// <summary>
            /// Декодирует кодовые слова с битом четности
            /// </summary>
            /// <param name="cx">Входная последовательность</param>
            /// <returns>Выходная последовательность</returns>
            public string ParityDitDecoding(string cx)
            {
                int n = cx.Length; // Длина входной последовательности
                int u = 0; // Число единиц во входной последовательности
                for (int i = 0; i < n; i++)
                {
                    if (cx.Substring(i, 1) == "1") u++;
                }
                // Проверить, является ли число единиц четным
                if (u % 2 == 0)
                {
                    cx = cx.Remove(n - 1, 1); // Удалить бит четности
                }
                else
                {
                    errNum++;
                    cx = "111"; // Ошибочная последовательность
                }

                return cx;
            }

            /// <summary>
            /// Возвращает строку с данными об ошибках декодирования
            /// </summary>
            /// <returns></returns>
            public string GetErrorNumber()
            {
                return string.Format("Число обнаруженных ошибок: {0}", errNum);
            }
        }

        static void Main(string[] args)
        {
            Console.Title = "Моделирование двоичного канала связи";

            // Экземпляр класса марковский источник сообщений (ИС)
            SourceS mSrc = new SourceS();
            // Экземпляр класса кодер источника сообщений (КИС)
            SourcEncoder sEnc = new SourcEncoder();
            // Экземпляр класса кдер канала связи (ККС)
            ChannelEncoder cEnc = new ChannelEncoder();
            // Экземпляр класса двоичный канал связи
            BinaryChanel bChn = new BinaryChanel();
            // Экземпляр класса декодер канала связи (ДКС)
            ChannelDecoder cDec = new ChannelDecoder();
            // Экземпляр класса декодер источника сообщений (ДИС)
            SourceDecoder sDec = new SourceDecoder();

            Console.WriteLine("Введите число генирируемых символов:");
            string buf = Console.ReadLine();
            int n = int.Parse((buf));

            string src = ""; // Сообщение на выходе источника (ИС)
            string enc = ""; // Сообщение на выходе кодера источника (КИС)
            string chn = ""; // Сообщение на выходе КС
            string dec = ""; // Сообщение на выходе декодера источника (ДИС)
            buf = "|{0,5}|{1,7}|{2,7}|{3,7}|{4,7}|"; // Строка форматного вывода

            Console.WriteLine(buf, "И", "ИС", "КИС", "КС", "ДИС");
            for (int i = 0; i < n; i++)
            {
                src = mSrc.GenerSymbol();
                enc = sEnc.SimpleEncoding(src);
                dec = sDec.SimpleDecoding(enc);
                chn = bChn.TransmitBits(enc);
                Console.WriteLine(buf, i + 1, src, enc, chn, dec);
            }

            Console.WriteLine(mSrc.GetFreqs());
            Console.WriteLine(sEnc.GetErrorNumber());
            Console.WriteLine(sDec.GetErrorNumber());

            Console.Title = "Моделирование системы передачи сообщений";

            Console.WriteLine();

            Console.WriteLine("Введите число генерируемых символов:");
            buf = Console.ReadLine();
            n = int.Parse(buf); // Число генерируемых символов

            src = ""; // Сообщение на выходе ИС
            string sen = ""; // Сообщение на выходе КИС
            string cen = ""; // Сообщение на выходе ККС
            chn = ""; // Сообщение на выходе КС
            string cdc = ""; // Сообщение на выходе ДКС
            string sdc = ""; // Сообщение на выходе ДИС
            // Строка форматного вывода
            buf = "|{0, 5}|{1,7}|{2,7}|{3,7}|{4,7}|{5,7}|{6,7}|";

            Console.WriteLine("\nСообщения на выходе элементов системы:");
            Console.WriteLine(buf, "N", "ИС", "КИС", "ККС", "КС", "ДКС", "ДИС");
            for (int i = 0; i < n; i++)
            {
                src = mSrc.GenerSymbol();
                sen = sEnc.SimpleEncoding(src);
                cen = cEnc.ParityBitEncoding(sen);
                chn = bChn.TransmitBits(cen);
                cdc = cDec.ParityDitDecoding(chn);
                sdc = sDec.SimpleDecoding(cdc);
                Console.WriteLine(buf, i + 1, src, sen, cen, chn, cdc, sdc);
            }

            Console.WriteLine(mSrc.GetFreqs());
            Console.WriteLine(bChn.GetErrorNumber());
            Console.WriteLine(cDec.GetErrorNumber());
            Console.WriteLine(sDec.GetErrorNumber());
            Console.Read();

        }
    }
}

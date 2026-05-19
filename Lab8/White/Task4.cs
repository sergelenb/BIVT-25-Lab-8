namespace Lab8.White
{
    public class Task4
    {
        public class Human
        {
            // имя человека
            private string name;
            // фамилия человека
            private string surname;
            
            public string Name => name;
            public string Surname => surname;

            // конструктор человека
            public Human(string name, string surname)
            {
                this.name = name;
                this.surname = surname;
            }

            // виртуальный метод вывода
            public virtual void Print()
            {
                Console.WriteLine($"{Name} {Surname}");
            }
        }

        public class Participant : Human
        {
            // общее количество участников
            private static int count;
            // массив очков за партии
            private double[] scores;
            // сколько матчей уже записано
            private int matchCount;
            // свойство количества участников
            public static int Count => count;
            // массив результатов
            public double[] Scores => scores;
            // сумма всех очков
            public double TotalScore
            {
                get
                {
                    if (scores == null)
                        return 0;
                    double sum = 0;

                    foreach (double score in scores)
                    {
                        if (score != -1)
                            sum += score;
                    }

                    return sum;
                }
            }

            // статический конструктор
            static Participant()
            {
                count = 0;
            }

            // конструктор участника
            public Participant(string name, string surname)
                : base(name, surname)
            {
                scores = new double[10];

                for (int i = 0; i < scores.Length; i++)
                {
                    scores[i] = -1;
                }

                matchCount = 0;
                count++;
            }

            // добавляет результат новой партии
            public void PlayMatch(double result)
            {
                if (scores == null)
                {
                    scores = new double[10];

                    for (int i = 0; i < scores.Length; i++)
                    {
                        scores[i] = -1;
                    }
                }

                if (matchCount < scores.Length)
                {
                    scores[matchCount] = result;
                    matchCount++;
                }
            }

            // сортировка по убыванию очков
            public static void Sort(Participant[] array)
            {
                Array.Sort(array,
                    (a, b) => b.TotalScore.CompareTo(a.TotalScore));
            }

            // переопределённый вывод
            public override void Print()
            {
                Console.WriteLine(
                    $"{Name} {Surname} {TotalScore}");
            }
        }
    }
}

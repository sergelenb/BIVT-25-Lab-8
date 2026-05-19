namespace Lab8.White
{
    public class Task2
    {
        public class Participant
        {
            // норматив для прыжка
            private static readonly double _standard;
            // имя участника
            private string name;
            // фамилия участника
            private string surname;
            // первый прыжок
            private double firstJump;
            // второй прыжок
            private double secondJump;
            // сколько прыжков уже записано
            private int jumpCount;
            
            public string Name => name;
            public string Surname => surname;
            public double FirstJump => firstJump;
            public double SecondJump => secondJump;

            // лучший прыжок
            public double BestJump => Math.Max(firstJump, secondJump);
            // прошёл ли участник норматив
            public bool IsPassed => BestJump >= _standard;
            // статический конструктор
            static Participant()
            {
                // норматив = 3
                _standard = 3;
            }

            // конструктор без прыжков
            public Participant(string name, string surname)
            {
                this.name = name;
                this.surname = surname;
                firstJump = 0;
                secondJump = 0;
                jumpCount = 0;
            }

            // конструктор сразу с двумя прыжками
            public Participant(
                string name,
                string surname,
                double firstJump,
                double secondJump)
            {
                this.name = name;
                this.surname = surname;
                this.firstJump = firstJump;
                this.secondJump = secondJump;
                jumpCount = 2;
            }

            // записывает только первые 2 прыжка
            public void Jump(double result)
            {
                if (jumpCount == 0)
                {
                    firstJump = result;
                    jumpCount++;
                }
                else if (jumpCount == 1)
                {
                    secondJump = result;
                    jumpCount++;
                }

                // если попытки уже есть — ничего не меняем
            }

            // сортировка по лучшему прыжку
            public static void Sort(Participant[] array)
            {
                Array.Sort(array,
                    (a, b) => b.BestJump.CompareTo(a.BestJump));
            }

            // возвращает только прошедших норматив
            public static Participant[] GetPassed(Participant[] participants)
            {
                if (participants == null)
                    return new Participant[0];

                int count = 0;

                foreach (Participant participant in participants)
                {
                    if (participant != null && participant.IsPassed)
                        count++;
                }

                Participant[] passed = new Participant[count];

                int index = 0;

                foreach (Participant participant in participants)
                {
                    if (participant != null && participant.IsPassed)
                    {
                        passed[index] = participant;
                        index++;
                    }
                }

                return passed;
            }

            // вывод информации
            public void Print()
            {
                Console.WriteLine(
                    $"{Name} {Surname} " +
                    $"{FirstJump} {SecondJump} " +
                    $"{BestJump} {IsPassed}");
            }
        }
    } 
    
}

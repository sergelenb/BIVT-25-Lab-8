namespace Lab8.White
{
    public class Task1
    {
        public class Participant
        {
            // норматив
            private static double _standard;
            // количество участников
            private static int _jumpers;
            // количество дисквалифицированных
            private static int _disqualified;
            // фамилия
            private string surname;
            // клуб
            private string club;
            // первый прыжок
            private double firstJump;
            // второй прыжок
            private double secondJump;
            // сколько прыжков уже есть
            private int jumpCount;
            
            public string Surname => surname;
            public string Club => club;
            public double FirstJump => firstJump;
            public double SecondJump => secondJump;

            // сумма прыжков
            public double JumpSum => firstJump + secondJump;
            // количество участников
            public static int Jumpers => _jumpers;
            // количество дисквалифицированных
            public static int Disqualified => _disqualified;
            // статический конструктор
            static Participant()
            {
                // норматив = 5
                _standard = 5;
                // в начале никого нет
                _jumpers = 0;
                _disqualified = 0;
            }

            // обычный конструктор
            public Participant(string surname, string club)
            {
                this.surname = surname;
                this.club = club;
                firstJump = 0;
                secondJump = 0;
                jumpCount = 0;
                // новый участник
                _jumpers++;
            }

            // записывает прыжки
            public void Jump(double result)
            {
                // первый прыжок
                if (jumpCount == 0)
                {
                    firstJump = result;
                    jumpCount++;
                }
                // второй прыжок
                else if (jumpCount == 1)
                {
                    secondJump = result;
                    jumpCount++;
                }
                // больше 2 попыток нельзя
            }

            // сортировка по сумме прыжков
            public static void Sort(Participant[] array)
            {
                Array.Sort(array,
                    (a, b) => b.JumpSum.CompareTo(a.JumpSum));
            }

            // дисквалификация
            public static void Disqualify(ref Participant[] participants)
            {
                if (participants == null)
                    return;
                int count = 0;

                // считаем кто прошёл норматив
                foreach (Participant participant in participants)
                {
                    if (participant != null &&
                        participant.firstJump >= _standard &&
                        participant.secondJump >= _standard)
                    {
                        count++;
                    }
                }

                Participant[] passed = new Participant[count];
                int index = 0;

                foreach (Participant participant in participants)
                {
                    if (participant != null &&
                        participant.firstJump >= _standard &&
                        participant.secondJump >= _standard)
                    {
                        passed[index] = participant;
                        index++;
                    }
                    else
                    {
                        // увеличиваем дисквалификацию
                        _disqualified++;
                    }
                }

                participants = passed;
            }

            // вывод информации
            public void Print()
            {
                Console.WriteLine(
                    $"{Surname} {Club} " +
                    $"{FirstJump} {SecondJump} " +
                    $"{JumpSum}");
            }
        }
    
    }
}

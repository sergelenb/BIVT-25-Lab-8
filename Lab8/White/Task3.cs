namespace Lab8.White
{
    public class Task3
    {
        public class Student
        {
            // имя студента
            private string name;
            // фамилия студента
            private string surname;
            // protected — чтобы наследник Undergraduate мог работать с оценками
            protected int[] marks;
            // protected — чтобы наследник мог работать с пропусками
            protected int skipped;
            // сколько оценок уже записано
            private int markCount;
            
            public string Name => name;
            public string Surname => surname;
            public int Skipped => skipped;

            // средняя оценка
            public double AverageMark
            {
                get
                {
                    if (marks == null || markCount == 0)
                        return 0;
                    int sum = 0;
                    for (int i = 0; i < markCount; i++)
                    {
                        sum += marks[i];
                    }
                    return (double)sum / markCount;
                }
            }

            // обычный конструктор
            public Student(string name, string surname)
            {
                this.name = name;
                this.surname = surname;

                marks = new int[10];
                skipped = 0;
                markCount = 0;
            }

            // защищённый конструктор копирования
            protected Student(Student student)
            {
                name = student.Name;
                surname = student.Surname;
                skipped = student.Skipped;
                marks = new int[10];
                markCount = 0;

                if (student.marks != null)
                {
                    for (int i = 0; i < student.marks.Length; i++)
                    {
                        if (student.marks[i] != 0)
                        {
                            marks[markCount] = student.marks[i];
                            markCount++;
                        }
                    }
                }
            }

            // добавляет оценку или пропуск
            public void Lesson(int mark)
            {
                // 0 — это пропуск
                if (mark == 0)
                {
                    skipped++;
                }
                else
                {
                    if (marks == null)
                        marks = new int[10];

                    if (markCount < marks.Length)
                    {
                        marks[markCount] = mark;
                        markCount++;
                    }
                }
            }

            // сортировка по пропускам
            public static void SortBySkipped(Student[] array)
            {
                Array.Sort(array,
                    (a, b) => b.Skipped.CompareTo(a.Skipped));
            }

            // вывод информации
            public void Print()
            {
                Console.WriteLine(
                    $"{Name} {Surname} " +
                    $"{AverageMark:F2} {Skipped}");
            }
        }

        public class Undergraduate : Student
        {
            // конструктор по имени и фамилии
            public Undergraduate(string name, string surname)
                : base(name, surname)
            {
            }

            // конструктор из обычного Student
            public Undergraduate(Student student)
                : base(student)
            {
            }

            // отработка пропуска или двойки
            public void WorkOff(int mark)
            {
                // если есть пропуск — уменьшаем пропуск
                // и добавляем новую оценку через Lesson
                if (skipped > 0)
                {
                    skipped--;
                    Lesson(mark);
                }
                else
                {
                    // если пропусков нет — ищем 2 и заменяем
                    if (marks == null)
                        return;

                    for (int i = 0; i < marks.Length; i++)
                    {
                        if (marks[i] == 2)
                        {
                            marks[i] = mark;
                            return;
                        }
                    }
                }
            }

            // вывод информации
            public new void Print()
            {
                Console.WriteLine(
                    $"{Name} {Surname} " +
                    $"{AverageMark:F2} {Skipped}");
            }
        }
    
    }
}

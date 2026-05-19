namespace Lab8.White
{
    public class Task5
    {
        public struct Match
        {
            // забитые голы
            private int goals;
            // пропущенные голы
            private int misses;
            
            public int Goals => goals;
            public int Misses => misses;
            // разница голов
            public int Difference => goals - misses;

            // очки за матч
            public int Score
            {
                get
                {
                    if (goals > misses)
                        return 3;

                    if (goals == misses)
                        return 1;

                    return 0;
                }
            }

            // конструктор матча
            public Match(int goals, int misses)
            {
                this.goals = goals;
                this.misses = misses;
            }

            // вывод матча
            public void Print()
            {
                Console.WriteLine($"{Goals}:{Misses} {Difference} {Score}");
            }
        }

        public abstract class Team
        {
            // название команды
            private string name;
            // массив матчей
            private Match[] matches;

            
            public string Name => name;
            public Match[] Matches => matches;
            // общая разница голов
            public int TotalDifference
            {
                get
                {
                    if (matches == null)
                        return 0;
                    int sum = 0;

                    foreach (Match match in matches)
                    {
                        sum += match.Difference;
                    }

                    return sum;
                }
            }

            // общее количество очков
            public int TotalScore
            {
                get
                {
                    if (matches == null)
                        return 0;

                    int sum = 0;

                    foreach (Match match in matches)
                    {
                        sum += match.Score;
                    }

                    return sum;
                }
            }

            // конструктор базовой команды
            protected Team(string name)
            {
                this.name = name;
                matches = new Match[0];
            }

            // виртуальный метод добавления матча
            public virtual void PlayMatch(int goals, int misses)
            {
                if (matches == null)
                    matches = new Match[0];

                Array.Resize(ref matches, matches.Length + 1);
                matches[matches.Length - 1] = new Match(goals, misses);
            }

            // сортировка команд
            public static void SortTeams(Team[] teams)
            {
                Array.Sort(teams, (a, b) =>
                {
                    int byScore = b.TotalScore.CompareTo(a.TotalScore);

                    if (byScore != 0)
                        return byScore;

                    return b.TotalDifference.CompareTo(a.TotalDifference);
                });
            }

            // вывод информации
            public void Print()
            {
                Console.WriteLine($"{Name} {TotalScore} {TotalDifference}");
            }
        }

        public class ManTeam : Team
        {
            // команда-дерби
            private ManTeam derby;
            // свойство команды-дерби
            public ManTeam Derby => derby;
            // конструктор мужской команды
            public ManTeam(string name, ManTeam derby = null)
                : base(name)
            {
                this.derby = derby;
            }

            // матч мужской команды
            public void PlayMatch(int goals, int misses, ManTeam team = null)
            {
                // если игра против дерби-команды,
                // добавляем +1 забитый гол
                if (team != null && team == derby)
                {
                    goals++;
                }
                base.PlayMatch(goals, misses);
            }
        }

        public class WomanTeam : Team
        {
            // массив штрафов
            private int[] penalties;
            // свойство штрафов
            public int[] Penalties => penalties;
            // сумма штрафов
            public int TotalPenalties
            {
                get
                {
                    if (penalties == null)
                        return 0;
                    int sum = 0;

                    foreach (int penalty in penalties)
                    {
                        sum += penalty;
                    }
                    return sum;
                }
            }

            // конструктор женской команды
            public WomanTeam(string name)
                : base(name)
            {
                penalties = new int[0];
            }

            // переопределённый матч
            public override void PlayMatch(int goals, int misses)
            {
                // сначала обычный матч
                base.PlayMatch(goals, misses);

                // если пропущено больше, чем забито — добавляем штраф
                if (misses > goals)
                {
                    if (penalties == null)
                        penalties = new int[0];

                    Array.Resize(ref penalties, penalties.Length + 1);
                    penalties[penalties.Length - 1] = misses - goals;
                }
            }

            // вывод информации
            public new void Print()
            {
                Console.WriteLine(
                    $"{Name} {TotalScore} {TotalDifference} {TotalPenalties}");
            }
        }
    
    }
}

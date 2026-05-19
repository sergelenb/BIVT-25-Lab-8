using System.Reflection;
using System.Text.Json;

namespace Lab8Test.White
{
   [TestClass]
   public sealed class Task5
   {
       record InputMatch(int Goals, int Misses);
       record InputTeam(string Name, InputMatch[] Matches);
       record OutputTeam(string Name, double TotalScore, double TotalDifference);

       private InputTeam[] _input;
       private OutputTeam[] _output;

       private Lab8.White.Task5.Team[] _teams;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory())
               .Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab8Test", "White");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _input = input.GetProperty("Task5").Deserialize<InputTeam[]>();
           _output = output.GetProperty("Task5").Deserialize<OutputTeam[]>();

           _teams = new Lab8.White.Task5.Team[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var match = typeof(Lab8.White.Task5.Match);
           Assert.IsTrue(match.IsValueType);
           Assert.AreEqual(0, match.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);
           Assert.IsNotNull(match.GetProperty("Goals"));
           Assert.IsNotNull(match.GetProperty("Misses"));
           Assert.IsNotNull(match.GetProperty("Difference"));
           Assert.IsNotNull(match.GetProperty("Score"));
           Assert.IsFalse(match.GetProperty("Goals")?.CanWrite ?? true);
           Assert.IsNotNull(match.GetConstructor(new[] { typeof(int), typeof(int) }));
           Assert.IsNotNull(match.GetMethod("Print"));

           var team = typeof(Lab8.White.Task5.Team);
           Assert.IsTrue(team.IsAbstract);
           Assert.IsTrue(team.IsClass);
           Assert.AreEqual(0, team.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);
           Assert.IsNotNull(team.GetProperty("Name"));
           Assert.IsNotNull(team.GetProperty("Matches"));
           Assert.IsNotNull(team.GetProperty("TotalScore"));
           Assert.IsNotNull(team.GetProperty("TotalDifference"));
           Assert.IsNotNull(team.GetConstructor(new[] { typeof(string) }));

           var play = team.GetMethod("PlayMatch");
           Assert.IsTrue(play.IsVirtual);

           Assert.IsNotNull(team.GetMethod("SortTeams", BindingFlags.Static | BindingFlags.Public));

           var man = typeof(Lab8.White.Task5.ManTeam);
           Assert.IsTrue(man.IsSubclassOf(team));
           Assert.IsNotNull(man.GetProperty("Derby"));
           Assert.IsNotNull(man.GetConstructor(new[] { typeof(string), typeof(Lab8.White.Task5.ManTeam) }));
           Assert.IsNotNull(man.GetMethod("PlayMatch",
               new[] { typeof(int), typeof(int), typeof(Lab8.White.Task5.ManTeam) }));

           var woman = typeof(Lab8.White.Task5.WomanTeam);
           Assert.IsTrue(woman.IsSubclassOf(team));
           Assert.IsNotNull(woman.GetProperty("Penalties"));
           Assert.IsNotNull(woman.GetProperty("TotalPenalties"));

           var overridePlay = woman.GetMethod("PlayMatch",
               new[] { typeof(int), typeof(int) });
           Assert.AreNotEqual(team, overridePlay.DeclaringType);

           Assert.AreEqual(0, match.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(4, match.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, match.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(match.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 9);

           Assert.AreEqual(0, team.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(4, team.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, team.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(team.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 10);

           Assert.AreEqual(0, man.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(4, man.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, man.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(man.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 12);

           Assert.AreEqual(0, woman.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(6, woman.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, woman.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(woman.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 12);
       }

       [TestMethod]
       public void Test_01_Create()
       {
           Init();
           Check(false);
       }

       [TestMethod]
       public void Test_02_PlayMatches()
       {
           Init();
           PlayMatches();
           Check(true);
       }

       [TestMethod]
       public void Test_03_Sort()
       {
           Init();
           PlayMatches();
           Lab8.White.Task5.Team.SortTeams(_teams);
           CheckSorted();
       }

       [TestMethod]
       public void Test_04_ManTeam_DerbyBonus()
       {
           var a = new Lab8.White.Task5.ManTeam("A");
           var b = new Lab8.White.Task5.ManTeam("B", a);

           b.PlayMatch(1, 1, a);

           Assert.AreEqual(3, b.TotalScore);
       }

       [TestMethod]
       public void Test_05_WomanTeam_Penalties()
       {
           var t = new Lab8.White.Task5.WomanTeam("W");

           t.PlayMatch(1, 4);
           t.PlayMatch(2, 0);

           Assert.AreEqual(1, t.Penalties.Length);
           Assert.AreEqual(3, t.TotalPenalties);
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               if (i % 2 == 0)
                   _teams[i] = new Lab8.White.Task5.ManTeam(_input[i].Name);
               else
                   _teams[i] = new Lab8.White.Task5.WomanTeam(_input[i].Name);
           }
       }

       private void PlayMatches()
       {
           for (int i = 0; i < _input.Length; i++)
               foreach (var m in _input[i].Matches)
                   _teams[i].PlayMatch(m.Goals, m.Misses);
       }

       private void Check(bool played)
       {
           for (int i = 0; i < _teams.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _teams[i].Name);

               if (played)
               {
                   Assert.AreEqual(
                       _input[i].Matches.Sum(m =>
                           m.Goals > m.Misses ? 3 :
                           m.Goals == m.Misses ? 1 : 0),
                       _teams[i].TotalScore);
               }
           }
       }

       private void CheckSorted()
       {
           for (int i = 0; i < _teams.Length; i++)
           {
               Assert.AreEqual(_output[i].Name, _teams[i].Name);
               Assert.AreEqual(_output[i].TotalScore, _teams[i].TotalScore, 0.0001);
               Assert.AreEqual(_output[i].TotalDifference, _teams[i].TotalDifference, 0.0001);
           }
       }
   }
}

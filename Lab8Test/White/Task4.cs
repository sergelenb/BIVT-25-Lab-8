using System.Reflection;
using System.Text.Json;

namespace Lab8Test.White
{
   [TestClass]
   public sealed class Task4
   {
       record InputRow(string Name, string Surname, double[] Scores);
       record OutputRow(string Name, string Surname, double TotalScore);

       private InputRow[] _input;
       private OutputRow[] _output;

       private Lab8.White.Task4.Participant[] _participants;

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

           _input = input.GetProperty("Task4").Deserialize<InputRow[]>();
           _output = output.GetProperty("Task4").Deserialize<OutputRow[]>();

           _participants = new Lab8.White.Task4.Participant[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var human = typeof(Lab8.White.Task4.Human);
           var participant = typeof(Lab8.White.Task4.Participant);

           Assert.IsTrue(human.IsClass);
           Assert.IsTrue(participant.IsClass);
           Assert.IsTrue(participant.IsSubclassOf(human));

           Assert.AreEqual(0, human.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);
           Assert.AreEqual(0, participant.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

           Assert.IsNotNull(human.GetProperty("Name")?.CanRead);
           Assert.IsNotNull(human.GetProperty("Surname")?.CanRead);
           Assert.IsFalse(human.GetProperty("Name")?.CanWrite ?? true);
           Assert.IsFalse(human.GetProperty("Surname")?.CanWrite ?? true);

           Assert.IsNotNull(participant.GetProperty("Scores")?.CanRead);
           Assert.IsNotNull(participant.GetProperty("TotalScore")?.CanRead);
           Assert.IsNotNull(participant.GetProperty("Count")?.CanRead);

           Assert.IsFalse(participant.GetProperty("Scores")?.CanWrite ?? true);
           Assert.IsFalse(participant.GetProperty("TotalScore")?.CanWrite ?? true);
           Assert.IsFalse(participant.GetProperty("Count")?.CanWrite ?? true);

           Assert.IsNotNull(human.GetConstructor(
               BindingFlags.Instance | BindingFlags.Public,
               null,
               new[] { typeof(string), typeof(string) },
               null));

           Assert.IsNotNull(participant.GetConstructor(
               BindingFlags.Instance | BindingFlags.Public,
               null,
               new[] { typeof(string), typeof(string) },
               null));

           Assert.IsNotNull(human.GetMethod("Print"));
           Assert.IsTrue(human.GetMethod("Print").IsVirtual);

           Assert.IsNotNull(participant.GetMethod("Print"));
           Assert.IsTrue(participant.GetMethod("Print").IsVirtual);
           Assert.AreNotEqual(
               human.GetMethod("Print").DeclaringType,
               participant.GetMethod("Print").DeclaringType);

           Assert.IsNotNull(participant.GetMethod("PlayMatch"));
           Assert.IsNotNull(participant.GetMethod("Sort"));

           Assert.AreEqual(0, human.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(2, human.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, human.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(human.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 7);

           Assert.AreEqual(0, participant.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(5, participant.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, participant.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(participant.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 10);

       }

       [TestMethod]
       public void Test_01_Create()
       {
           Init();
           CheckBase(justNames: true);
           Assert.AreEqual(_participants.Length, Lab8.White.Task4.Participant.Count);
       }

       [TestMethod]
       public void Test_02_PlayMatches()
       {
           Init();
           PlayMatches();
           CheckBase(justNames: false);
       }

       [TestMethod]
       public void Test_03_Sort()
       {
           Init();
           PlayMatches();
           Lab8.White.Task4.Participant.Sort(_participants);
           CheckSorted();
       }

       [TestMethod]
       public void Test_04_ArrayIsolation()
       {
           Init();
           PlayMatches();

           for (int i = 0; i < _participants.Length; i++)
           {
               var scores = _participants[i].Scores;
               if (scores != null && scores.Length > 0)
                   scores[0] = -100;
           }

           CheckBase(justNames: false);
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
               _participants[i] =
                   new Lab8.White.Task4.Participant(_input[i].Name, _input[i].Surname);
       }

       private void PlayMatches()
       {
           for (int i = 0; i < _input.Length; i++)
               foreach (var score in _input[i].Scores)
                   _participants[i].PlayMatch(score);
       }

       private void CheckBase(bool justNames)
       {
           for (int i = 0; i < _participants.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _participants[i].Name);
               Assert.AreEqual(_input[i].Surname, _participants[i].Surname);

               if (!justNames)
               {
                   Assert.AreEqual(
                       _input[i].Scores.Sum(),
                       _participants[i].TotalScore,
                       0.0001);

                   Assert.AreEqual(
                       _input[i].Scores.Length,
                       _participants[i].Scores.Length);

                   for (int j = 0; j < _participants[i].Scores.Length; j++)
                       Assert.AreEqual(
                           _input[i].Scores[j],
                           _participants[i].Scores[j],
                           0.0001);
               }
           }
       }

       private void CheckSorted()
       {
           for (int i = 0; i < _participants.Length; i++)
           {
               Assert.AreEqual(_output[i].Name, _participants[i].Name);
               Assert.AreEqual(_output[i].Surname, _participants[i].Surname);
               Assert.AreEqual(_output[i].TotalScore, _participants[i].TotalScore, 0.0001);
           }
       }
   }
}

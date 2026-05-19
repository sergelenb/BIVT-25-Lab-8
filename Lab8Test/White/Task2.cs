using System.Reflection;
using System.Text.Json;

namespace Lab8Test.White
{
   [TestClass]
   public sealed class Task2
   {
       record InputRow(string Name, string Surname, double FirstJump, double SecondJump);
       record OutputRow(string Name, string Surname, double BestJump);

       private InputRow[] _input;
       private OutputRow[] _output;
       private Lab8.White.Task2.Participant[] _student;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab8Test", "White");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _input = input.GetProperty("Task2").Deserialize<InputRow[]>();
           _output = output.GetProperty("Task2").Deserialize<OutputRow[]>();
           _student = new Lab8.White.Task2.Participant[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var type = typeof(Lab8.White.Task2.Participant);

           Assert.IsTrue(type.IsClass);
           Assert.IsFalse(type.IsValueType);

           Assert.IsTrue(type.GetProperty("Name")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("FirstJump")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("SecondJump")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("BestJump")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("IsPassed")?.CanRead ?? false);

           Assert.IsFalse(type.GetProperty("Name")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("FirstJump")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("SecondJump")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("BestJump")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("IsPassed")?.CanWrite ?? true);

           Assert.IsNotNull(type.TypeInitializer);

           Assert.IsNotNull(
               type.GetConstructor(
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   new[] { typeof(string), typeof(string) },
                   null));

           Assert.IsNotNull(
               type.GetConstructor(
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   new[] { typeof(string), typeof(string), typeof(double), typeof(double) },
                   null));

           Assert.IsNotNull(
               type.GetMethod(
                   "Jump",
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   new[] { typeof(double) },
                   null));

           Assert.IsNotNull(
               type.GetMethod(
                   "Sort",
                   BindingFlags.Static | BindingFlags.Public,
                   null,
                   new[] { typeof(Lab8.White.Task2.Participant[]) },
                   null));

           Assert.IsNotNull(
               type.GetMethod(
                   "GetPassed",
                   BindingFlags.Static | BindingFlags.Public,
                   null,
                   new[] { typeof(Lab8.White.Task2.Participant[]) },
                   null));

           Assert.IsNotNull(
               type.GetMethod(
                   "Print",
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   Type.EmptyTypes,
                   null));

           Assert.AreEqual(0, type.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(6, type.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(2, type.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 12);
       }

       [TestMethod]
       public void Test_01_Create()
       {
           Init();

           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _student[i].Name);
               Assert.AreEqual(_input[i].Surname, _student[i].Surname);
               Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
               Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
               Assert.AreEqual(0, _student[i].BestJump, 0.0001);
               Assert.IsFalse(_student[i].IsPassed);
           }
       }

       [TestMethod]
       public void Test_02_Init()
       {
           Init();
           CheckParticipants(false);
       }

       [TestMethod]
       public void Test_03_Jumps()
       {
           Init();
           Jump();
           CheckParticipants(true);
       }

       [TestMethod]
       public void Test_04_Sort()
       {
           Init();
           Jump();

           Lab8.White.Task2.Participant.Sort(_student);

           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_output[i].Name, _student[i].Name);
               Assert.AreEqual(_output[i].Surname, _student[i].Surname);
               Assert.AreEqual(_output[i].BestJump, _student[i].BestJump, 0.01);
           }
       }

       [TestMethod]
       public void Test_05_GetPassed()
       {
           Init();
           Jump();

           var passed = Lab8.White.Task2.Participant.GetPassed(_student);

           foreach (var p in passed)
               Assert.IsTrue(p.IsPassed);

           Assert.IsTrue(passed.All(p => p.BestJump >= 3));
       }

       [TestMethod]
       public void Test_06_ConstructorWithJumps()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               var p = new Lab8.White.Task2.Participant(
                   _input[i].Name,
                   _input[i].Surname,
                   _input[i].FirstJump,
                   _input[i].SecondJump);

               Assert.AreEqual(_input[i].FirstJump, p.FirstJump, 0.0001);
               Assert.AreEqual(_input[i].SecondJump, p.SecondJump, 0.0001);
               Assert.AreEqual(
                   Math.Max(_input[i].FirstJump, _input[i].SecondJump),
                   p.BestJump,
                   0.0001);
           }
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
               _student[i] = new Lab8.White.Task2.Participant(
                   _input[i].Name, _input[i].Surname);
       }

       private void Jump()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               _student[i].Jump(_input[i].FirstJump);
               _student[i].Jump(_input[i].SecondJump);
               _student[i].Jump(-1);
           }
       }

       private void CheckParticipants(bool jumpsExpected)
       {
           for (int i = 0; i < _input.Length; i++)
           {
               if (!jumpsExpected)
               {
                   Assert.AreEqual(0, _student[i].FirstJump, 0.0001);
                   Assert.AreEqual(0, _student[i].SecondJump, 0.0001);
                   Assert.AreEqual(0, _student[i].BestJump, 0.0001);
                   Assert.IsFalse(_student[i].IsPassed);
               }
               else
               {
                   Assert.AreEqual(_input[i].FirstJump, _student[i].FirstJump, 0.0001);
                   Assert.AreEqual(_input[i].SecondJump, _student[i].SecondJump, 0.0001);
                   Assert.AreEqual(
                       Math.Max(_input[i].FirstJump, _input[i].SecondJump),
                       _student[i].BestJump,
                       0.0001);
               }
           }
       }
   }
}

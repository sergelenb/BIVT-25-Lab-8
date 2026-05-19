using System.Reflection;
using System.Text.Json;

namespace Lab8Test.White
{
   [TestClass]
   public sealed class Task1
   {
       record InputRow(string Surname, string Club, double FirstJump, double SecondJump);
       record OutputRow(string Surname, string Club, double JumpSum);

       private InputRow[] _input;
       private OutputRow[] _output;
       private Lab8.White.Task1.Participant[] _student;

       [TestInitialize]
       public void LoadData()
       {
           var folder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
           folder = Path.Combine(folder, "Lab8Test", "White");

           var input = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "input.json")))!;
           var output = JsonSerializer.Deserialize<JsonElement>(
               File.ReadAllText(Path.Combine(folder, "output.json")))!;

           _input = input.GetProperty("Task1").Deserialize<InputRow[]>();
           _output = output.GetProperty("Task1").Deserialize<OutputRow[]>();
           _student = new Lab8.White.Task1.Participant[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var type = typeof(Lab8.White.Task1.Participant);

           Assert.IsFalse(type.IsValueType);
           Assert.IsTrue(type.IsClass);

           Assert.IsTrue(type.GetProperty("Surname")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("Club")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("FirstJump")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("SecondJump")?.CanRead ?? false);
           Assert.IsTrue(type.GetProperty("JumpSum")?.CanRead ?? false);

           Assert.IsFalse(type.GetProperty("Surname")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("Club")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("FirstJump")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("SecondJump")?.CanWrite ?? true);
           Assert.IsFalse(type.GetProperty("JumpSum")?.CanWrite ?? true);

           Assert.IsTrue(type.GetProperty("Jumpers")?.GetMethod?.IsStatic ?? false);
           Assert.IsTrue(type.GetProperty("Disqualified")?.GetMethod?.IsStatic ?? false);

           Assert.IsNotNull(
               type.GetConstructor(
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   new[] { typeof(string), typeof(string) },
                   null));

           Assert.IsNotNull(type.TypeInitializer);

           Assert.IsNotNull(
               type.GetMethod("Jump",
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   new[] { typeof(double) },
                   null));

           Assert.IsNotNull(
               type.GetMethod("Sort",
                   BindingFlags.Static | BindingFlags.Public,
                   null,
                   new[] { typeof(Lab8.White.Task1.Participant[]) },
                   null));

           Assert.IsNotNull(
               type.GetMethod("Disqualify",
                   BindingFlags.Static | BindingFlags.Public,
                   null,
                   new[] { typeof(Lab8.White.Task1.Participant[]).MakeByRefType() },
                   null));

           Assert.IsNotNull(
               type.GetMethod("Print",
                   BindingFlags.Instance | BindingFlags.Public,
                   null,
                   Type.EmptyTypes,
                   null));

           Assert.AreEqual(0, type.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(7, type.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, type.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 11);
       }

       [TestMethod]
       public void Test_01_Create()
       {
           Init();

           Assert.AreEqual(_input.Length,
               Lab8.White.Task1.Participant.Jumpers);

           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_input[i].Surname, _student[i].Surname);
               Assert.AreEqual(_input[i].Club, _student[i].Club);
               Assert.AreEqual(0, _student[i].FirstJump);
               Assert.AreEqual(0, _student[i].SecondJump);
               Assert.AreEqual(0, _student[i].JumpSum);
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

           Lab8.White.Task1.Participant.Sort(_student);

           for (int i = 0; i < _student.Length; i++)
           {
               Assert.AreEqual(_output[i].Surname, _student[i].Surname);
               Assert.AreEqual(_output[i].Club, _student[i].Club);
               Assert.AreEqual(_output[i].JumpSum, _student[i].JumpSum, 0.01);
           }
       }

       [TestMethod]
       public void Test_05_Disqualify()
       {
           var hpType = typeof(Lab8.White.Task1.Participant);
           var sumFi = hpType.GetField("_jumpers", BindingFlags.Static | BindingFlags.NonPublic);
           sumFi?.SetValue(null, 0);

           Init();
           Jump();


           int before = _student.Length;

           Lab8.White.Task1.Participant.Disqualify(ref _student);

           Assert.IsTrue(_student.Length < before);
           Assert.AreEqual(
               before - _student.Length,
               Lab8.White.Task1.Participant.Disqualified);


           Assert.AreEqual(_student.Length, Lab8.White.Task1.Participant.Jumpers);

           foreach (var s in _student)
           {
               Assert.IsTrue(s.FirstJump >= 5 && s.SecondJump >= 5);
           }
       }

       private void Init()
       {
           for (int i = 0; i < _input.Length; i++)
               _student[i] = new Lab8.White.Task1.Participant(_input[i].Surname, _input[i].Club);
       }

       private void Jump()
       {
           for (int i = 0; i < _input.Length; i++)
           {
               _student[i].Jump(_input[i].FirstJump);
               _student[i].Jump(_input[i].SecondJump);
               _student[i].Jump(1);
           }
       }

       private void CheckParticipants(bool jumpsExpected)
       {
           for (int i = 0; i < _input.Length; i++)
           {
               if (!jumpsExpected)
               {
                   Assert.AreEqual(0, _student[i].FirstJump, $"at {_student[i].Surname} in {_student[i].Club}");
                   Assert.AreEqual(0, _student[i].SecondJump, $"at {_student[i].Surname} in {_student[i].Club}");
                   Assert.AreEqual(0, _student[i].JumpSum, $"at {_student[i].Surname} in {_student[i].Club}");
               }
               else
               {
                   Assert.AreEqual(_input[i].FirstJump, _student[i].FirstJump, 0.0001, $"at {_student[i].Surname} in {_student[i].Club}");
                   Assert.AreEqual(_input[i].SecondJump, _student[i].SecondJump, 0.0001, $"at {_student[i].Surname} in {_student[i].Club}");
                   Assert.AreEqual(_input[i].FirstJump + _input[i].SecondJump, _student[i].JumpSum, 0.0001, $"at {_student[i].Surname} in {_student[i].Club}");
               }
           }
       }
   }
}

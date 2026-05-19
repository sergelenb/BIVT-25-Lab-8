using System.Reflection;
using System.Text.Json;

namespace Lab8Test.White
{
   [TestClass]
   public sealed class Task3
   {
       record InputRow(string Name, string Surname, int[] Marks);
       record OutputStudent(string Name, string Surname, double AverageMark, int Skipped);
       record OutputUndergraduate(string Name, string Surname, double AverageMark, int Skipped);

       private InputRow[] _input;
       private OutputStudent[] _outStudent;
       private OutputUndergraduate[] _outUndergraduate;
       private OutputStudent[] _outSorted;
       private OutputStudent[] _outWorkedOff;

       private Lab8.White.Task3.Student[] _students;
       private Lab8.White.Task3.Undergraduate[] _undergraduates;

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

           _input = input.GetProperty("Task3").Deserialize<InputRow[]>();
           _outStudent = output.GetProperty("Task3Student").Deserialize<OutputStudent[]>();
           _outUndergraduate = output.GetProperty("Task3Undergraduate").Deserialize<OutputUndergraduate[]>();
           _outSorted = output.GetProperty("Task3Sorted").Deserialize<OutputStudent[]>();
           _outWorkedOff = output.GetProperty("Task3WorkedOff").Deserialize<OutputStudent[]>();

           _students = new Lab8.White.Task3.Student[_input.Length];
           _undergraduates = new Lab8.White.Task3.Undergraduate[_input.Length];
       }

       [TestMethod]
       public void Test_00_OOP()
       {
           var student = typeof(Lab8.White.Task3.Student);
           var undergrad = typeof(Lab8.White.Task3.Undergraduate);

           Assert.IsTrue(student.IsClass);
           Assert.IsTrue(undergrad.IsClass);
           Assert.IsTrue(undergrad.IsSubclassOf(student));

           Assert.AreEqual(0, student.GetFields(BindingFlags.Public | BindingFlags.Instance).Length);

           Assert.IsNotNull(student.GetProperty("Name")?.CanRead);
           Assert.IsNotNull(student.GetProperty("Surname")?.CanRead);
           Assert.IsNotNull(student.GetProperty("AverageMark")?.CanRead);
           Assert.IsNotNull(student.GetProperty("Skipped")?.CanRead);

           Assert.IsFalse(student.GetProperty("Name")?.CanWrite ?? true);
           Assert.IsFalse(student.GetProperty("Surname")?.CanWrite ?? true);
           Assert.IsFalse(student.GetProperty("AverageMark")?.CanWrite ?? true);
           Assert.IsFalse(student.GetProperty("Skipped")?.CanWrite ?? true);

           var marks = student.GetField("_marks", BindingFlags.Instance | BindingFlags.NonPublic);
           var skipped = student.GetField("_skipped", BindingFlags.Instance | BindingFlags.NonPublic);

           Assert.IsTrue(marks.IsFamily);
           Assert.IsTrue(skipped.IsFamily);

           Assert.IsNotNull(student.GetConstructor(
               BindingFlags.Instance | BindingFlags.Public,
               null,
               new[] { typeof(string), typeof(string) },
               null));

           Assert.IsNotNull(student.GetConstructor(
               BindingFlags.Instance | BindingFlags.NonPublic,
               null,
               new[] { student },
               null));

           Assert.IsNotNull(student.GetMethod("Lesson"));
           Assert.IsNotNull(student.GetMethod("SortBySkipped"));
           Assert.IsNotNull(student.GetMethod("Print"));

           Assert.IsNotNull(undergrad.GetConstructor(
               new[] { typeof(string), typeof(string) }));

           Assert.IsNotNull(undergrad.GetConstructor(
               new[] { student }));

           Assert.IsNotNull(undergrad.GetMethod("WorkOff"));
           Assert.IsNotNull(undergrad.GetMethod("Print"));


           Assert.AreEqual(0, student.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(4, student.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(1, student.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(student.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 10);

           Assert.AreEqual(0, undergrad.GetFields().Count(f => f.IsPublic));
           Assert.AreEqual(4, undergrad.GetProperties().Count(f => f.PropertyType.IsPublic));
           Assert.AreEqual(2, undergrad.GetConstructors().Count(f => f.IsPublic));
           Assert.AreEqual(undergrad.GetMethods(BindingFlags.Instance | BindingFlags.Public).Length, 12);
       }

       [TestMethod]
       public void Test_01_CreateStudent()
       {
           InitStudents();
           CheckStudentBase();
       }

       [TestMethod]
       public void Test_02_LessonsStudent()
       {
           InitStudents();
           LessonsStudents();
           CheckStudentFull();
       }

       [TestMethod]
       public void Test_03_SortStudent()
       {
           InitStudents();
           LessonsStudents();
           Lab8.White.Task3.Student.SortBySkipped(_students);
           CheckStudentSorted();
       }

       [TestMethod]
       public void Test_04_CreateUndergraduate()
       {
           InitUndergraduates();
           CheckUndergraduateBase();
       }

       [TestMethod]
       public void Test_05_WorkOff()
       {
           InitUndergraduates();
           LessonsUndergraduates();

           for (int i = 0; i < _undergraduates.Length; i++)
               _undergraduates[i].WorkOff(5);

           CheckUndergraduateFull();
       }

       private void InitStudents()
       {
           for (int i = 0; i < _input.Length; i++)
               _students[i] = new Lab8.White.Task3.Student(_input[i].Name, _input[i].Surname);
       }

       private void InitUndergraduates()
       {
           for (int i = 0; i < _input.Length; i++)
               _undergraduates[i] = new Lab8.White.Task3.Undergraduate(_input[i].Name, _input[i].Surname);
       }

       private void LessonsStudents()
       {
           for (int i = 0; i < _input.Length; i++)
               foreach (var m in _input[i].Marks)
                   _students[i].Lesson(m);
       }

       private void LessonsUndergraduates()
       {
           for (int i = 0; i < _input.Length; i++)
               foreach (var m in _input[i].Marks)
                   _undergraduates[i].Lesson(m);
       }

       private void CheckStudentBase()
       {
           for (int i = 0; i < _students.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _students[i].Name);
               Assert.AreEqual(_input[i].Surname, _students[i].Surname);
               Assert.AreEqual(0, _students[i].AverageMark, 0.001);
               Assert.AreEqual(0, _students[i].Skipped);
           }
       }

       private void CheckStudentFull()
       {
           for (int i = 0; i < _students.Length; i++)
           {
               Assert.AreEqual(_outStudent[i].AverageMark, _students[i].AverageMark, 0.01);
               Assert.AreEqual(_outStudent[i].Skipped, _students[i].Skipped);
           }
       }

       private void CheckStudentSorted()
       {
           for (int i = 0; i < _students.Length; i++)
           {
               Assert.AreEqual(_outSorted[i].Name, _students[i].Name);
               Assert.AreEqual(_outSorted[i].Surname, _students[i].Surname);
               Assert.AreEqual(_outSorted[i].Skipped, _students[i].Skipped);
           }
       }

       private void CheckUndergraduateBase()
       {
           for (int i = 0; i < _undergraduates.Length; i++)
           {
               Assert.AreEqual(_input[i].Name, _undergraduates[i].Name);
               Assert.AreEqual(_input[i].Surname, _undergraduates[i].Surname);
           }
       }

       private void CheckUndergraduateFull()
       {
           for (int i = 0; i < _undergraduates.Length; i++)
           {
                   Assert.AreEqual(_outWorkedOff[i].AverageMark, _undergraduates[i].AverageMark, 0.01);
                   Assert.AreEqual(_outWorkedOff[i].Skipped, _undergraduates[i].Skipped);
           }
       }
   }
}

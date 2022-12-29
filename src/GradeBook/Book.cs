namespace GradeBook
{
    public delegate void GradeAddedDelegate(object sender, EventArgs args);

    public class NamedObject
    {
        public NamedObject(string name)
        {
            Name = name;
        }

        public string Name
        {

            get
            {
                return Name;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Name = value;
                }
                else
                {
                    throw new Exception("Invalid Name");
                }
            }
        }
    }

    public interface IBook
    {
        void AddGrade(double grade);
        Stats GetStats();
        string Name { get; }
        event GradeAddedDelegate GradeAdded;
    }

    public abstract class Book : NamedObject, IBook
    {
        protected Book(string name) : base(name)
        {
        }

        public abstract event GradeAddedDelegate GradeAdded;
        public abstract void AddGrade(double grade);
        public abstract Stats GetStats();
    }

    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {
        }

        public override event GradeAddedDelegate GradeAdded;

        public override void AddGrade(double grade)
        {
            var writer = File.AppendText($"{Name}.txt");
            writer.WriteLine(grade);
        }

        public override Stats GetStats()
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryBook : Book
    {
        private List<double> grades;
        private string name;

        private double sumOfGrades;
        readonly string category;
        public const string SCHOOL = "Codeworks";
        // can only ever be read, is accessed through the class, not an instance so Book.SCHOOL not book.SCHOOL, because the compiler doesn't need to make a separate instance for each book, it is a universal constant.
        public InMemoryBook(string name) : base(name)
        {
            category = "Misc";
            grades = new List<double>();
            Name = name;
        }
        public char GetLetterGrade(double grade)
        {
            switch (grade)
            {
                case var g when g >= 90:
                    return 'A';
                case var g when g >= 80:
                    return 'B';
                case var g when g >= 70:
                    return 'C';
                case var g when g >= 60:
                    return 'D';
                default:
                    return 'F';

            }
        }
        public override void AddGrade(double grade)
        {
            if (grade >= 0 && grade <= 100)
            {
                grades.Add(grade);

                // how do we inform other pieces of code that this event has taken place?
                if (GradeAdded != null)
                // ^only if someone is listening
                {
                    GradeAdded(this, new EventArgs());
                }
            }
            else
            {
                throw new ArgumentException($"Invalid {nameof(grade)}");
            }
        }

        public override event GradeAddedDelegate GradeAdded;

        public override Stats GetStats()
        {
            var result = new Stats();
            result.Average = 0.0;
            result.HighScore = double.MinValue;
            result.LowScore = 101;
            foreach (var grade in grades)
            {
                result.LowScore = Math.Min(grade, result.LowScore);
                result.HighScore = Math.Max(grade, result.HighScore);
                result.Average += grade;
                // grade.Letter = AddLetterGrade(grade); add per grade in the future
            }
            result.Average /= grades.Count;
            result.Letter = GetLetterGrade(result.Average);
            Console.WriteLine($"Statistics for {Name}'s class:");
            Console.WriteLine($"Number of Students: {grades.Count}");
            return result;
        }
        public void GetSumOfGrades()
        {
            for (int i = 0; i < grades.Count; i++)
            {
                sumOfGrades += grades[i];
            }
        }
        public double GetHighestGrade()
        {
            double highScore = double.MinValue;
            var index = 0;
            while (index < grades.Count)
            {
                highScore = Math.Max(grades[index], highScore);
                index += 1;
            };
            return highScore;
        }
        public double GetLowestGrade()
        {
            double lowScore = 101;
            foreach (var number in grades)
            {
                lowScore = Math.Min(number, lowScore);
            }
            return lowScore;
        }
        public double GetAverageGrade()
        {
            double averageScore = sumOfGrades / grades.Count;
            return averageScore;
        }
        public void DisplayStats()
        {
            Console.WriteLine($"Statistics for {Name}'s class:");
            Console.WriteLine($"Number of Students: {grades.Count}");
        }
    }
}
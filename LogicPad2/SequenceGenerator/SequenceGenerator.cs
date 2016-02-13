using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SequenceGenerator
{
    public class SequenceGenerator
    {
        #region Singleton

        private static SequenceGenerator _instance;

        private SequenceGenerator() {
            InitiateDictionary();        
        }

        public static SequenceGenerator Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new SequenceGenerator();
                }

                return _instance;
            }   
        }

        #endregion

        private IDictionary<int, List<int>> userDict;

        private void InitiateDictionary()
        {
            userDict = new Dictionary<int, List<int>>();

            AddMockData();
        }

        public string GetUserSequence(int userId)
        {
            StringBuilder sb = new StringBuilder();
            SequenceTask.Task task = null;
            if (userId > 0 && userId < 19)
            {
                List<int> list = userDict[userId];
                foreach(int i in list)
                {
                    task = SequenceTask.Instance.TaskDict[i] as SequenceTask.Task;
                    sb.Append("\n");
                    sb.Append(task.ToString());
                }
            }
            return sb.ToString();
        }

        public List<SequenceTask.Task> GetTaskSequence(int userId, UserInterface ui)
        {
            List<int> list = userDict[userId];
            SequenceTask.Task task = null;
            List<SequenceTask.Task> tasks = new List<SequenceTask.Task>();
            foreach (int i in list)
            { 
                task = SequenceTask.Instance.TaskDict[i] as SequenceTask.Task;
                if (task.UserInterface == ui)
                {
                    tasks.Add(task);
                }
            }
            return tasks;
        }

        public void AddMockData()
        {
            List<int> taskSequence;
            int userId = 0;

            taskSequence = new List<int>();
            userId = 1; 
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(29);
            taskSequence.Add(31);
            taskSequence.Add(26);
            taskSequence.Add(27);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(10);
            taskSequence.Add(11);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(5);
            taskSequence.Add(7);
            userDict.Add(userId, taskSequence);


            taskSequence = new List<int>();
            userId = 2;
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(10);
            taskSequence.Add(11);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(29);
            taskSequence.Add(32);
            taskSequence.Add(33);
            taskSequence.Add(35);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 3;
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(6);
            taskSequence.Add(7);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(26);
            taskSequence.Add(27);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(22);
            taskSequence.Add(24);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(17);
            taskSequence.Add(20);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 4;
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(1);
            taskSequence.Add(3);
            userDict.Add(userId, taskSequence);


            taskSequence = new List<int>();
            userId = 5;
            taskSequence.Add(25);
            taskSequence.Add(28);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(2);
            taskSequence.Add(3);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(9);
            taskSequence.Add(11);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 6;
            taskSequence.Add(18);
            taskSequence.Add(19);
            taskSequence.Add(21);
            taskSequence.Add(24);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(5);
            taskSequence.Add(8);
            taskSequence.Add(10);
            taskSequence.Add(11);
            taskSequence.Add(2);
            taskSequence.Add(3);
            taskSequence.Add(29);
            taskSequence.Add(32);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(26);
            taskSequence.Add(27);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 7;
            taskSequence.Add(2);
            taskSequence.Add(3);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(29);
            taskSequence.Add(31);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(21);
            taskSequence.Add(23);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 8;
            taskSequence.Add(21);
            taskSequence.Add(24);
            taskSequence.Add(18);
            taskSequence.Add(20);
            taskSequence.Add(14);
            taskSequence.Add(15);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(1);
            taskSequence.Add(4);
            taskSequence.Add(6);
            taskSequence.Add(7);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(29);
            taskSequence.Add(31);
            taskSequence.Add(25);
            taskSequence.Add(28);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 9;
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(10);
            taskSequence.Add(11);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(18);
            taskSequence.Add(19);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 10;
            taskSequence.Add(18);
            taskSequence.Add(20);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(22);
            taskSequence.Add(23);
            taskSequence.Add(6);
            taskSequence.Add(8);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(9);
            taskSequence.Add(12);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(25);
            taskSequence.Add(28);
            taskSequence.Add(33);
            taskSequence.Add(36);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 11;
            taskSequence.Add(9);
            taskSequence.Add(12);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(5);
            taskSequence.Add(8);
            taskSequence.Add(34);
            taskSequence.Add(36);
            taskSequence.Add(29);
            taskSequence.Add(31);
            taskSequence.Add(26);
            taskSequence.Add(27);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(18);
            taskSequence.Add(19);
            taskSequence.Add(14);
            taskSequence.Add(15);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 12;
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(25);
            taskSequence.Add(28);
            taskSequence.Add(18);
            taskSequence.Add(19);
            taskSequence.Add(22);
            taskSequence.Add(24);
            taskSequence.Add(14);
            taskSequence.Add(15);
            taskSequence.Add(6);
            taskSequence.Add(7);
            taskSequence.Add(10);
            taskSequence.Add(11);
            taskSequence.Add(1);
            taskSequence.Add(4);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 13;
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(18);
            taskSequence.Add(19);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(29);
            taskSequence.Add(31);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 14;
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(9);
            taskSequence.Add(12);
            taskSequence.Add(30);
            taskSequence.Add(31);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(17);
            taskSequence.Add(20);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(21);
            taskSequence.Add(23);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 15;
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(1);
            taskSequence.Add(4);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(5);
            taskSequence.Add(7);
            taskSequence.Add(25);
            taskSequence.Add(28);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(29);
            taskSequence.Add(32);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 16;
            taskSequence.Add(6);
            taskSequence.Add(7);
            taskSequence.Add(10);
            taskSequence.Add(12);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(29);
            taskSequence.Add(32);
            taskSequence.Add(34);
            taskSequence.Add(35);
            taskSequence.Add(26);
            taskSequence.Add(27);
            taskSequence.Add(17);
            taskSequence.Add(20);
            taskSequence.Add(22);
            taskSequence.Add(23);
            taskSequence.Add(14);
            taskSequence.Add(15);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 17;
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(29);
            taskSequence.Add(31);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(1);
            taskSequence.Add(3);
            taskSequence.Add(9);
            taskSequence.Add(11);
            taskSequence.Add(5);
            taskSequence.Add(8);
            userDict.Add(userId, taskSequence);

            taskSequence = new List<int>();
            userId = 18;
            taskSequence.Add(29);
            taskSequence.Add(32);
            taskSequence.Add(25);
            taskSequence.Add(27);
            taskSequence.Add(33);
            taskSequence.Add(35);
            taskSequence.Add(17);
            taskSequence.Add(19);
            taskSequence.Add(13);
            taskSequence.Add(15);
            taskSequence.Add(21);
            taskSequence.Add(23);
            taskSequence.Add(6);
            taskSequence.Add(7);
            taskSequence.Add(1);
            taskSequence.Add(4);
            taskSequence.Add(9);
            taskSequence.Add(11);
            userDict.Add(userId, taskSequence);
        }
  
    }

    //e.g DE1Y
    public class SequenceTask
    {
        #region
        private static SequenceTask _instance;

        private SequenceTask()
        {
            TaskDict = new Dictionary<int, Task>();

            InitiateTaskDictionary();       
        }

        public static SequenceTask Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new SequenceTask();
                }

                return _instance;
            }   
        }

        public IDictionary<int, Task> TaskDict {set; get;}

        #endregion

        private void InitiateTaskDictionary()
        {
            string equation = null;
            string realEquation = null;

            equation = "F = X(Y' + Z'X)";
            realEquation = equation;
            Task task = new Task(UserInterface.dragdrop, DiagramComplexity.easy, 1, true, equation, realEquation);
            TaskDict.Add(1, task);

            equation = "F = X + (Y' + Z'X)";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.easy, 1, false, equation, realEquation);
            TaskDict.Add(2, task);

            equation = "F = (X + Y)'Z'";
            realEquation = equation;
            task = new Task(UserInterface.dragdrop, DiagramComplexity.easy, 2, true, equation, realEquation);
            TaskDict.Add(3, task);

            equation = "F = ((X + Y)Z)'";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.easy, 2, false, equation, realEquation);
            TaskDict.Add(4, task);

            equation = "F = X'Z' + YZ' + XYZ";
            realEquation = equation;
            task = new Task(UserInterface.dragdrop, DiagramComplexity.medium, 1, true, equation, realEquation);
            TaskDict.Add(5, task);

            equation = "F = X'Z' + XY'";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.medium, 1, false, equation, realEquation);
            TaskDict.Add(6, task);

            equation = "F = X + Y";
            realEquation = equation;
            task = new Task(UserInterface.dragdrop, DiagramComplexity.medium, 2, true, equation, realEquation);
            TaskDict.Add(7, task);

            equation = "F = X + Y'";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.medium, 2, false, equation, realEquation);
            TaskDict.Add(8, task);

            equation = "Y = B' + A'C'D'";
            realEquation = equation;
            task = new Task(UserInterface.dragdrop, DiagramComplexity.hard, 1, true, equation, realEquation);
            TaskDict.Add(9, task);

            equation = "Y = B + A C'D'";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.hard, 1, false, equation, realEquation);
            TaskDict.Add(10, task);

            equation = "Y = B'D + CD + AC'";
            realEquation = equation;
            task = new Task(UserInterface.dragdrop, DiagramComplexity.hard, 2, true, equation, realEquation);
            TaskDict.Add(11, task);

            equation = "Y = BD + CD + AC'";
            task = new Task(UserInterface.dragdrop, DiagramComplexity.hard, 2, false, equation, realEquation);
            TaskDict.Add(12, task);

            equation = "F = (X + Y) + X'Z";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.easy, 1, true, equation, realEquation);
            TaskDict.Add(13, task);

            equation = "F = (X XOR Y) + X'Z";
            task = new Task(UserInterface.sketch, DiagramComplexity.easy, 1, false, equation, realEquation);
            TaskDict.Add(14, task);

            equation = "F = X' + Y'Z";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.easy, 2, true, equation, realEquation);
            TaskDict.Add(15, task);

            equation = "F = X + Y'Z";
            task = new Task(UserInterface.sketch, DiagramComplexity.easy, 2, false, equation, realEquation);
            TaskDict.Add(16, task);

            equation = "F = X";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.medium, 1, true, equation, realEquation);
            TaskDict.Add(17, task);

            equation = "F = X'";
            task = new Task(UserInterface.sketch, DiagramComplexity.medium, 1, false, equation, realEquation);
            TaskDict.Add(18, task);

            equation = "F = X' + Y'Z";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.medium, 2, true, equation, realEquation);
            TaskDict.Add(19, task);

            equation = "F = X' + XY'Z'";
            task = new Task(UserInterface.sketch, DiagramComplexity.medium, 2, false, equation, realEquation);
            TaskDict.Add(20, task);

            equation = "Y = AC + AB  + A'B'D";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.hard, 1, true, equation, realEquation);
            TaskDict.Add(21, task);

            equation = "Y = AC' + AB + A'B'D";
            task = new Task(UserInterface.sketch, DiagramComplexity.hard, 1, false, equation, realEquation);
            TaskDict.Add(22, task);

            equation = "Y = BD + AC'D' + ABC' + A'BC + B'CD'";
            realEquation = equation;
            task = new Task(UserInterface.sketch, DiagramComplexity.hard, 2, true, equation, realEquation);
            TaskDict.Add(23, task);

            equation = "Y = BD + ACD";
            task = new Task(UserInterface.sketch, DiagramComplexity.hard, 2, false, equation, realEquation);
            TaskDict.Add(24, task);

            equation = "F = (X XOR Y)' + Z";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.easy, 1, true, equation, realEquation);
            TaskDict.Add(25, task);

            equation = "F = (X XOR Y) + Z";
            task = new Task(UserInterface.hybrid, DiagramComplexity.easy, 1, false, equation, realEquation);
            TaskDict.Add(26, task);

            equation = "F = ((Y XOR Z) + X)'";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.easy, 2, true, equation, realEquation);
            TaskDict.Add(27, task);

            equation = "F = X'(Y'Z + YZ')";
            task = new Task(UserInterface.hybrid, DiagramComplexity.easy, 2, false, equation, realEquation);
            TaskDict.Add(28, task);

            equation = "F = Z + X'Y'";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.medium, 1, true, equation, realEquation);
            TaskDict.Add(29, task);

            equation = "F = Z + Y'";
            task = new Task(UserInterface.hybrid, DiagramComplexity.medium, 1, false, equation, realEquation);
            TaskDict.Add(30, task);

            equation = "F = X' + YZ";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.medium, 2, true, equation, realEquation);
            TaskDict.Add(31, task);

            equation = "F = X + Y'Z";
            task = new Task(UserInterface.hybrid, DiagramComplexity.medium, 2, false, equation, realEquation);
            TaskDict.Add(32, task);

            equation = "Y = CD + AB' + AC + A'C' + A'B + C'D'";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.hard, 1, true, equation, realEquation);
            TaskDict.Add(33, task);

            equation = "Y = AB' + AC + A'C' + A'B + C'D'";
            task = new Task(UserInterface.hybrid, DiagramComplexity.hard, 1, false, equation, realEquation);
            TaskDict.Add(34, task);

            equation = "Y = B'D' + ABD + A'BC";
            realEquation = equation;
            task = new Task(UserInterface.hybrid, DiagramComplexity.hard, 2, true, equation, realEquation);
            TaskDict.Add(35, task);

            equation = "Y = B'(D' + AD) + A'BC";
            task = new Task(UserInterface.hybrid, DiagramComplexity.hard, 2, false, equation, realEquation);
            TaskDict.Add(36, task);
        }

       
        public class Task
        {
            public UserInterface UserInterface{get; set;}
            public DiagramComplexity DiagramComplexity {get; set;}
            public int CaseNmbr{get; set;}
            public bool CaseMatched{get; set;}
            public string Equation { get; set; }
            public string RealEquation { get; set; }
            
            public Task(UserInterface ui, DiagramComplexity dc, int caseNmbr, bool Matched, string equation, string realEquation)
            {
                UserInterface = ui;
                DiagramComplexity = dc;
                CaseNmbr = caseNmbr;
                CaseMatched = Matched;
                Equation = equation;
                RealEquation = realEquation;
            }

            public override string  ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(UserInterface.ToString() + "-" + DiagramComplexity.ToString() + "-" + CaseNmbr.ToString() + "-" + CaseMatched.ToString());
 	            return sb.ToString();
            }
        }
    }
}

namespace Ksu.Cis300.TaskScheduler
{
    public partial class UserInterface : Form
    {
        public UserInterface()
        {
            InitializeComponent();
        }

        private void UserInterface_Load(object sender, EventArgs e)
        {

        }

        // Fields to hold task and schedule data
        private List<Task> _tasks = new List<Task>(); // Holds the list of tasks
        private int _processorCount; // Holds the number of processors
        private int _superPeriod; // Holds the super-period

        // Fields for UI controls
        private DataGridView _scheduleGrid; // Grid to display the schedule
        private Button _loadTasksButton; // Button to load tasks from a file
        private Button _generateButton; // Button to generate the schedule
        private NumericUpDown _processorInput; // Input for the number of processors
        private NumericUpDown _superPeriodInput; // Input for the super-period
        private OpenFileDialog _openFileDialog; // Dialog for selecting the task file


        /// <summary>
        /// The pixel size of the font to use.
        /// </summary>
        private const int _fontSize = 18;

        /// <summary>
        /// The width of the column containing the times.
        /// </summary>
        private int _timeColumnWidth = _fontSize * 4;

        /// <summary>
        /// The width of each column containing tasks assigned to a processor.
        /// </summary>
        private int _taskColumnWidth = _fontSize * ScheduleIO.MaxNameLength;

        /// <summary>
        /// A readonly Font object for the ListView.
        /// </summary>
        private Font _font = new Font(FontFamily.GenericSansSerif, _fontSize, GraphicsUnit.Pixel);

        /// <summary>
        /// A 2D array representing the schedule.
        /// </summary>
        private SchedulingDecision?[,] _schedule = new SchedulingDecision?[0, 0];

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void openFileDialog1_FileOk_1(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// Constructs a new UserInterface and initializes its components.
        /// </summary>




    }
}

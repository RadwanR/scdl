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

        /// <summary>
        /// The pixel size of the font to use.
        /// </summary>
        private const int _fontSize = 18;

        /// <summary>
        /// The width of the column containing the times.
        /// </summary>
        private readonly int _timeColumnWidth = _fontSize * 4;

        /// <summary>
        /// The width of each column containing tasks assigned to a processor.
        /// </summary>
        private readonly int _taskColumnWidth = _fontSize * ScheduleIO.MaxNameLength;

        /// <summary>
        /// A readonly Font object for the ListView.
        /// </summary>
        private readonly Font _font = new Font(FontFamily.GenericSansSerif, _fontSize, GraphicsUnit.Pixel);

        /// <summary>
        /// A 2D array representing the schedule.
        /// </summary>
        private SchedulingDecision?[,] _schedule = new SchedulingDecision?[0, 0];

        /// <summary>
        /// Constructs a new UserInterface and initializes its components.
        /// </summary>
        



    }
}

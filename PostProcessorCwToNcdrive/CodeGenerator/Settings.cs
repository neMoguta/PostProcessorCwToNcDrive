using KellermanSoftware.CompareNetObjects;

namespace PostProcessor.CodeGenerator
{
    public class Settings
    {
        public Settings()
        {
            CirclePoint = new string[3];
            WriteCircle = false;
            Counterclockwise = false;
            Rapid = false;
            DrillCycleOn = false;
            DrillCommand = "";
            MillMoveType = MillMoveTypes.Move;
        }

        public string[] CirclePoint { get; set; }
        public bool WriteCircle { get; set; }
        public bool Counterclockwise { get; set; }
        public bool Rapid { get; set; }
        public bool DrillCycleOn { get; set; }
        public string DrillCommand { get; set; }
        public MillMoveTypes MillMoveType { get; set; }

        public bool NotEqual(Settings other)
        {
            CompareLogic compareLogic = new CompareLogic();
            ComparisonResult result = compareLogic.Compare(this, other);

            return !result.AreEqual;
        }
    }
}

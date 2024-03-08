public enum ObjectiveState
{
    /// <summary>
    /// Requirements not met. Cannot start quest
    /// </summary>
    RequirementsNotMet,
    /// <summary>
    /// Requirements met. Can start quest
    /// </summary>
    CanStart,
    /// <summary>
    /// In progress (started but not completed)
    /// </summary>
    InProgress,
    /// <summary>
    /// All steps completed
    /// </summary>
    Completed, 
    Failed
}

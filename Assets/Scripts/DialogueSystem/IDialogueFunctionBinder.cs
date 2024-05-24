using Ink.Runtime;

namespace YuzuValen.DialogueSystem
{
    public interface IDialogueFunctionBinder
    {
        /// <summary>
        /// This function is called after Story Obj is created but before it is played<br/>
        /// Useful for binding external functions to the story
        /// </summary>
        /// <param name="story">Current Story Object</param>
        void BindFunctions(Story story);
        /// <summary>
        /// This function is called when dialogue is exited<br/>
        /// Useful for unbinding external functions from the story
        /// </summary>
        /// <param name="story"></param>
        void UnbindFunctions(Story story);
    }
}

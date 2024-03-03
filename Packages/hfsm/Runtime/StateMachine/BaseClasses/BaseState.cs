namespace YuzuValen.HFSM
{
    public class BaseState<TStateId>
    {
        public float exitTime = 0;

        /// <summary>
        /// Only run once when the state is created
        /// </summary>
        /// <param name="args">Used to pass any arguments state needs</param>
        public virtual void Init(params object[] args) { }
        /// <summary>
        /// Run every time fsm enters this state
        /// </summary>
        public virtual void OnEnter() { }
        /// <summary>
        /// Run every time fsm exits this state
        /// </summary>
        public virtual void OnExit() { }
        /// <summary>
        /// Run on every fsm update
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// Run on every fsm fixed update
        /// </summary>
        public virtual void FixedUpdate() { }

        public virtual void TriggerEvent(string eventName, params object[] args) { }
    }
}
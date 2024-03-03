using System;
using System.Collections.Generic;

namespace YuzuValen.HFSM
{
    public class StateMachine<TStateId> : BaseState<TStateId>
    {
        #region current & initial state info
        protected TStateId initialStateId;
        protected TStateId currentStateId;
        protected BaseState<TStateId> currentState;
        protected StateBundle<TStateId> currentBundle;
        #endregion
        // List of all parent state machines
        public readonly List<StateMachine<TStateId>> parents = new();
        // List of all state bundle objects, accessible by ID
        public readonly Dictionary<TStateId, StateBundle<TStateId>> stateBundles = new();

        public Func<TStateId> decideState;

        private readonly List<BaseTransition<TStateId>> transitionsFromAny = new();
        private readonly Dictionary<string, List<BaseTransition<TStateId>>> triggerTransitionsFromAny = new();

        /// <summary>
        /// Set initial state of the state machine<br/>
        /// State machine will reset to this state every OnEnter call
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public void SetInitialState(TStateId id)
        {
            if (!stateBundles.ContainsKey(id)) throw new Exception("Add state first!");
            initialStateId = id;
        }
        /// <summary>
        /// Provide a function that will decide the initial state during every OnEnter call
        /// </summary>
        /// <param name="decideState"></param>
        public void SetInitialStateOnEnter(Func<TStateId> decideState)
        {
            this.decideState = decideState;
        }

        #region inherited methods
        /// <summary>
        /// State machine will be active after calling this method
        /// </summary>
        public override void OnEnter()
        {
            if (decideState != null)
                SetInitialState(decideState());
            if (initialStateId == null)
                throw new Exception("Set initial state first!");
            currentStateId = initialStateId;
            currentBundle = stateBundles[initialStateId];
            currentState = currentBundle.state;
            currentState.OnEnter();
        }

        public override void OnExit()
        {
            currentState.OnExit();
        }

        public override void Update()
        {
            currentState.Update();
            CheckTransitions();
        }

        public override void FixedUpdate()
        {
            currentState.FixedUpdate();
        }
        #endregion

        #region Public methods to Add Transition
        /// <summary>
        /// Adds a transition to the state machine<br />
        /// If from state = null, this adds a TransitionFromAny 
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(BaseTransition<TStateId> transition)
        {
            if (EqualityComparer<TStateId>.Default.Equals(transition.from))
            {
                transitionsFromAny.Add(transition);
                return;
            }
            var bundle = stateBundles[transition.from];
            bundle.AddTransition(transition);
        }

        /// <summary>
        /// Add transitions that are only checked on event trigger<br />
        /// If from state = null, this adds a TriggerTransitionFromAny
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="transition"></param>
        public void AddTriggerTransition(string trigger, BaseTransition<TStateId> transition)
        {
            if (EqualityComparer<TStateId>.Default.Equals(transition.from))
                if (triggerTransitionsFromAny.TryGetValue(trigger, out var list))
                    list.Add(transition);
                else
                    triggerTransitionsFromAny.Add(trigger, new List<BaseTransition<TStateId>> { transition });

            var bundle = stateBundles[transition.from];
            if (bundle.triggerTransitionsFrom.TryGetValue(trigger, out var list2))
                list2.Add(transition);
            else
                bundle.triggerTransitionsFrom.Add(trigger, new List<BaseTransition<TStateId>> { transition });
        }
        #endregion

        public void AddState(TStateId id, BaseState<TStateId> state)
        {
            if (state is StateMachine<TStateId>)
            {
                StateMachine<TStateId> sm = state as StateMachine<TStateId>;
                sm.parents.Add(this);
            }
            stateBundles.Add(id, new StateBundle<TStateId>(state));
            state.Init();
        }
        /// <summary>
        /// Will check for transitions linked to the trigger event
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="args">Used to pass any arguments to </param>
        public override void TriggerEvent(string trigger, params object[] args)
        {
            // trigger event on current state 
            currentState.TriggerEvent(trigger, args);

            // check for trigger transitions from any state
            if (triggerTransitionsFromAny.TryGetValue(trigger, out var list))
            {
                foreach (var transition in list)
                {
                    if (transition.ShouldTransition())
                    {
                        DoTransition(transition);
                        return;
                    }
                }
            }
            // check for trigger transitions from current state
            var currentBundle = stateBundles[currentStateId];
            if (currentBundle.triggerTransitionsFrom.TryGetValue(trigger, out var list2))
            {
                foreach (var transition in list2)
                {
                    if (transition.ShouldTransition())
                    {
                        DoTransition(transition);
                        return;
                    }
                }
            }
        }

        private void CheckTransitions()
        {
            foreach (var transition in transitionsFromAny)
            {
                if (transition.ShouldTransition())
                {
                    DoTransition(transition);
                    return;
                }
            }
            foreach (var transition in currentBundle.transitionsFrom)
            {
                if (transition.ShouldTransition())
                {
                    DoTransition(transition);
                    return;
                }
            }
        }
        private void DoTransition(BaseTransition<TStateId> transition)
        {
            transition.BeforeTransition();
            RequestStateChange(transition.to);
            transition.AfterTransition();
        }

        private void RequestStateChange(TStateId stateId)
        {
            if (currentState == null || stateId.Equals(currentStateId))
                return;
            if (currentState.exitTime < 0.001)
                SetState(stateId);
        }
        public void SetState(TStateId stateId)
        {
            if (stateBundles.TryGetValue(stateId, out var bundle))
            {
                currentState?.OnExit();
                currentStateId = stateId;
                currentBundle = bundle;
                currentState = currentBundle.state;
                currentState.OnEnter();
            }
            else
            {
                foreach (StateMachine<TStateId> sm in parents)
                {
                    if (sm.stateBundles.ContainsKey(stateId))
                    {
                        // set parent sm to new state
                        sm.SetState(stateId);
                    }
                }
            }
        }
        /// <summary>
        /// Returns a list of current and all nested state IDs
        /// </summary>
        /// <returns></returns>
        public List<TStateId> GetAllCurrentStates()
        {
            List<TStateId> states = new();

            StateMachine<TStateId> temp = this;
            while (temp is StateMachine<TStateId>)
            {
                states.Add(temp.currentStateId);
                temp = temp.currentState as StateMachine<TStateId>;
            }
            return states;
        }
    }
}
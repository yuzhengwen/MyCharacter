using YuzuValen.HFSM;

namespace YuzuValen
{
    public class GroundSM: PlayerSM
    {
        public GroundSM(string id, PlayerMovement player) : base(id, player)
        {
        }
        public override void Init()
        {
            parent.AddTriggerTransition("JumpInput", PlayerState.Grounded, PlayerState.AirborneState.Jumping);
        }
    }
}

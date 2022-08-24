using rpsls.Domain.Values;

namespace rpsls.Application.Models
{
    public class PlayerInput
    {
        public ChallengeValue Challenge { get; set; }
        public string Name { get; set; }
        public int Won { get; internal set; }
    }
}
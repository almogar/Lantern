
namespace Assets.Scripting
{
    public enum Status { Dead, Waiting, Active, Summon }

    public static class StatusExtensions
    {
        public static Status changeStatus(this Status corrent)
        {
            if (corrent.Equals(Status.Dead) || corrent.Equals(Status.Summon))
                return corrent;
            if (corrent.Equals(Status.Active))
                 return Status.Waiting;
            return Status.Active;
        }
    }
}

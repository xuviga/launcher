using System.Collections.Generic;

namespace iMine.Launcher.Hasher
{
    public class StrongFileNameMatcher : FileNameMatcher
    {
        public StrongFileNameMatcher() : base(new string[0], new string[0], new string[0])
        {
        }

        public override bool ShouldUpdate(ICollection<string> path)
        {
            return true;
        }
    }
}
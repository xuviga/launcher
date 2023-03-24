using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace iMine.Launcher.Hasher
{
    public class FileNameMatcher
    {
        private static readonly Entry[] NoEntries = new Entry[0];

        private readonly Entry[] update;

        private readonly Entry[] verify;
        private readonly Entry[] exclusions;

        public FileNameMatcher(string[] update, string[] verify, string[] exclusions)
        {
            this.update = ToEntries(update);
            this.verify = ToEntries(verify);
            this.exclusions = ToEntries(exclusions);
        }

        private FileNameMatcher(Entry[] update, Entry[] verify, Entry[] exclusions)
        {
            this.update = update;
            this.verify = verify;
            this.exclusions = exclusions;
        }

        public virtual bool ShouldUpdate(ICollection<string> path)
        {
            return (AnyMatch(update, path) || AnyMatch(verify, path)) && !AnyMatch(exclusions, path);
        }

        private static bool AnyMatch(Entry[] entries, ICollection<string> path)
        {
            return entries.Any(it => it.Matches(path));
        }

        private static Entry[] ToEntries(params string[] entries)
        {
            return entries.ToList().ConvertAll(it => new Entry(it)).ToArray();
        }

        private class Entry
        {
            private static readonly Regex Splitter = new Regex("/+");
            private readonly List<Regex> Parts;

            public Entry(string exclusion)
            {
                Parts = Splitter.Split(exclusion).ToList().ConvertAll(it=>new Regex(it));
            }

            public bool Matches(ICollection<string> path)
            {
                if (Parts.Count > path.Count)
                    return false;

                var iterator = path.GetEnumerator();
                foreach (var part in Parts)
                {
                    iterator.MoveNext();
                    var pathPart = iterator.Current;
                    if (!part.Match(pathPart).Success)
                        return false;
                }

                return true;
            }
        }
    }
}
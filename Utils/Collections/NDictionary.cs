using System.Collections.Generic;

namespace iMine.Launcher.Utils.Collections
{
    public class NDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public NDictionary() { }

        public NDictionary(IDictionary<TKey, TValue> original) : base(original) { }

        public new TValue this[TKey key]
        {
            get
            {
                TValue value;
                return TryGetValue(key, out value) ? value : default(TValue);
            }
            set => base[key] = value;
        }
    }
}
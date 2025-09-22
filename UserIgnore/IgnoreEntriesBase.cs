using Core;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserIgnore
{
    [DataContract]
    public class IgnoreEntriesBase
    {
        protected long[] _Entries;
        protected IgnoreEntriesBase() { }
        public void Add(long userId)
        {
            lock (this)
            {
                int oldLength;
                if (_Entries==null||(oldLength = _Entries.Length)<1)
                {
                    _Entries = new long[] {userId};
                    return;
                }
                long[] newEntries = new long[oldLength + 1];
                int insertIndex = (int)BinarySearchHelper.BinarySearchWithOverflowEachEnd(
                    _Entries, u => userId > u ? 1 : (userId < u ? -1 : 0), out bool wasExactMatch)!;
                if (wasExactMatch)
                    return;
                if(insertIndex>0)
                    Array.Copy(_Entries, 0, newEntries, 0, insertIndex);
                newEntries[insertIndex] = userId;
                int secondLength = oldLength - insertIndex;
                if(secondLength>0) 
                    Array.Copy(_Entries, insertIndex, newEntries, insertIndex + 1, secondLength);
                _Entries = newEntries;
            }
        }
        public void Remove(long userId)
        {
            lock (this)
            {
                if (_Entries == null)
                {
                    return;
                }
                int oldLength = _Entries.Length;
                if (oldLength < 1)
                {
                    return;
                }
                int? indexNullable = BinarySearchHelper.BinarySearchWithOverflowEachEnd(
                    _Entries, u => userId > u ? 1 : (userId < u ? -1 : 0), out bool ignore,
                    exactMatch: true, roundUpOnEquals:false);
                if (indexNullable == null) return;
                long[] newEntries = new long[oldLength - 1];
                int index = (int)indexNullable!;
                if (index > 0)
                    Array.Copy(_Entries, 0, newEntries, 0, index);
                int secondLength = oldLength - (index + 1);
                if (secondLength > 0)
                    Array.Copy(_Entries, index + 1, newEntries, index, secondLength);
                _Entries = newEntries;
            }
        }
    }
}

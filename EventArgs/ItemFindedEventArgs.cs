using System.IO;

namespace Task1.EventArgs
{
    public class ItemFindedEventArgs<T> : System.EventArgs 
        where T : FileSystemInfo
    {
        public T FindedItem { get; set; }
        public ActionType ActionType { get; set; }
    }
}
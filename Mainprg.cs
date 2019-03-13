using System;

namespace Task1
{
    class Mainprg
    {
        static void Main(string[] args)
        {
            string startPoint = "c:\\dev";
            var visitor = new FileSystemVisitor(startPoint, new FileSystemProcessingStrategy(), (info) => true);
            visitor.Start += (s, e) =>
            {
                Console.WriteLine("Iteration started");
            };

            visitor.Finish += (s, e) =>
            {
                Console.WriteLine("Iteration finished");
            };

            visitor.FileFinded += (s, e) =>
            {
                Console.WriteLine("\tFounded file: " + e.FindedItem.Name);
            };

            visitor.DirectoryFinded += (s, e) =>
            {
                Console.WriteLine("\tFounded directory: " + e.FindedItem.Name);
                if (e.FindedItem.Name.Length == 4)
                {
                    e.ActionType = ActionType.StopSearch;
                }
            };

            visitor.FilteredFileFinded += (s, e) =>
            {
                Console.WriteLine("Founded filtered file: " + e.FindedItem.Name);
            };

            visitor.FilteredDirectoryFinded += (s, e) =>
            {
                Console.WriteLine("Founded filtered directory: " + e.FindedItem.Name);
                if (e.FindedItem.Name.Length == 4)
                    e.ActionType = ActionType.StopSearch;
            };

            foreach (var fileSysInfo in visitor.GetFileSystemInfoSequence())
            {
                Console.WriteLine(fileSysInfo);
            }
        }
    }
}

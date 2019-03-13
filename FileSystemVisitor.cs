using System;
using System.Collections.Generic;
using System.IO;
using Task1.EventArgs;

namespace Task1
{
    public class FileSystemVisitor
    {
        private readonly DirectoryInfo _startDirectory;
        private readonly Func<FileSystemInfo, bool> _filter;
        private readonly IFileSystemProcessingStrategy _fileSystemProcessingStrategy;

        public FileSystemVisitor(string path,
            IFileSystemProcessingStrategy fileSystemProcessingStrategy,
            Func<FileSystemInfo, bool> filter = null)
            : this(new DirectoryInfo(path), fileSystemProcessingStrategy, filter) { }

        public FileSystemVisitor(DirectoryInfo startDirectory,
            IFileSystemProcessingStrategy fileSystemProcessingStrategy,
            Func<FileSystemInfo, bool> filter = null)
        {
            _startDirectory = startDirectory;
            _filter = filter;
            _fileSystemProcessingStrategy = fileSystemProcessingStrategy;
        }

        public event EventHandler<StartEventArgs> Start;
        public event EventHandler<FinishEventArgs> Finish;
        public event EventHandler<ItemFindedEventArgs<FileInfo>> FileFinded;
        public event EventHandler<ItemFindedEventArgs<FileInfo>> FilteredFileFinded;
        public event EventHandler<ItemFindedEventArgs<DirectoryInfo>> DirectoryFinded;
        public event EventHandler<ItemFindedEventArgs<DirectoryInfo>> FilteredDirectoryFinded;

        public IEnumerable<FileSystemInfo> GetFileSystemInfoSequence()
        {
            OnEvent(Start, new StartEventArgs());
            foreach (var fileSystemInfo in BypassFileSystem(_startDirectory, CurrentAction.ContinueSearch))
            {
                yield return fileSystemInfo;
            }
            OnEvent(Finish, new FinishEventArgs());
        }

        private IEnumerable<FileSystemInfo> BypassFileSystem(DirectoryInfo directory, CurrentAction currentAction)
        {
            foreach (var fileSystemInfo in directory.EnumerateFileSystemInfos())
            {
                if (fileSystemInfo is FileInfo file)
                {
                    currentAction.Action = ProcessFile(file);
                }

                if (fileSystemInfo is DirectoryInfo dir)
                {
                    currentAction.Action = ProcessDirectory(dir);
                    if (currentAction.Action == ActionType.ContinueSearch)
                    {
                        yield return dir;
                        foreach (var innerInfo in BypassFileSystem(dir, currentAction))
                        {
                            yield return innerInfo;
                        }
                        continue;
                    }
                }
                
                if (currentAction.Action == ActionType.StopSearch)
                {
                    yield break;
                }

                yield return fileSystemInfo;
            }
        }

        private ActionType ProcessFile(FileInfo file)
        {
            return _fileSystemProcessingStrategy
                .ProcessItemFinded(file, _filter, FileFinded, FilteredFileFinded, OnEvent);
        }

        private ActionType ProcessDirectory(DirectoryInfo directory)
        {
            return _fileSystemProcessingStrategy
                .ProcessItemFinded(directory, _filter, DirectoryFinded, FilteredDirectoryFinded, OnEvent);
        }

        private void OnEvent<TArgs>(EventHandler<TArgs> someEvent, TArgs args)
        {
            someEvent?.Invoke(this, args);
        }

        private class CurrentAction
        {
            public ActionType Action { get; set; }
            public static CurrentAction ContinueSearch 
                => new CurrentAction { Action = ActionType.ContinueSearch };
        }
    }
}

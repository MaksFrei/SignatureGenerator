using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace SignatureGenerator.Generator.Producers.CollectionProducers
{
    internal class FileToConsoleProducer : ICollectionProducer<string, ILogger<Generator>>
    {
        /// <summary>
        /// Flag for current work status
        /// </summary>
        public bool DoesWorkDone { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ILogger<Generator> ProducedData { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public FileToConsoleProducer()
        {
            SyncEvent = new ManualResetEventSlim(true);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collection">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<string, ILogger<Generator>>
            SetProducedData(ILogger<Generator> data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            ProducedData = data;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="consumedData"><inheritdoc/></param>
        /// <param name="doesConsumedCollectionFinishedCallback"><inheritdoc/></param>
        /// <param name="cancellationToken"><inheritdoc/></param>
        public void Produce(string consumedData, Func<bool> doesConsumedCollectionFinishedCallback, CancellationToken cancellationToken = default)
        {
            if (ProducedData is null) throw new ArgumentNullException(nameof(ProducedData));
            if (consumedData is null) throw new ArgumentNullException(nameof(consumedData));

            DoesWorkDone = false;
            while (TryToGetOldestFile(consumedData, out var file) || !doesConsumedCollectionFinishedCallback.Invoke())
            {
                if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                SyncEvent.Wait(cancellationToken);
                
                if (file != null && !IsFileLocked(file))
                {
                    Console.WriteLine(File.ReadAllText(file.FullName));
                    File.Delete(file.FullName);
                }
            }
            DoesWorkDone = true;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            SyncEvent.Dispose();
        }

        private bool TryToGetOldestFile(string path, out FileInfo file)
        {
            file = Directory.GetFiles(path)
                .Select(x => new FileInfo(x))
                .OrderByDescending(x => x.LastWriteTime)
                .LastOrDefault();

            return file != null;
        }

        private bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }

            return false;
        }
    }
}

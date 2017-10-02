using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Util.Internal;
using Akka.Persistence;
using Akka.Persistence.Journal;
using Akka.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace PersistencePlugins
{
    using Messages = List<Persistent>;

    public class FileJournal : AsyncWriteJournal
    {
        JsonSerializerSettings _serializationsettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };

        private Messages _messages = new Messages();

        private string _folder;

        protected virtual Messages Messages { get { return _messages; } }

        public FileJournal(Config config)
        {
            _folder = config.GetString("folder");

            if (_folder == null)
            {
                throw new ConfigurationException("Setting 'folder' was not specified in the FileJournal configuration.");
            }

            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }

        protected override Task<IImmutableList<Exception>> WriteMessagesAsync(IEnumerable<AtomicWrite> messages)
        {
            foreach (var w in messages)
            {
                LoadMessages(w.PersistenceId);
                foreach (var p in (IEnumerable<IPersistentRepresentation>)w.Payload)
                {
                    Add(p);
                }
                SaveMessages(w.PersistenceId);
            }

            return Task.FromResult((IImmutableList<Exception>)null); // all good
        }

        public override Task<long> ReadHighestSequenceNrAsync(string persistenceId, long fromSequenceNr)
        {
            LoadMessages(persistenceId);
            return Task.FromResult(HighestSequenceNr(persistenceId));
        }

        public override Task ReplayMessagesAsync(IActorContext context, string persistenceId, long fromSequenceNr, long toSequenceNr, long max,
            Action<IPersistentRepresentation> recoveryCallback)
        {
            LoadMessages(persistenceId);
            var highest = HighestSequenceNr(persistenceId);
            if (highest != 0L && max != 0L)
                Read(persistenceId, fromSequenceNr, Math.Min(toSequenceNr, highest), max).ForEach(recoveryCallback);
            return Task.FromResult(new object());
        }

        protected override Task DeleteMessagesToAsync(string persistenceId, long toSequenceNr)
        {
            LoadMessages(persistenceId);
            var highestSeqNr = HighestSequenceNr(persistenceId);
            var toSeqNr = Math.Min(toSequenceNr, highestSeqNr);
            for (var snr = 1L; snr <= toSeqNr; snr++)
                Delete(persistenceId, snr);
            SaveMessages(persistenceId);
            return Task.FromResult(new object());
        }

        private Messages Add(IPersistentRepresentation persistent)
        {
            Messages.Add((Persistent)persistent);
            return Messages;
        }

        private Messages Delete(string pid, long seqNr)
        {
            Messages.RemoveAt((int)seqNr);
            return Messages;
        }

        private IEnumerable<IPersistentRepresentation> Read(string pid, long fromSeqNr, long toSeqNr, long max)
        {
            if (Messages.Count > 0)
            {
                return Messages
                    .Where(x => x.SequenceNr >= fromSeqNr && x.SequenceNr <= toSeqNr)
                    .Take(max > int.MaxValue ? int.MaxValue : (int)max);
            }

            return Enumerable.Empty<IPersistentRepresentation>();
        }

        private long HighestSequenceNr(string pid)
        {
            if (Messages.Count > 0)
            {
                var last = Messages.LastOrDefault();
                return last?.SequenceNr ?? 0L;
            }

            return 0L;
        }

        private void LoadMessages(string persistenceId)
        {
            string filePath = Path.Combine(_folder, $"{persistenceId}.journal.json");
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                
                _messages = JsonConvert.DeserializeObject<Messages>(jsonData, _serializationsettings);
            }
            else
            {
                _messages = new Messages();
            }
        }

        private void SaveMessages(string persistenceId)
        {
            string filePath = Path.Combine(_folder, $"{persistenceId}.journal.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string jsonData = JsonConvert.SerializeObject(Messages, _serializationsettings);
            File.WriteAllText(filePath, jsonData);
        }
    }
}

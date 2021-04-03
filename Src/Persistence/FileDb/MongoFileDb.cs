namespace Isitar.FileStorage.Persistence.FileDb
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Interfaces;
    using Contract;
    using MongoDB.Driver;
    using Attachment = Application.Entities.Attachment;

    public class MongoFileDb : IFileDb, IDisposable
    {
        private readonly MongoClient mongoClient;
        private readonly IMongoCollection<Attachment> attachments;
        private IClientSessionHandle session;

        public MongoFileDb(MongoDbSettings dbSettings)
        {
            mongoClient = new MongoClient(dbSettings.ConnectionString);
            var database = mongoClient.GetDatabase("Attachments");
            attachments = database.GetCollection<Attachment>("Attachments");
        }

        private async Task EnsureTransaction()
        {
            if (null == session)
            {
                session = await mongoClient.StartSessionAsync();
                try
                {
                    session.StartTransaction();
                }
                catch
                {
                    // ignored
                }
            }
        }


        public IQueryable<Attachment> Attachments()
        {
            return attachments.AsQueryable();
        }

        public async Task<Attachment> ByIdAsync(Guid id)
        {
            var document = await attachments
                .FindAsync(a => a.Id.Equals(id));
            return await document.FirstOrDefaultAsync();
        }

        public async Task<Attachment> ByIdAndSizeAsync(Guid id, AttachmentSize size)
        {
            
            var document = await attachments
                .FindAsync(a => size == AttachmentSize.MEDIA_SIZE_ORIG && a.Id.Equals(id)
                                                                         || a.OriginalAttachmentId.Equals(id) && a.Size.Equals(size));

            return await document.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Attachment attachment, CancellationToken cancellationToken)
        {
            await EnsureTransaction();
            if (attachment.OriginalAttachmentId.HasValue)
            {
                var origAttachments = await attachments.Find(session, a => a.Id.Equals(attachment.OriginalAttachmentId.Value))
                    .ToListAsync(cancellationToken);
                foreach (var origAttachment in origAttachments)
                {
                    origAttachment.SubAttachments.Add(attachment);
                    await attachments.ReplaceOneAsync(session, a => a.Id.Equals(origAttachment.Id), origAttachment, cancellationToken: cancellationToken);
                }
            }

            await attachments.InsertOneAsync(session, attachment, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await EnsureTransaction();
            await attachments.DeleteManyAsync(session, a => a.Id.Equals(id)
                                                            || a.OriginalAttachmentId.HasValue && a.OriginalAttachmentId.Value.Equals(id),
                cancellationToken: cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await EnsureTransaction();
            if (session.IsInTransaction)
            {
                await session.CommitTransactionAsync(cancellationToken);
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            await EnsureTransaction();
            if (session.IsInTransaction)
            {
                await session.AbortTransactionAsync(cancellationToken);
            }
        }

        public void Dispose()
        {
            if (null != session)
            {
                if (session.IsInTransaction)
                {
                    session.AbortTransaction();
                }

                session.Dispose();
            }
        }
    }
}
using Microsoft.EntityFrameworkCore;

namespace Trackify.DataModels
{
    internal interface IDatabaseContext
    {
        DbSet<TimeAcquisition> TimeAcquisitions { get; set; }

        void EnsureDatabaseIsCreated();

        void SaveChanges();

        void ClearTimeAcquisitions();

        string StorageLocation { get; }
    }
}
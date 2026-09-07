using WaDesktop.Domain.Entities;

namespace WaDesktop.Domain.Interfaces
{
    public interface IPersistedSessionStore
    {
        PersistedSession Load();
        void Save(PersistedSession session);
        void Delete();
    }
}

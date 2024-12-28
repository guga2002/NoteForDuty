using Note.Models;

namespace Speaker.leison.Sistem.layer.Repositories
{
    public interface IUdpComunicationRepository
    {
        Task<List<ExcellDataMode3l>> ReceiveAsync();
    }
}
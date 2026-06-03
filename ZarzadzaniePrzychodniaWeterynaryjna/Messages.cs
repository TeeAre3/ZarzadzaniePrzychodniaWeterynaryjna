using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    public class GoToConsultationMessage(Appointment r)
    {
        public Appointment Appointment { get; } = r;
    }
    public class ClientChangedMessage { }
    public class PatientChangedMessage { }
    public class CatalogChangedMessage { }
    public class ConsultationEndedMessage { }
}
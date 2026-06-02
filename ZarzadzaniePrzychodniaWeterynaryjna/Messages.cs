using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    public class GoToConsultationMessage
    {
        public Appointment Appointment { get; }
        public GoToConsultationMessage(Appointment r) => Appointment = r;
    }
    public class ClientChangedMessage { }
    public class PatientChangedMessage { }
    public class CatalogChangedMessage { }
    public class ConsultationEndedMessage { }
}
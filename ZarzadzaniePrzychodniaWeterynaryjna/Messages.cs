using ZarzadzaniePrzychodniaWeterynaryjna.Models;

namespace ZarzadzaniePrzychodniaWeterynaryjna
{
    public class PrzejdzDoGabinetuMessage
    {
        public Harmonogram Rezerwacja { get; }
        public PrzejdzDoGabinetuMessage(Harmonogram r) => Rezerwacja = r;
    }
    public class WlascicielZmienionyMessage { }
    public class ZwierzeZmienioneMessage { }
    public class KatalogZmienionyMessage { }
    public class WizytaZakonczonaMessage { }
}
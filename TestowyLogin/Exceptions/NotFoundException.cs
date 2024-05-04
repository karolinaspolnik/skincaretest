namespace TestowyLogin.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) //nie musimy sprawdzac czy cos jest nullem np przy pobieraniu wyswietlaniu usuwaniu
        {
            
        }
    }
}

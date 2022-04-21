namespace LinkMicroservice.Services
{
    public class CreationService
    {
        public int AddNumbers(int number1, int number2)
        {
            return number1 + number2;
        }

        public int SubtractNumbers(int number1, int number2)
        {
            return number1 - number2;
        }

        public CreationService()
        {

        }
    }
}

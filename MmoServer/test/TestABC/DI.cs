
namespace TestAOI
{
    public class Application
    {
        public PaymentService service;

        public Application()
        {
            service = new PaymentService();
        }
    }
    public class PaymentService
    {
        public int price;

        public PaymentService()
        {
            price = 0;
        }
    }
}

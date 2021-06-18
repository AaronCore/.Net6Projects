namespace VueAdmin.Application.HelloWorld
{
    public class HelloWorldService : VueAdminApplicationService, IHelloWorldService
    {
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}

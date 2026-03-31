public class Hello
{
    public void SayHello()
    {
        Console.WriteLine("Hello from Hello class!");
    }
}
class Program
{
    static void Main(string[] args)
    {
        var hello = new Hello();
        hello.SayHello();
    }
}